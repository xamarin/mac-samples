// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace UserNotificationExample
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		MonoMac.AppKit.NSColorWell DeliveredColorWell { get; set; }

		[Outlet]
		MonoMac.AppKit.NSColorWell TouchedColorWell { get; set; }

		[Action ("NotifyMeAction:")]
		partial void NotifyMeAction (MonoMac.AppKit.NSButton sender);

		[Action ("ResetAction:")]
		partial void ResetAction (MonoMac.AppKit.NSButton sender);
		
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
