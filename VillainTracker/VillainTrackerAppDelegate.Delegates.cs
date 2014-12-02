using System;
using System.Collections.Generic;
using Foundation;
using AppKit;

namespace VillainTracker
{
	partial class VillainTrackerAppDelegate
	{
		private class DataSource : AppKit.NSTableViewDataSource
		{
			private VillainTrackerAppDelegate _app;

			public DataSource (VillainTrackerAppDelegate app)
			{
				_app = app;
			}

			public override nint GetRowCount (NSTableView tableView)
			{
				return _app == null ? 0 : _app.villains.Count;
			}

			public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint rowIndex)
			{
				var valueKey = (string)(NSString)tableColumn.Identifier;
				var dataRow = _app.villains [(int)rowIndex];
				
				switch ((string)valueKey) {
				case "name":
					return (NSString)dataRow.Name;
				case "mugshot":
					return dataRow.Mugshot;
				case "lastSeenDate":
					return (NSDate)dataRow.LastSeenDate;
				}
			
				throw new Exception (string.Format ("Incorrect value requested '{0}'", (string)valueKey));
			}

			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, nint rowIndex)
			{
				var valueKey = (string)(NSString)tableColumn.Identifier;
				var dataRow = _app.villains [(int)rowIndex];
	
				switch ((string)valueKey) {
				case "name":
					dataRow.Name = (string)(NSString)theObject;
					break;
				case "mugshot":
					dataRow.Mugshot = (NSImage)theObject;
					break;
				case "lastSeenDate":
					dataRow.LastSeenDate = (DateTime)(NSDate)theObject;
					break;
				}
		
				_app.UpdateDetailViews ();
			}
		}

		private class VillainsTableViewDelegate : AppKit.NSTableViewDelegate
		{
			private VillainTrackerAppDelegate _app;

			public VillainsTableViewDelegate (VillainTrackerAppDelegate app)
			{
				_app = app;
			}

			public override void SelectionDidChange (NSNotification notification)
			{
				if (_app != null && _app.villainsTableView.SelectedRow >= 0) {
					_app.villain = _app.villains [(int)_app.villainsTableView.SelectedRow];
					_app.UpdateDetailViews ();
				}
			}
		}
	}
}

