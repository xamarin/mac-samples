//
// AsyncTests.Framework.ThreadingModeProxyRunner
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

namespace AsyncTests.Framework
{
	class ThreadingModeProxyRunner : ProxyTestRunner
	{
		public ThreadingMode Mode {
			get;
			private set;
		}

		public ThreadingModeProxyRunner (ITestRunner inner, ThreadingMode mode)
			: base (inner)
		{
			this.Mode = mode;
		}

		#region implemented abstract members of AsyncTests.Framework.ProxyTestRunner
		protected override string ProxyName {
			get { return Mode.ToString (); }
		}

		protected override bool IsProxyEnabled (TestCase test)
		{
			if (Mode == ThreadingMode.Default) {
				if (test.Fixture.Attribute.ThreadingMode != ThreadingMode.Default)
					return false;
				return test.Attribute.ThreadingMode == ThreadingMode.Default;
			}

			// Explicitly requested
			if ((test.Attribute.ThreadingMode & Mode) != 0)
				return true;
			// Fixture requests a special mode.
			if (test.Fixture.Attribute.ThreadingMode != ThreadingMode.Default) {
				if ((test.Fixture.Attribute.ThreadingMode & Mode) == 0)
					return false;
				return test.Attribute.ThreadingMode == ThreadingMode.Default;
			} else {
				return false;
			}
		}

		protected override void ModifyContext (TestContext context)
		{
			context.ThreadingMode = Mode;
		}
		#endregion
	}
}
