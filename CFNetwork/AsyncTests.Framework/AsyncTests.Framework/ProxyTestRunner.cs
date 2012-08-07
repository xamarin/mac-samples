//
// AsyncTests.Framework.ProxyTestRunner
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

namespace AsyncTests.Framework {

	abstract class ProxyTestRunner : ITestRunner {
		ITestRunner inner;

		public ProxyTestRunner (ITestRunner inner)
		{
			this.inner = inner;
		}

		protected abstract string ProxyName {
			get;
		}

		protected abstract bool IsProxyEnabled (TestCase test);

		protected abstract void ModifyContext (TestContext context);

		#region ITestRunner implementation
		public TestContext CreateContext (TestFixture fixture)
		{
			var context = inner.CreateContext (fixture);
			ModifyContext (context);
			return context;
		}

		public bool IsEnabled (TestCase test)
		{
			return inner.IsEnabled (test) && IsProxyEnabled (test);
		}

		public string Name {
			get {
				if (ProxyName != null)
					return inner.Name + ":" + ProxyName;
				else
					return inner.Name;
			}
		}
		#endregion
	}
}
