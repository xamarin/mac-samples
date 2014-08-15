// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace DockAppIcon
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSButton badgeCheck { get; set; }

		[Outlet]
		AppKit.NSTextField badgeNumber { get; set; }

		[Outlet]
		AppKit.NSTextField customFormat { get; set; }

		[Outlet]
		AppKit.NSButton formatCheck { get; set; }

		[Outlet]
		AppKit.NSPopUpButton popupRequestType { get; set; }

		[Outlet]
		AppKit.NSButton showAppBadgeCheck { get; set; }

		[Outlet]
		AppKit.NSButton requestButton { get; set; }

		[Outlet]
		AppKit.NSStepper stepper { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (badgeCheck != null) {
				badgeCheck.Dispose ();
				badgeCheck = null;
			}

			if (badgeNumber != null) {
				badgeNumber.Dispose ();
				badgeNumber = null;
			}

			if (customFormat != null) {
				customFormat.Dispose ();
				customFormat = null;
			}

			if (formatCheck != null) {
				formatCheck.Dispose ();
				formatCheck = null;
			}

			if (popupRequestType != null) {
				popupRequestType.Dispose ();
				popupRequestType = null;
			}

			if (showAppBadgeCheck != null) {
				showAppBadgeCheck.Dispose ();
				showAppBadgeCheck = null;
			}

			if (requestButton != null) {
				requestButton.Dispose ();
				requestButton = null;
			}

			if (stepper != null) {
				stepper.Dispose ();
				stepper = null;
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
