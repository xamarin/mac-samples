// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Xamarin.HeartMonitor
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton chooseDeviceButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton connectButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController deviceListController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator deviceListScanningProgressIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSWindow deviceListSheet { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField deviceNameLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView deviceTableView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton dismissDeviceListButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView heartImage { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField heartRateLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField heartRateUnitLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField statusLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (chooseDeviceButton != null) {
				chooseDeviceButton.Dispose ();
				chooseDeviceButton = null;
			}

			if (connectButton != null) {
				connectButton.Dispose ();
				connectButton = null;
			}

			if (deviceListController != null) {
				deviceListController.Dispose ();
				deviceListController = null;
			}

			if (deviceListScanningProgressIndicator != null) {
				deviceListScanningProgressIndicator.Dispose ();
				deviceListScanningProgressIndicator = null;
			}

			if (deviceListSheet != null) {
				deviceListSheet.Dispose ();
				deviceListSheet = null;
			}

			if (deviceTableView != null) {
				deviceTableView.Dispose ();
				deviceTableView = null;
			}

			if (dismissDeviceListButton != null) {
				dismissDeviceListButton.Dispose ();
				dismissDeviceListButton = null;
			}

			if (heartImage != null) {
				heartImage.Dispose ();
				heartImage = null;
			}

			if (heartRateLabel != null) {
				heartRateLabel.Dispose ();
				heartRateLabel = null;
			}

			if (heartRateUnitLabel != null) {
				heartRateUnitLabel.Dispose ();
				heartRateUnitLabel = null;
			}

			if (statusLabel != null) {
				statusLabel.Dispose ();
				statusLabel = null;
			}

			if (deviceNameLabel != null) {
				deviceNameLabel.Dispose ();
				deviceNameLabel = null;
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
