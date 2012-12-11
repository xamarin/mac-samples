//
// AsyncTests.Framework.TestCategoryAttribute
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
using Mono.Addins;

namespace AsyncTests.Framework {

	[AttributeUsage (AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public abstract class TestCategoryAttribute : Attribute, ITestCategory {
		public virtual string Name {
			get {
				var name = GetType ().Name;
				if (name.EndsWith ("Attribute"))
					name = name.Substring (0, name.Length - 9);
				return name;
			}
		}

		/*
		 * Must explicitly select this category to run it.
		 */
		public virtual bool Explicit {
			get { return true; }
		}

		public virtual bool IsEnabled (ITestRunner runner)
		{
			return true;
		}

		public bool IsEnabled (TestCase test)
		{
			return test.Categories.Contains (this);
		}
	}
}
