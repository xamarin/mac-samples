using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;
using SQLite;

namespace MacDatabase
{
	public class TableORMDelegate : NSTableViewDelegate
	{
		#region Constants 
		private const string CellIdentifier = "OccCell";
		#endregion

		#region Private Variables
		private TableORMDatasource DataSource;
		#endregion

		#region Constructors
		public TableORMDelegate (TableORMDatasource dataSource)
		{
			// Initialize
			this.DataSource = dataSource;
		}
		#endregion

		#region Override Methods
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTextField view = (NSTextField)tableView.MakeView (CellIdentifier, this);
			if (view == null) {
				view = new NSTextField ();
				view.Identifier = CellIdentifier;
				view.BackgroundColor = NSColor.Clear;
				view.Bordered = false;
				view.Selectable = false;
				view.Editable = false;
			}

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Occupation":
				view.StringValue = DataSource.Occupations [(int)row].Name;
				break;
			case "Description":
				view.StringValue = DataSource.Occupations [(int)row].Description;
				break;
			}

			return view;
		}
		#endregion
	}
}

