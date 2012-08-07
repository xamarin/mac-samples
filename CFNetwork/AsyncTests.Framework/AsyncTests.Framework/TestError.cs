//
// AsyncTests.Framework.TestError
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
using System.Xml;

namespace AsyncTests.Framework {

	public class TestError : TestResult {
		public Exception Error {
			get;
			private set;
		}

		List<TestResultText> items;
		string fullText;

		public TestError (string name, string message, Exception error)
			: base (name)
		{
			this.Error = error;

			items = new List<TestResultText> ();
			if (message != null)
				fullText = message + ": " + error.ToString ();
			else
				fullText = error.ToString ();

			foreach (var line in fullText.Split ('\n'))
				items.Add (new TestResultText (line));
		}

		#region implemented abstract members of TestResult

		public override TestStatus Status {
			get { return TestStatus.Error; }
		}

		#endregion

		#region implemented abstract members of TestResultItem

		public override bool HasChildren {
			get { return Count > 0; }
		}

		public override int Count {
			get { return items.Count; }
		}

		public override TestResultItem this [int index] {
			get { return items [index]; }
		}

		public override void Accept (ResultVisitor visitor)
		{
			visitor.Visit (this);
		}

		#endregion

		#region IXmlSerializable implementation

		public override void WriteXml (XmlWriter writer)
		{
			base.WriteXml (writer);
			writer.WriteAttributeString ("Exception", Error.ToString ());
		}

		#endregion

	}
}

