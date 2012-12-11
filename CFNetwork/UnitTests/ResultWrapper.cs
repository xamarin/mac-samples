//
// MonoMac.CFNetwork.Test.UnitTests.ResultWrapper
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
using System.Collections.Generic;
using MonoMac.Foundation;
using AsyncTests.Framework;

namespace MonoMac.CFNetwork.Test.UnitTests {

	[Register ("ResultWrapper")]
	internal class ResultWrapper : NSObject {
		List<ResultWrapper> children;

		public TestResultItem Item {
			get;
			private set;
		}

		public int Count {
			get { return children != null ? children.Count : 0; }
		}

		public bool HasChildren {
			get { return children != null; }
		}

		public ResultWrapper GetChild (int index)
		{
			return children [index];
		}

		public ResultWrapper (TestResultItem item)
		{
			this.Item = item;

			if (!item.HasChildren)
				return;

			children = new List<ResultWrapper> ();
			for (int i = 0; i < item.Count; i++)
				children.Add (new ResultWrapper (item [i]));
		}
	}
}

