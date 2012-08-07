//
// AsyncTests.Framework.TestCase
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
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncTests.Framework {

	public sealed class TestCase {
		public TestFixture Fixture {
			get;
			private set;
		}

		public AsyncTestAttribute Attribute {
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

		public MethodInfo Method {
			get;
			private set;
		}

		public string Name {
			get;
			private set;
		}

		public bool IsEnabled {
			get { return !disabled; }
		}

		TestConfiguration config;
		bool disabled;

		public TestCase (TestFixture fixture, AsyncTestAttribute attr, MethodInfo method)
		{
			this.Fixture = fixture;
			this.Attribute = attr;
			this.Method = method;
			this.Name = method.Name;

			IDictionary<Type, TestCategoryAttribute> categories;
			IList<TestWarning> warnings;

			TestFixture.Resolve (
				fixture.Suite, fixture, method, out categories,
				out warnings, out config, out disabled);

			Categories = categories.Values.ToList ().AsReadOnly ();
			Warnings = warnings;

		}

		internal void Log (string message, params object[] args)
		{
			Fixture.Log (message, args);
		}

		public async Task<TestResult> Run (TestContext context, CancellationToken cancellationToken)
		{
			var cts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);
			if (Attribute.Timeout > 0)
				cts.CancelAfter (Attribute.Timeout);

			var oldConfig = context.Configuration;

			try {
				context.Configuration = Configuration;

				var attr = Method.GetCustomAttribute<ExpectedExceptionAttribute> ();
				TestResult result;
				if (attr != null)
					result = await ExpectingException (context, attr.ExceptionType, cts.Token);
				else
					result = await ExpectingSuccess (context, cts.Token);
				if (!context.HasWarnings)
					return result;
				var collection = new TestResultCollection (result.Name);
				collection.AddChild (result);
				collection.AddWarnings (context.Warnings);
				return collection;
			} catch (Exception ex) {
				Log ("Test {0} failed: {1}", Name, ex);
				return new TestError (Name, null, ex);
			} finally {
				context.Configuration = oldConfig;
			}
		}

		async Task<TestResult> ExpectingSuccess (TestContext context,
		                                         CancellationToken cancellationToken)
		{
			await context.Invoke (this, cancellationToken);
			return new TestSuccess (Name);
		}

		async Task<TestResult> ExpectingException (TestContext context, Type type,
		                                           CancellationToken cancellationToken)
		{
			try {
				await context.Invoke (this, cancellationToken);
				var message = string.Format ("Expected an exception of type {0}", type);
				return new TestError (Name, message, new AssertionException (message));
			} catch (Exception ex) {
				if (ex is TargetInvocationException)
					ex = ((TargetInvocationException)ex).InnerException;
				if (type.IsAssignableFrom (ex.GetType ()))
					return new TestSuccess (Name);
				var message = string.Format ("Expected an exception of type {0}, but got {1}",
				                             type, ex.GetType ());
				return new TestError (Name, message, new AssertionException (message, ex));
			}
		}
	}
}
