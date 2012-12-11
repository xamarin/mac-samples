//
// AsyncTests.Framework.TestSuite
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using Mono.Addins;

[assembly:AddinRoot ("AsyncTests", "0.1")]

namespace AsyncTests.Framework {

	public class TestSuite {
		Assembly assembly;
		List<TestFixture> fixtures;
		static readonly ITestFramework[] frameworks;
		static readonly ITestHost[] hosts;
		static readonly ITestCategory[] categories;
		static readonly ITestCategoryFilter[] filters;
		static readonly Configuration config;

		static TestSuite ()
		{
			AddinManager.Initialize ();
			AddinManager.Registry.Update ();

			frameworks = AddinManager.GetExtensionObjects<ITestFramework> () ?? new ITestFramework[0];
			hosts = AddinManager.GetExtensionObjects<ITestHost> () ?? new ITestHost[0];
			categories = AddinManager.GetExtensionObjects<ITestCategory> () ?? new ITestCategory[0];

			filters = categories.Where (category => category is ITestCategoryFilter).
				Select (x => (ITestCategoryFilter)x).ToArray ();

			config = ConfigurationManager.OpenExeConfiguration (ConfigurationUserLevel.PerUserRoamingAndLocal);
		}

		TestSuite (Assembly assembly)
		{
			this.assembly = assembly;
		}

		public Assembly Assembly {
			get { return assembly; }
		}

		public static Configuration Configuration {
			get { return config; }
		}

		public string Name {
			get { return assembly.GetName ().Name; }
		}

		public static ITestCategory[] Categories {
			get { return categories; }
		}

		internal static ITestCategoryFilter[] CategoryFilters {
			get { return filters; }
		}

		public IList<TestFixture> Fixtures {
			get { return fixtures.AsReadOnly (); }
		}

		public int CountFixtures {
			get { return fixtures.Count; }
		}

		public int CountTests {
			get { return fixtures.Sum (fixture => fixture.CountTests); }
		}

		internal static bool Filter (TestCase test, ITestRunner runner, ITestCategory category)
		{
			if (!test.IsEnabled || !category.IsEnabled (test) || !runner.IsEnabled (test))
				return false;
			if (test.Categories.Where (c => !c.Equals (category)).Any (c => c.Explicit))
				return false;
			return filters.Where (f => !f.Equals (category)).All (f => f.Filter (test));
		}

		public static T GetConfiguration<T> () where T : TestConfiguration
		{
			return (T)GetConfiguration (typeof (T));
		}

