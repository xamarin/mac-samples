using System;
using MonoMac.Foundation;
namespace CoreWLANWirelessManager
{
	// Copyright Ashok Gelal (http://ashokgelal.com)
	
	partial class MainWindowController
	{
		private class ScanResultsTableDataSource : MonoMac.AppKit.NSTableViewDataSource
		{
			private MainWindowController MyController { get; set;}
			public ScanResultsTableDataSource (MainWindowController controller)
			{
				MyController = controller;
			}
			
			public override int GetRowCount (MonoMac.AppKit.NSTableView tableView)
			{
				return MyController == null ? 0 : MyController.ScanResults.Length;
			}
			
			public override MonoMac.Foundation.NSObject GetObjectValue (MonoMac.AppKit.NSTableView tableView, MonoMac.AppKit.NSTableColumn tableColumn, int row)
			{
				var valueKey = (NSString)tableColumn.Identifier.ToString ();
				var dataRow = MyController.ScanResults[row];
				
				switch (valueKey)
				{
					case "NETWORK_NAME":
						return (NSString)dataRow.Ssid;
					case "BSSID":
						return (NSString)dataRow.Bssid;
					case "CHANNEL":
						return (NSString)dataRow.Channel.StringValue;
					case "PHY_MODE":
						return (NSString)dataRow.PhyMode.StringValue;
					case "NETWORK_MODE":
						return (NSString)(dataRow.IsIBSS?"Yes":"No");
					case "RSSI":
						return (NSString)dataRow.Rssi.StringValue;
					case "SECURITY_MODE":
						return (NSString)dataRow.SecurityMode.StringValue;
				}
				throw new Exception(string.Format("Incorrect value requested '{0}'", valueKey));
			}
		}
	}
}
