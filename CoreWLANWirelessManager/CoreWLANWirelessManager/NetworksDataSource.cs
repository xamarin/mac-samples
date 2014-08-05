using System;
using AppKit;
using System.Collections.Generic;
using CoreWlan;
using Foundation;
using System.Linq;

namespace CoreWLANWirelessManager
{
	public class NetworksDataSource : NSTableViewDataSource
	{
		public CWNetwork[] Networks { get; private set; }

		public NetworksDataSource (IEnumerable<CWNetwork> scanResults)
		{
			Networks = scanResults.Cast<CWNetwork> ().ToArray ();
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return Networks.Length;
		}

		public override NSObject GetObjectValue (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var valueKey = (NSString)tableColumn.Identifier.ToString ();
			CWNetwork selectedNetwork = Networks [row];
			switch (valueKey) {
			case "NETWORK_NAME":
				return (NSString)selectedNetwork.Ssid;
			case "BSSID":
				return (NSString)selectedNetwork.Bssid;
			case "CHANNEL":
				return (NSString)selectedNetwork.WlanChannel.ChannelNumber.ToString ();
			case "PHY_MODE":
				return (NSString)selectedNetwork.GetSupportedPHYMode ().GetStringInterpretation ();
			case "NETWORK_MODE":
				return (NSString)(selectedNetwork.Ibss ? "Yes" : "No");
			case "RSSI":
				return (NSString)selectedNetwork.RssiValue.ToString ();
			case "SECURITY_MODE":
				return (NSString)selectedNetwork.GetSecurityMode ().GetStringInterpretation ();
			}

			throw new Exception (string.Format ("Incorrect value requested '{0}'", valueKey));
		}
	}
}

