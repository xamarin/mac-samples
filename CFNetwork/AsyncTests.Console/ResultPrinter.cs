using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace AsyncTests.ConsoleRunner {

	using Framework;

	public class ResultPrinter : ResultVisitor {
		TextWriter writer;
		Stack<string> names;
		int id;

		public ResultPrinter (TextWriter writer)
		{
			this.writer = writer;
			names = new Stack<string> ();
		}

		public static void Print (TextWriter writer, TestResultCollection result)
		{
			writer.WriteLine ();
			writer.WriteLine ("Total: {0} tests, {1} passed, {2} errors.",
			                  result.Count, result.TotalSuccess, result.TotalErrors);
			writer.WriteLine ();

			var printer = new ResultPrinter (writer);
			printer.Visit (result);
		}

		string GetName ()
		{
			return string.Join (".", names.ToArray ());
		}

		#region implemented abstract members of ResultVisitor
		public override void Visit (TestResultCollection node)
		{
			for (int i = 0; i < node.Count; i++) {
				var item = node [i];
				names.Push (item.Name);
				item.Accept (this);
				names.Pop ();
			}
		}

		public override void Visit (TestResultText node)
		{
			;
		}

		public override void Visit (TestSuccess node)
		{
			;
		}

		public override void Visit (TestError node)
		{
			writer.WriteLine ("{0}) {1}\n{2}\n", ++id, GetName (), node.Error);
		}

		public override void Visit (TestResultWithErrors node)
		{
			for (int i = 0; i < node.Count; i++) {
				var item = node [i];
				names.Push (item.Name);
				item.Accept (this);
				names.Pop ();
			}
		}
		#endregion

	}
}

