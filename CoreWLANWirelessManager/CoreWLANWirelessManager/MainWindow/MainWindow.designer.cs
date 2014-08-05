// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CoreWLANWirelessManager
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSTextField bssidTextField { get; set; }

		[Outlet]
		AppKit.NSPopUpButton channelPicker { get; set; }

		[Outlet]
		AppKit.NSTextField countryCodeTextField { get; set; }

		[Outlet]
		AppKit.NSButton createIBSSButton { get; set; }

		[Outlet]
		AppKit.NSButton disconnectButton { get; set; }

		[Outlet]
		AppKit.NSPopUpButton interfacesPicker { get; set; }

		[Outlet]
		AppKit.NSButton joinButton { get; set; }

		[Outlet]
		AppKit.NSButton mergeScanResults { get; set; }

		[Outlet]
		AppKit.NSTableView networksTable { get; set; }

		[Outlet]
		AppKit.NSTextField noiseTextField { get; set; }

		[Outlet]
		AppKit.NSTextField phyModeTextField { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl powerState { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator refreshSpinner { get; set; }

		[Outlet]
		AppKit.NSTextField rssiTextField { get; set; }

		[Outlet]
		AppKit.NSTextField securityTextField { get; set; }

		[Outlet]
		AppKit.NSTextField ssidTextField { get; set; }

		[Outlet]
		AppKit.NSTextField supportedChannelsTextField { get; set; }

		[Outlet]
		AppKit.NSTabView tabView { get; set; }

		[Outlet]
		AppKit.NSTextField transmissionPowerTextField { get; set; }

		[Outlet]
		AppKit.NSTextField transmissionRateTextField { get; set; }

		[Action ("changeChannel:")]
		partial void changeChannel (AppKit.NSPopUpButton sender);

		[Action ("disconnectButtonClicked:")]
		partial void disconnectButtonClicked (AppKit.NSButton sender);

		[Action ("interfaceSelected:")]
		partial void interfaceSelected (AppKit.NSPopUpButton sender);

		[Action ("powerStateChanged:")]
		partial void powerStateChanged (AppKit.NSSegmentedControl sender);

		[Action ("refreshButtonClicked:")]
		partial void refreshButtonClicked (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (bssidTextField != null) {
				bssidTextField.Dispose ();
				bssidTextField = null;
			}

			if (channelPicker != null) {
				channelPicker.Dispose ();
				channelPicker = null;
			}

			if (countryCodeTextField != null) {
				countryCodeTextField.Dispose ();
				countryCodeTextField = null;
			}

			if (disconnectButton != null) {
				disconnectButton.Dispose ();
				disconnectButton = null;
			}

			if (interfacesPicker != null) {
				interfacesPicker.Dispose ();
				interfacesPicker = null;
			}

			if (noiseTextField != null) {
				noiseTextField.Dispose ();
				noiseTextField = null;
			}

			if (phyModeTextField != null) {
				phyModeTextField.Dispose ();
				phyModeTextField = null;
			}

			if (powerState != null) {
				powerState.Dispose ();
				powerState = null;
			}

			if (refreshSpinner != null) {
				refreshSpinner.Dispose ();
				refreshSpinner = null;
			}

			if (rssiTextField != null) {
				rssiTextField.Dispose ();
				rssiTextField = null;
			}

			if (securityTextField != null) {
				securityTextField.Dispose ();
				securityTextField = null;
			}

			if (ssidTextField != null) {
				ssidTextField.Dispose ();
				ssidTextField = null;
			}

			if (supportedChannelsTextField != null) {
				supportedChannelsTextField.Dispose ();
				supportedChannelsTextField = null;
			}

			if (tabView != null) {
				tabView.Dispose ();
				tabView = null;
			}

			if (transmissionPowerTextField != null) {
				transmissionPowerTextField.Dispose ();
				transmissionPowerTextField = null;
			}

			if (transmissionRateTextField != null) {
				transmissionRateTextField.Dispose ();
				transmissionRateTextField = null;
			}

			if (networksTable != null) {
				networksTable.Dispose ();
				networksTable = null;
			}

			if (mergeScanResults != null) {
				mergeScanResults.Dispose ();
				mergeScanResults = null;
			}

			if (createIBSSButton != null) {
				createIBSSButton.Dispose ();
				createIBSSButton = null;
			}

			if (joinButton != null) {
				joinButton.Dispose ();
				joinButton = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
