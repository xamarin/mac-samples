//
// AsyncTests.Framework.TestFixture
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
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTests.Framework
{
	public sealed class TestFixture
	{
		public TestSuite Suite {
			get;
			private set;
		}

		public ITestFramework Framework {
			get;
			private set;
		}

		public AsyncTestFixtureAttribute Attribute {
			get;
			private set;
		}

		public IList<TestCategoryAttribute> Categories {
			get;
			private set;
		}

		public IList<TestWarning> Warnings {
			get;
			private set;
		}

		public TestConfiguration Configuration {
			get { return config; }
		}

		public Type Type {
			get;
			private set;
		}

		public string Name {
			get { return Type.Name; }
		}

		public int CountTests {
			get { return tests.Length; }
		}

		public TestCase[] Tests {
			get { return tests; }
		}

		bool disabled;
		TestConfiguration config;
		TestCase[] tests;
		MethodInfo setup, teardown;

		internal TestFixture (TestSuite suite, ITestFramework framework,
		                      AsyncTestFixtureAttribute attr, Type type)
		{
			this.Suite = suite;
			this.Framework = framework;
			this.Attribute = attr;
			this.Type = type;

			IDictionary<Type, TestCategoryAttribute> categories;
			IList<TestWarning> warnings;

			Resolve (suite, null, type, out categories, out warnings, out config, out disabled);
			Categories = categories.Values.ToList ().AsReadOnly ();
			Warnings = warnings;
		}

		internal static void Resolve (
			TestSuite suite, TestFixture parent, MemberInfo member,
			out IDictionary<Type, TestCategoryAttribute> categories,
			out IList<TestWarning> warnings, out TestConfiguration config,
			out bool disabled)
		{
			disabled = false;
			config = null;

			bool hasConfig = false;
			warnings = new List<TestWarning> ();
			categories = new Dictionary<Type, TestCategoryAttribute> ();

			if (parent != null) {
				config = parent.Configuration;
				hasConfig = config != null;
				foreach (var category in parent.Categories)
					categories [category.GetType ()] = category;
			}

			string fullName;
			if (member is Type)
				fullName = ((Type)member).FullName;
			else if (member is MethodInfo) {
				var method = (MethodInfo)member;
				fullName = method.DeclaringType.FullName + "." + method.Name;
			} else {
				fullName = member.ToString ();
			}

			var attrs = member.GetCustomAttributes (typeof(TestCategoryAttribute), false);

			foreach (var obj in attrs) {
				var category = obj as TestCategoryAttribute;
				if (category == null)
					continue;

				if (categories.ContainsKey (category.GetType ())) {
					suite.Log ("Duplicate [{0}] in {1}.",
					           category.GetType ().FullName, fullName);
					continue;
				}

				var configAttr = obj as ConfigurableTestCategoryAttribute;
				if (configAttr == null) {
					categories [category.GetType ()] = category;
					continue;
				}

				if (hasConfig) {
					suite.Log ("Only one single [ConfigurableTestCategory] is " +
					           "allowed in {0}", fullName);
					continue;
				}

				config = configAttr.Resolve ();
				if ((config != null) && config.IsEnabled)
					categories [category.GetType ()] = category;
				else
					disabled = true;
			}

			var wattrs = member.GetCustomAttributes (typeof(TestWarningAttribute), false);

			foreach (var obj in wattrs) {
				var attr = obj as TestWarningAttribute;
				if (attr == null)
					continue;

				string message;
				if (member is MethodInfo)
					message = member.Name + ": " + attr.Message;
				else
					message = attr.Message;
				warnings.Add (new TestWarning (message));
			}
		}

		internal void Log (string message, params object[] args)
		{
			Suite.Log (message, args);
		}

		public bool Resolve ()
		{
			var list = new List<TestCase> ();

			var bf = BindingFlags.Instance | BindingFlags.Public;
			foreach (var method in Type.GetMethods (bf)) {
				if (method.GetCustomAttribute<AsyncTestSetUpAttribute> () != null) {
					if (setup != null)
						Log ("Duplicate [AsyncTestSetUp] method: {0}.{1}",
						     method.DeclaringType.FullName, method.Name);
					else if (!CheckSignatureForSetUpOrTearDown (method))
						Log ("Invalid [AsyncTestSetUp] method: {0}.{1}",
						     method.DeclaringType.FullName, method.Name);
					else
						setup = method;
				} else if (method.GetCustomAttribute<AsyncTestTearDownAttribute> () != null) {
					if (teardown != null)
						Log ("Duplicate [AsyncTestTearDown] method: {0}.{1}",
						     method.DeclaringType.FullName, method.Name);
					else if (!CheckSignatureForSetUpOrTearDown (method))
						Log ("Invalid [AsyncTestTearDown] method: {0}.{1}",
						     method.DeclaringType.FullName, method.Name);
					else
						teardown = method;
				}
			}

			foreach (var method in Type.GetMethods (bf)) {
				var attr = (AsyncTestAttribute)method.GetCustomAttribute (
					Framework.TestAttributeType, false);
				if ((attr == null) || attr.GetType ().IsSubclassOf (Framework.TestAttributeType))
					continue;
				if (!CheckSignature (method)) {
					Log ("Invalid [AsyncTest] method: {0}.{1}",
					     method.DeclaringType.FullName, method.Name);
					continue;
				}
				list.Add (new TestCase (this, attr, method));
			}

			tests = list.ToArray ();
			return true;
		}

		bool CheckSignatureForSetUpOrTearDown (MethodInfo method)
		{
			var pinfo = method.GetParameters ();
			if (pinfo.Length == 0)
				return true;
			else if (pinfo.Length > 1)
				return false;
			return pinfo [0].ParameterType.IsAssignableFrom (Framework.ContextType);
		}

		bool CheckSignature (MethodInfo method)
		{
			var pinfo = method.GetParameters ();
			if (pinfo.Length == 0)
				return true;
			else if (pinfo.Length > 2)
				return false;
			if (!pinfo [0].ParameterType.IsAssignableFrom (Framework.ContextType))
				return false;
			if (pinfo.Length == 1)
				return true;
			return pinfo [1].ParameterType.Equals (typeof (CancellationToken));
		}

		IList<ITestRunner> GetTestRunners (ITestCategory category)
		{
			var list = new List<ITestRunner> ();
			if (disabled)
				return list.AsReadOnly ();

			foreach (var runner in Framework.GetTestRunners (this)) {
				if (!category.IsEnabled (runner))
					continue;
				if (!HasCustomThreadingMode ()) {
					list.Add (runner);
					continue;
				}
				if (HasDefaultThreadingMode ())
					list.Add (new ThreadingModeProxyRunner (runner, ThreadingMode.Default));
				CheckThreadingModeProxy (list, runner, ThreadingMode.MainThread);
				CheckThreadingModeProxy (list, runner, ThreadingMode.ExitContext);
				CheckThreadingModeProxy (list, runner, ThreadingMode.ThreadPool);
			}
			return list.AsReadOnly ();
		}

		bool HasDefaultThreadingMode ()
		{
			if (Attribute.ThreadingMode != ThreadingMode.Default)
				return false;
			return tests.Any (test => test.Attribute.ThreadingMode == ThreadingMode.Default);
		}

		bool HasCustomThreadingMode ()
		{
			if (Attribute.ThreadingMode != ThreadingMode.Default)
				return true;
			return tests.Any (test => test.Attribute.ThreadingMode != ThreadingMode.Default);
		}

		void CheckThreadingModeProxy (List<ITestRunner> list, ITestRunner runner, ThreadingMode mode)
		{
			bool found = false;
			if ((Attribute.ThreadingMode & mode) != 0)
				found = true;
			else
				found = tests.Any (test => (test.Attribute.ThreadingMode & mode) != 0);
			if (found)
				list.Add (new ThreadingModeProxyRunner (runner, mode));
		}

		void AddWarnings (TestResultCollection result)
		{
			result.AddWarnings (Warnings);
			foreach (var test in tests) {
				result.AddWarnings (test.Warnings);
			}
		}

		public bool IsEnabled (ITestCategory category)
		{
			return !disabled && GetTestRunners (category).Any (
				runner => tests.Any (test => TestSuite.Filter (test, runner, category)));
		}

		public async Task<TestResultCollection> Run (ITestCategory category,
		                                             CancellationToken cancellationToken)
		{
			TestResultCollection result;

			var runners = GetTestRunners (category);
			if (runners.Count == 1) {
				result = await Run (category, runners [0], cancellationToken);
				AddWarnings (result);
				return result;
			}

			result = new TestResultCollection (Name);

			for (int i = 0; i < runners.Count; i++) {
				var runner = runners [i];
				result.AddChild (await Run (category, runner, cancellationToken));
			}

			AddWarnings (result);
			return result;
		}

		async Task<TestResultCollection> Run (ITestCategory category, ITestRunner runner,
		                                      CancellationToken cancellationToken)
		{
			string name = Name;
			if (runner.Name != null)
				name = string.Format ("{0} ({1})", Name, runner.Name);

			var result = new TestResultCollection (name);

			var selected = Tests.Where (test => TestSuite.Filter (test, runner, category));

			if (Attribute.ReuseContext) {
				await Run (runner, result, selected, cancellationToken);
				return result;
			}

			foreach (var test in selected) {
				var array = new TestCase[] { test };

				bool reuseContext = GetReuseContext (test);
				int repeat = reuseContext ? 1 : GetRepeat (test);

				for (int i = 0; i < repeat; i++)
					await Run (runner, result, array, cancellationToken);
			}

			return result;
		}

		bool GetReuseContext (TestCase test)
		{
			if (test.Attribute.ReuseContext_internal != null)
				return test.Attribute.ReuseContext_internal.Value;
			else
				return Attribute.ReuseContext;
		}

		int GetRepeat (TestCase test)
		{
			if (test.Attribute.Repeat_internal != null)
				return test.Attribute.Repeat_internal.Value;
			else
				return Attribute.Repeat;
		}

		async Task Run (ITestRunner runner, TestResultCollection result,
		                IEnumerable<TestCase> selected, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var cts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);
			if (Attribute.Timeout > 0)
				cts.CancelAfter (Attribute.Timeout);

			var context = runner.CreateContext (this);
			context.Configuration = Configuration;

			try {
				SetUp (context);
			} catch (Exception ex) {
				Log ("{0}.SetUp failed: {1}", result.Name, ex);
				result.AddChild (new TestError (result.Name, "SetUp failed", ex));
				return;
			}

			foreach (var test in selected) {
				if (cts.Token.IsCancellationRequested)
					break;

				bool reuseContext = GetReuseContext (test);
				int repeat = reuseContext ? GetRepeat (test) : 1;

				string iteration;
				if (Suite.MaxIterations == 1)
					iteration = string.Empty;
				else
					iteration = string.Format (
						" (iteration {0}/{1})", Suite.CurrentIteration+1,
						Suite.MaxIterations);

				string name = Name;
				if (runner.Name != null)
					name = string.Format (
						"{0}.{1} ({2}){3}", Name, test.Name, runner.Name, iteration);
				else
					name = string.Format (
						"{0}.{1} ({2}", Name, test.Name, iteration);

				for (int i = 0; i < repeat; i++) {
					string thisIter;
					if (repeat == 1)
						thisIter = string.Empty;
					else
						thisIter = string.Format (" (iteration {0}/{1})", i+1, repeat);

					Suite.OnStatusMessageEvent ("Running {0}{1}", name, thisIter);

					try {
						var retval = await Run (test, context, cts.Token);
						result.AddChild (retval);
					} catch (Exception ex) {
						Log ("{0} failed: {1}", test.Name, ex);
						result.AddChild (new TestError (result.Name, null, ex));
					}
				}
			}

			try {
				TearDown (context);
			} catch (Exception ex) {
				Log ("{0}.TearDown failed: {1}", result.Name, ex);
				result.AddChild (new TestError (result.Name, "TearDown failed", ex));
			}

			cts.Token.ThrowIfCancellationRequested ();
		}

		async Task<TestResult> RunTest (TestCase test, TestContext context,
		                                CancellationToken cancellationToken)
		{
			context.ClearErrors ();
			var result = await Run (test, context, cancellationToken);
			if (context.HasErrors)
				return new TestResultWithErrors (context, result);
			else
				return result;
		}

		Task<TestResult> Run (TestCase test, TestContext context,
		                      CancellationToken cancellationToken)
		{
			return test.Run (context, cancellationToken);
		}

		void SetUp (TestContext context)
		{
			if (setup != null)
				InvokeSetUpOrTearDown (setup, context);
		}

		void TearDown (TestContext context)
		{
			if (teardown != null)
				InvokeSetUpOrTearDown (setup, context);
		}

		void InvokeSetUpOrTearDown (MethodInfo method, TestContext context)
		{
			object[] args;
			var pinfo = method.GetParameters ();
			if (pinfo.Length == 0)
				args = new object[0];
			else
				args = new object[] { context };
			method.Invoke (context.Instance, args);
		}
	}
}
