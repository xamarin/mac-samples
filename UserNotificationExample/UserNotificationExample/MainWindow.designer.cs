// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace UserNotificationExample
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.NSColorWell DeliveredColorWell { get; set; }

		[Outlet]
		AppKit.NSColorWell TouchedColorWell { get; set; }

		[Action ("NotifyMeAction:")]
		partial void NotifyMeAction (AppKit.NSButton sender);

		[Action ("ResetAction:")]
		partial void ResetAction (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DeliveredColorWell != null) {
				DeliveredColorWell.Dispose ();
				DeliveredColorWell = null;
			}

			if (TouchedColorWell != null) {
				TouchedColorWell.Dispose ();
				TouchedColorWell = null;
			}
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
