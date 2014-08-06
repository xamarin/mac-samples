// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace SCNetworkReachability
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSImageView HostIcon { get; set; }

		[Outlet]
		AppKit.NSImageView NetworkIcon { get; set; }

		[Outlet]
		AppKit.NSTextField HostTextField { get; set; }

		[Outlet]
		AppKit.NSTextField HostStatusTextField { get; set; }

		[Outlet]
		AppKit.NSTextField NetworkStatusTextField { get; set; }

		[Outlet]
		AppKit.NSPopUpButton WirelessInterfaceButton { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl WirelessInterfaceToggleButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HostIcon != null) {
				HostIcon.Dispose ();
				HostIcon = null;
			}

			if (NetworkIcon != null) {
				NetworkIcon.Dispose ();
				NetworkIcon = null;
			}

			if (HostTextField != null) {
				HostTextField.Dispose ();
				HostTextField = null;
			}

			if (HostStatusTextField != null) {
				HostStatusTextField.Dispose ();
				HostStatusTextField = null;
			}

			if (NetworkStatusTextField != null) {
				NetworkStatusTextField.Dispose ();
				NetworkStatusTextField = null;
			}

			if (WirelessInterfaceButton != null) {
				WirelessInterfaceButton.Dispose ();
				WirelessInterfaceButton = null;
			}

			if (WirelessInterfaceToggleButton != null) {
				WirelessInterfaceToggleButton.Dispose ();
				WirelessInterfaceToggleButton = null;
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
