//
// AsyncTests.Framework.TestContext
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace AsyncTests.Framework {
	public abstract class TestContext : IDisposable {
		List<TestError> errors;
		List<TestWarning> warnings;
		List<IDisposable> disposables;
		int countAssertions;

		public TestFixture Fixture {
			get;
			private set;
		}

		internal object Instance {
			get;
			private set;
		}

		public ThreadingMode ThreadingMode {
			get;
			internal set;
		}

		public TestConfiguration Configuration {
			get;
			internal set;
		}

		protected TestContext (TestFixture fixture, object instance)
		{
			this.Fixture = fixture;
			this.Instance = instance;
			ThreadingMode = ThreadingMode.Default;
		}

		public virtual void Log (string message, params object[] args)
		{
			Fixture.Log (message, args);
		}

		public T GetConfiguration<T> ()
			where T : TestConfiguration
		{
			var message = string.Format ("GetConfiguration({0})", typeof (T).FullName);
			Assert (Configuration, Is.Not.Null, "GetConfiguration({0})", message);
			Assert (Configuration, Is.InstanceOfType (typeof (T)), message);
			return (T)Configuration;
		}

		internal void ClearErrors ()
		{
			errors = null;
			warnings = null;
			countAssertions = 0;
		}

		internal void AddError (string name, Exception error)
		{
			if (errors == null)
				errors = new List<TestError> ();
			errors.Add (new TestError (name, null, error));
		}

		public bool HasErrors {
			get { return errors != null; }
		}

		internal IList<TestError> Errors {
			get {
				return HasErrors ? errors.AsReadOnly () : null;
			}
		}

		public bool HasWarnings {
			get { return warnings != null; }
		}

		public IList<TestWarning> Warnings {
			get {
				return HasWarnings ? warnings.AsReadOnly () : null;
			}
		}

		internal void CheckErrors (string message)
		{
			if (errors == null)
				return;
			throw new TestErrorException (message, errors.ToArray ());
		}

		protected internal Task Invoke (TestCase test, CancellationToken cancellationToken)
		{
			object[] args;
			if (test.Method.GetParameters ().Length == 1)
				args = new object[] { this };
			else
				args = new object[] { this, cancellationToken };
			return Invoke_internal (test.Name, test.Method, Instance, args);
		}

		protected internal async Task Invoke_internal (string name, MethodInfo method,
		                                               object instance, object[] args)
		{
			ClearErrors ();

			try {
				var retval = method.Invoke (instance, args);

				var task = retval as Task;
				if (task != null)
					await task;
			} finally {
				AutoDispose ();
			}

			CheckErrors (name);
		}

		#region Assertions

		/*
		 * By default, Exepct() is non-fatal.  Multiple failed expections will be
		 * collected and a TestErrorException will be thrown when the test method
		 * returns.
		 * 
		 * Use Assert() to immediately abort the test method or set 'AlwaysFatal = true'.
		 * 
		 */

		public bool AlwaysFatal {
			get;
			set;
		}

		public bool Expect (object actual, Constraint constraint)
		{
			return Expect (false, actual, constraint, null, null);
		}

		public bool Expect (object actual, Constraint constraint, string message)
		{
			return Expect (false, actual, constraint, message, null);
		}

		public bool Expect (object actual, Constraint constraint,
		                    string message, params object[] args)
		{
			return Expect (false, actual, constraint, message, args);
		}

		public bool Expect (bool fatal, object actual, Constraint constraint,
		                    string message, params object[] args)
		{
			if (constraint.Matches (actual)) {
				++countAssertions;
				return true;
			}
			using (var writer = new TextMessageWriter (message, args)) {
				constraint.WriteMessageTo (writer);
				var error = new AssertionException (writer.ToString ());
				string text = string.Empty;;
				if ((message != null) && (message != string.Empty)) {
					if (args != null)
						text = string.Format (message, args);
					else
						text = message;
				}
				AddError (text, error);
				if (AlwaysFatal || fatal)
					throw error;
				return false;
			}
		}

		public void Assert (object actual, Constraint constraint)
		{
			Expect (true, actual, constraint, null, null);
		}

		public void Assert (object actual, Constraint constraint, string message)
		{
			Expect (true, actual, constraint, message, null);
		}

		public void Assert (object actual, Constraint constraint,
		                    string message, params object[] args)
		{
			Expect (true, actual, constraint, message, args);
		}

		public bool Expect (bool condition, string message, params object[] args)
		{
			return Expect (false, condition, Is.True, message, args);
		}

		public bool Expect (bool condition, string message)
		{
			return Expect (false, condition, Is.True, message, null);
		}

		public bool Expect (bool condition)
		{
			return Expect (false, condition, Is.True, null, null);
		}

		public void Assert (bool condition, string message, params object[] args)
		{
			Expect (true, condition, Is.True, message, args);
		}

		public void Assert (bool condition, string message)
		{
			Expect (true, condition, Is.True, message, null);
		}

		public void Assert (bool condition)
		{
			Expect (true, condition, Is.True, null, null);
		}

		public void Warning (string message, params object[] args)
		{
			Warning (string.Format (message, args));
		}

		public void Warning (string message)
		{
			if (warnings == null)
				warnings = new List<TestWarning> ();
			warnings.Add (new TestWarning (message));
		}

		#endregion

		#region Disposing

		public void AutoDispose (IDisposable disposable)
		{
			if (disposable == null)
				return;
			if (disposables == null)
				disposables = new List<IDisposable> ();
			disposables.Add (disposable);
		}

		void AutoDispose ()
		{
			if (disposables == null)
				return;
			foreach (var disposable in disposables) {
				try {
					disposable.Dispose ();
				} catch (Exception ex) {
					AddError ("Auto-dispose failed", ex);
				}
			}
			disposables = null;
		}

		~TestContext ()
		{
			Dispose (false);
		}
		
		public void Dispose ()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose (bool disposing)
		{
		}

		#endregion
	}
}
