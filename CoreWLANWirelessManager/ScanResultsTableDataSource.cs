using System;
using MonoMac.Foundation;
using MonoMac.CoreWlan;
using System.Linq;


namespace CoreWLANWirelessManager
{
	// Copyright Ashok Gelal (http://ashokgelal.com)
	
	partial class MainWindowController
	{
		private class ScanResultsTableDataSource : MonoMac.AppKit.NSTableViewDataSource
		{
			private MainWindowController MyController { get; set; }
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
				CWNetwork dataRow = MyController.ScanResults[row];
				switch (valueKey)
				{
					case "NETWORK_NAME":
						return (NSString)dataRow.Ssid;
					case "BSSID":
						return (NSString)dataRow.Bssid;
					case "CHANNEL":
						return (NSString)dataRow.WlanChannel.ChannelNumber.ToString ();
					case "PHY_MODE":
						return (NSString)CWHelpers.StringForPhyMode (CWHelpers.PHYModeFromSSID (MyController.ScanResults, dataRow.Ssid));
					case "NETWORK_MODE":
						return (NSString)(dataRow.Ibss ? "Yes" : "No");
					case "RSSI":
						return (NSString)dataRow.RssiValue.ToString ();
					case "SECURITY_MODE":
						return (NSString)CWHelpers.StringForSecurityMode (CWHelpers.SecurityModeFromSSID (MyController.ScanResults, dataRow.Ssid));
				}

				throw new Exception(string.Format("Incorrect value requested '{0}'", valueKey));
			}
		}
	}
}
