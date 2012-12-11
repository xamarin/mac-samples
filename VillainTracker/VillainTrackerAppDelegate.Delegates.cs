using System;
using System.Collections.Generic;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace VillainTracker
{
	partial class VillainTrackerAppDelegate
	{
		private class DataSource : MonoMac.AppKit.NSTableViewDataSource
		{
			private VillainTrackerAppDelegate _app;
			public DataSource(VillainTrackerAppDelegate app)
			{
				_app = app;
			}
			
			public override int GetRowCount (MonoMac.AppKit.NSTableView tableView)
			{
				return _app == null ? 0 : _app.villains.Count;
			}
			
			public override MonoMac.Foundation.NSObject GetObjectValue (MonoMac.AppKit.NSTableView tableView, MonoMac.AppKit.NSTableColumn tableColumn, int rowIndex)
			{
				var valueKey = (string)(NSString)tableColumn.Identifier;
				var dataRow = _app.villains[rowIndex];
				
				switch((string)valueKey)
				{
				case "name":
					return (NSString)dataRow.Name;
				case "mugshot":
					return dataRow.Mugshot;
				case "lastSeenDate":
					return (NSDate)dataRow.LastSeenDate;
				}
			
				throw new Exception(string.Format("Incorrect value requested '{0}'", (string)valueKey));
			}
			
			public override void SetObjectValue (NSTableView tableView, NSObject theObject, NSTableColumn tableColumn, int rowIndex)
			{
				var valueKey = (string)(NSString)tableColumn.Identifier;
				var dataRow = _app.villains[rowIndex];
	
				switch((string)valueKey)
				{
				case "name":
					dataRow.Name = (string)(NSString)theObject;
					break;
				case "mugshot":
					dataRow.Mugshot = (NSImage)theObject;
					break;
				case "lastSeenDate":
					dataRow.LastSeenDate = (NSDate)theObject;
					break;
				}
		
				_app.UpdateDetailViews();
			}
		}
		
		private class VillainsTableViewDelegate : MonoMac.AppKit.NSTableViewDelegate
		{
			private VillainTrackerAppDelegate _app;
			public VillainsTableViewDelegate(VillainTrackerAppDelegate app)
			{
				_app = app;
			}
			
			public override void SelectionDidChange (NSNotification notification)
			{
				if(_app != null && _app.villainsTableView.SelectedRow >= 0)
				{
					_app.villain = _app.villains[_app.villainsTableView.SelectedRow];
					_app.UpdateDetailViews();
				}
			}
		}
	}
}

