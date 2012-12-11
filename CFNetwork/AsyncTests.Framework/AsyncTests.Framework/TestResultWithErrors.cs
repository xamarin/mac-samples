//
// AsyncTests.Framework.TestResultWithErrors
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

namespace AsyncTests.Framework {

	public class TestResultWithErrors : TestResult {
		TestResult result;
		List<TestResultItem> children;

		internal TestResultWithErrors (TestContext context, TestResult result)
		{
			this.result = result;
			children = new List<TestResultItem> ();
			children.Add (result);
			children.AddRange (context.Errors);
			context.ClearErrors ();
		}

		#region implemented abstract members of TestResult

		public override TestStatus Status {
			get { return result.Status; }
		}

		#endregion

		#region implemented abstract members of TestResultItem

		public override bool HasChildren {
			get { return true; }
		}

		public override int Count {
			get { return children.Count; }
		}

		public override TestResultItem this [int index] {
			get { return children [index]; }
		}

		public override void Accept (ResultVisitor visitor)
		{
			visitor.Visit (this);
		}

		#endregion
	}
}
