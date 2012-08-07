//
// AsyncTests.Framework.TestResultCollection
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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AsyncTests.Framework {

	public class TestResultCollection : TestResult, IXmlSerializable {
		List<TestResult> children = new List<TestResult> ();

		public void AddChild (TestResult child)
		{
			children.Add (child);
		}

		public void AddWarnings (IList<TestWarning> warnings)
		{
			children.AddRange (warnings);
		}

		public override TestStatus Status {
			get {
				return TotalErrors == 0 ? TestStatus.Success : TestStatus.Error;
			}
		}

		public override bool HasChildren {
			get { return children.Count > 0; }
		}

		public override int Count {
			get { return children.Count; }
		}

		public override int TotalSuccess {
			get { return children.Sum (child => child.TotalSuccess); }
		}

		public override int TotalWarnings {
			get { return children.Sum (child => child.TotalWarnings); }
		}

		public override int TotalErrors {
			get { return children.Sum (child => child.TotalErrors); }
		}

		public override TestResultItem this [int index] {
			get { return children [index]; }
		}

		public TestResultCollection (string name)
			: base (name)
		{
		}

		public TestResultCollection ()
			: base ("TestResults")
		{
		}

		public override void Accept (ResultVisitor visitor)
		{
			visitor.Visit (this);
		}

		#region IXmlSerializable implementation

		public override void WriteXml (XmlWriter writer)
		{
			base.WriteXml (writer);
			writer.WriteAttributeString ("Status", Status.ToString ());
			writer.WriteAttributeString ("TotalSuccess", TotalSuccess.ToString ());
			writer.WriteAttributeString ("TotalErrors", TotalErrors.ToString ());

			foreach (var child in children) {
				writer.WriteStartElement (child.GetType ().Name);
				child.WriteXml (writer);
				writer.WriteEndElement ();
			}
		}

		#endregion
	}
}