		internal static TestConfiguration GetConfiguration (Type type)
		{
			var name = type.FullName;
			var section = (TestConfiguration)config.GetSection (name);
			config.Save (ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection (name);

			if (section == null) {
				section = (TestConfiguration)Activator.CreateInstance (type);
				config.Sections.Add (name, section);
				config.Save (ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection (name);
			}

			return section;
		}

		public static Task<TestSuite> Create (Assembly assembly)
		{
			var tcs = new TaskCompletionSource<TestSuite> ();

			ThreadPool.QueueUserWorkItem (_ => {
				try {
					var suite = new TestSuite (assembly);
					suite.DoResolve ();
					tcs.SetResult (suite);
				} catch (Exception ex) {
					tcs.SetException (ex);
				}
			}
			);

			return tcs.Task;
		}

		void DoResolve ()
		{
			fixtures = new List<TestFixture> ();
			foreach (var framework in frameworks) {
				if (!IsEqualOrSubclassOf<AsyncTestFixtureAttribute> (framework.FixtureAttributeType)) {
					Log ("Framework '{0}' has invalid FixtureAttributeType '{1}'",
					     framework.GetType ().FullName, framework.FixtureAttributeType.FullName);
					continue;
				}
				if (!IsEqualOrSubclassOf<AsyncTestAttribute> (framework.TestAttributeType)) {
					Log ("Framework '{0}' has invalid FixtureAttributeType '{1}'",
					     framework.GetType ().FullName, framework.TestAttributeType.FullName);
					continue;
				}
				if (!IsEqualOrSubclassOf<TestContext> (framework.ContextType)) {
					Log ("Framework '{0}' has invalid ContextType '{1}'",
					     framework.GetType ().FullName, framework.ContextType.FullName);
					continue;
				}

				foreach (var type in assembly.GetExportedTypes ()) {
					var attr = (AsyncTestFixtureAttribute)type.GetCustomAttribute (
						framework.FixtureAttributeType, false);
					if ((attr == null) || attr.GetType ().IsSubclassOf (framework.FixtureAttributeType))
						continue;

					fixtures.Add (new TestFixture (this, framework, attr, type));
				}
			}

			foreach (var fixture in fixtures) {
				fixture.Resolve ();
			}
		}

		bool IsEqualOrSubclassOf<T> (Type type)
		{
			return type.Equals (typeof (T)) || type.IsSubclassOf (typeof (T));
		}

		internal int CurrentIteration {
			get;
			private set;
		}

		internal int MaxIterations {
			get;
			private set;
		}

		public static ITestCategory AllTests = new AllTestsCategory ();

		public Task<TestResultCollection> Run (CancellationToken cancellationToken)
		{
			return Run (AllTests, 1, cancellationToken);
		}

		public async Task<TestResultCollection> Run (ITestCategory category, int repeatCount,
		                                             CancellationToken cancellationToken)
		{
			var result = new TestResultCollection (Name);

			try {
				for (int i = 0; i < hosts.Length; i++)
					await hosts [i].SetUp (this);
			} catch (Exception ex) {
				Log ("SetUp failed: {0}", ex);
				result.AddChild (new TestError (Name, "SetUp failed", ex));
				return result;
			}

			if (repeatCount == 1) {
				CurrentIteration = MaxIterations = 1;
				await DoRun (category, result, cancellationToken);
			} else {
				MaxIterations = repeatCount;
				for (CurrentIteration = 0; CurrentIteration < repeatCount; CurrentIteration++) {
					var name = string.Format ("{0} (iteration {1})", Name, CurrentIteration + 1);
					var iteration = new TestResultCollection (name);
					result.AddChild (iteration);
					await DoRun (category, iteration, cancellationToken);
				}
			}

			try {
				for (int i = 0; i < hosts.Length; i++)
					await hosts [i].TearDown (this);
			} catch (Exception ex) {
				Log ("TearDown failed: {0}", ex);
				result.AddChild (new TestError (Name, "TearDown failed", ex));
			}

			OnStatusMessageEvent ("Test suite finished.");

			return result;
		}

		async Task DoRun (ITestCategory category, TestResultCollection result,
		                  CancellationToken cancellationToken)
		{
			foreach (var fixture in fixtures) {
				if (!fixture.IsEnabled (category))
					continue;
				try {
					result.AddChild (await fixture.Run (category, cancellationToken));
				} catch (Exception ex) {
					Log ("Test fixture {0} failed: {1}", fixture.Name, ex);
					result.AddChild (new TestError (fixture.Name, "Test fixture failed", ex));
				}
			}
		}

		protected internal void Log (string message, params object[] args)
		{
			Debug.WriteLine (string.Format (message, args), "TestSuite");
		}

		public event EventHandler<StatusMessageEventArgs> StatusMessageEvent;

		protected internal void OnStatusMessageEvent (string message, params object[] args)
		{
			OnStatusMessageEvent (new StatusMessageEventArgs (string.Format (message, args)));
		}

		protected void OnStatusMessageEvent (StatusMessageEventArgs args)
		{
			if (StatusMessageEvent != null)
				StatusMessageEvent (this, args);
		}

		public class StatusMessageEventArgs : EventArgs {
			public string Message {
				get;
				private set;
			}

			public Exception Error {
				get;
				private set;
			}

			public StatusMessageEventArgs (string message)
			{
				this.Message = message;
			}

			public StatusMessageEventArgs (string message, Exception error)
			{
				this.Message = message;
				this.Error = error;
			}
		}
	}
}
