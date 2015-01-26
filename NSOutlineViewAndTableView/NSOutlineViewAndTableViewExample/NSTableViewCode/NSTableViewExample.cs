using System;
using MonoMac.AppKit;
using System.Drawing;
using MonoMac.Foundation;

namespace NSOutlineViewAndTableViewExample
{
	class TableSetup
	{
		// This sets up a NSTableView for demonstration
		internal static NSView SetupTableView (RectangleF frame)
		{
			// Create our NSTableView and set it's frame to a reasonable size. It will be autosized via the NSClipView
			NSTableView tableView = new NSTableView () {
				Frame = frame
			};

			// Just like NSOutlineView, NSTableView expects at least one column
			tableView.AddColumn (new NSTableColumn ("Values"));
			tableView.AddColumn (new NSTableColumn ("Data"));

			// Setup the Delegate/DataSource instances to be interrogated for data and view information
			// In Unified, these take an interface instead of a base class and you can combine these into
			// one instance. 
			tableView.DataSource = new TableDataSource ();
			tableView.Delegate = new TableDelegate ();

			// NSTableView expects to be hosted inside an NSClipView and won't draw correctly otherwise  
			NSClipView clipView = new NSClipView (frame) {
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
			};
			clipView.DocumentView = tableView;
			return clipView;
		}
	}

	// Delegates recieve events associated with user action and determine how an item should be visualized
	class TableDelegate : NSTableViewDelegate 
	{
		const string identifer = "myCellIdentifier";
		static string [] NumberWords = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };

		// Returns the NSView for a given column/row. NSTableView is strange as unlike NSOutlineView 
		// it does not pass in the data for the given item (obtained from the DataSource) for the NSView APIs
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, int row)
		{
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTextField view = (NSTextField)tableView.MakeView (identifer, this);
			if (view == null) {
				view = new NSTextField ();
				view.Identifier = identifer;
				view.Bordered = false;
				view.Selectable = false;
				view.Editable = false;
			}
			if (tableColumn.Identifier == "Values")
				view.StringValue = (NSString)row.ToString ();
			else
				view.StringValue = (NSString)NumberWords [row];

			return view;		
		}

		// An example of responding to user input 
		public override bool ShouldSelectRow (NSTableView tableView, int row)
		{
			Console.WriteLine ("ShouldSelectRow: {0}", row);
			return true;
		}
	}

	// Data sources in general walk a given data source and respond to questions from AppKit to generate
	// the data used in your Delegate. However, as noted in GetViewForItem above, NSTableView
	// only requires the row count from the data source, instead of also requesting the data for that item
	// and passing that into the delegate.
	class TableDataSource : NSTableViewDataSource
	{
		public override int GetRowCount (NSTableView tableView)
		{
			return 10;
		}
	}
}

