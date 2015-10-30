using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

namespace AzureTodo
{
	/// <summary>
	/// Data Source - exposes what data goes in each row/column (cell)
	/// </summary>
	public class TableDataSource : NSTableViewDataSource
	{
		#region Private Variables
		readonly List<TodoItem> tasks;
		#endregion

		#region Constructors
		public TableDataSource (List<TodoItem> tasks)
		{
			// Initialize
			this.tasks = tasks;
		}
		#endregion

		#region Override Methods
		public override nint GetRowCount (NSTableView tableView)
		{
			// Return the number of tasks
			return tasks.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			// Return an empty object if the row is outside of the range
			if (row < 0)
				return new NSObject ();

			// Get the current task
			var todo = tasks[(int)row];

			// Return the value based on the column
			if (tableColumn.Identifier == "name")
				return new NSString (todo.Name);
			else if (tableColumn.Identifier == "complete")
				return new NSString (todo.Done ? "Done" : "Incomplete");
			else if (tableColumn.Identifier == "id")
				return new NSString (todo.ID); // not really a column

			// Return an empty string for an unknown column name
			return new NSString (string.Empty);
		}
		#endregion
	}
}

