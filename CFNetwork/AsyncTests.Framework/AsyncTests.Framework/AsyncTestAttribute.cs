//
// AsyncTests.Framework.AsyncTestAttribute
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

	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class AsyncTestAttribute : Attribute {
		int timeout = -1;
		int? repeat;
		bool? reuseContext;
		ThreadingMode mode = ThreadingMode.Default;

		public int Timeout {
			get { return timeout; }
			set { timeout = value; }
		}

		/*
		 * Whether to reuse the same TestContext for all test cases
		 * in this test suite.
		 */
		public bool ReuseContext {
			get { return reuseContext ?? true; }
			set { reuseContext = value; }
		}

		internal bool? ReuseContext_internal {
			get { return reuseContext; }
		}

		public int Repeat {
			get { return repeat ?? 1; }
			set { repeat = value; }
		}

		internal int? Repeat_internal {
			get { return repeat; }
		}

		public ThreadingMode ThreadingMode {
			get { return mode; }
			set { mode = value; }
		}
	}
}
