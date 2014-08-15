using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreWlan;

namespace CoreWLANWirelessManager
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		private CWNetwork[] networks;

		private CWInterface CurrentInterface { get; set; }

		private string SupportedChannels
		{
			get
			{
				if (CurrentInterface == null)
					return string.Empty;
				else
					return String.Join (", ", CurrentInterface.SupportedWlanChannels.Select (channel => 
						channel.ChannelNumber.ToString ()).ToArray<string>());
			}
		}

		public NetworksDataSource NetworksDataSource
		{
			get
			{
				if (CurrentInterface == null)
					return new NetworksDataSource (new CWNetwork[0]);

				NSError error;
				var scannedNetworks = CurrentInterface.ScanForNetworksWithName (null, out error);

				if (error != null) {
					Console.WriteLine ("Error occurred while scanning netowrks: {0}", error.LocalizedDescription);
					return new NetworksDataSource (networks);
				} else {
					var networksForDataSource = new List<CWNetwork> ();

					if (mergeScanResults.State == NSCellStateValue.On) {
						networksForDataSource = scannedNetworks.ToList<CWNetwork>();
					} else {
						networksForDataSource = scannedNetworks.Concat (networks).ToList<CWNetwork>();
					}

					var dataSource = new NetworksDataSource (networksForDataSource);
					networks = networksForDataSource.ToArray<CWNetwork>();
					return dataSource;
				}
			}
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			createIBSSButton.Activated += CreateIBSS;

			joinButton.Enabled = false;
			joinButton.Activated += JoinNetwrok;
			networksTable.Activated += (sender, e) => joinButton.Enabled = networksTable.SelectedRow >= 0;

			networks = new CWNetwork[0];
			refreshSpinner.Hidden = true;
			interfacesPicker.AddItems (CWInterface.InterfaceNames);
			CurrentInterface = new CWInterface (interfacesPicker.SelectedItem.Title);
			tabView.DidSelect += (sender, e) => UpdateInfo();
			UpdateInfo ();
		}

		partial void powerStateChanged (AppKit.NSSegmentedControl sender)
		{
			NSError error;
			CurrentInterface.SetPower(powerState.SelectedSegment == 0, out error);

			if(error != null)
				Console.WriteLine("Error occurred while changing interface power state: {0}", error.LocalizedDescription);
		}

		private void JoinNetwrok (object sender, EventArgs e)
		{
			var selectedRow = networksTable.SelectedRow;
			if (selectedRow >= 0) {
				var joinDialog = new JoinDialogController (networks [selectedRow], CurrentInterface);
				joinDialog.ShowWindow(this);
			}
		}

		private void CreateIBSS (object sender, EventArgs e)
		{
			new IBSSDialogController (CurrentInterface).ShowWindow (this);
		}

		partial void disconnectButtonClicked (AppKit.NSButton sender)
		{
			CurrentInterface.Disassociate();
			UpdateInfoTab();
		}

		partial void interfaceSelected (AppKit.NSPopUpButton sender)
		{
			CurrentInterface = new CWInterface(interfacesPicker.SelectedItem.Title);
			UpdateInfoTab();
		}

		partial void changeChannel (AppKit.NSPopUpButton sender)
		{
			var previousChannel = CurrentInterface.WlanChannel;
			var selectedChannel = CurrentInterface.SupportedWlanChannels.Where(channel => 
				string.Format("{0} {1}", channel.ChannelNumber, channel.ChannelBand) == channelPicker.SelectedItem.Title).First();

			NSError error;
			CurrentInterface.SetWlanChannel(selectedChannel, out error);

			if(error != null) {
				Console.WriteLine("Error occurred while changing interface channel: {0}", error.LocalizedDescription);
				CurrentInterface.SetWlanChannel(previousChannel, out error);
				channelPicker.SelectItem (string.Format("{0} {1}", previousChannel.ChannelNumber, previousChannel.ChannelBand));
			}
		}

		partial void refreshButtonClicked (AppKit.NSButton sender)
		{
			UpdateInfo();
		}

		private void UpdateInfo()
		{
			refreshSpinner.Hidden = false;
			refreshSpinner.StartAnimation(this);

			if(tabView.Selected.Label == "Scan")
				UpdateScanTab();
			else
				UpdateInfoTab();

			refreshSpinner.StopAnimation(this);
			refreshSpinner.Hidden = true;
		}

		private void UpdateInfoTab ()
		{
			powerState.SetSelected (CurrentInterface.PowerOn, CurrentInterface.PowerOn ? 0 : 1);
			disconnectButton.Enabled = CurrentInterface.ServiceActive;

			if (CurrentInterface.PowerOn) {
				securityTextField.StringValue = CurrentInterface.Security.ToString();
				phyModeTextField.StringValue = CurrentInterface.ActivePHYMode.ToString();
				noiseTextField.StringValue = string.Format("{0} dBm", CurrentInterface.NoiseMeasurement.ToString ());
				ssidTextField.StringValue = CurrentInterface.Ssid ?? string.Empty;
				bssidTextField.StringValue = CurrentInterface.Bssid ?? string.Empty;
				rssiTextField.StringValue = string.Format("{0} dBm", CurrentInterface.RssiValue.ToString ());
				transmissionRateTextField.StringValue = string.Format("{0} Mbps", CurrentInterface.TransmitRate.ToString ());
				transmissionPowerTextField.StringValue = string.Format("{0} mW", CurrentInterface.TransmitPower.ToString());
				countryCodeTextField.StringValue = CurrentInterface.CountryCode;
				supportedChannelsTextField.StringValue = SupportedChannels;

				channelPicker.AddItems (CurrentInterface.SupportedWlanChannels.Select(channel => 
					string.Format("{0} {1}", channel.ChannelNumber, channel.ChannelBand)).ToArray<string>());

				channelPicker.SelectItem (string.Format("{0} {1}",
					CurrentInterface.WlanChannel.ChannelNumber, CurrentInterface.WlanChannel.ChannelBand));
			}
		}

		private void UpdateScanTab()
		{
			if (networksTable != null) {
				networksTable.DataSource = NetworksDataSource;
				networksTable.ReloadData ();
			}
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}
}

