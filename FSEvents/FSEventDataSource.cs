using System;
using System.Collections.Generic;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreServices;

namespace FSEventWatcher
{
	[Register ("FSEventDataSource")]
	public class FSEventDataSource : NSTableViewDataSource
	{
		struct Row
		{
			public int Group;
			public FSEvent Event;
		}

		int groupId;
		List<Row> rows = new List<Row> ();

		public void Add (IEnumerable<FSEvent> events)
		{
			var id = groupId++;

			foreach (var evnt in events) {
				rows.Add (new Row {
					Group = id,
					Event = evnt
				});
			}
		}

		public override int GetRowCount (NSTableView tableView)
		{
			return rows.Count;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, int rowIndex)
		{
			var row = rows[rowIndex];

			switch (tableColumn.DataCell.Tag) {
			case 0:
				return new NSString (row.Group.ToString ());
			case 1:
				return new NSString (row.Event.Id.ToString ());
			case 2:
				return new NSString (row.Event.Flags.ToString ());
			case 3:
				return new NSString (row.Event.Path);
			}

			return null;
		}
	}
}