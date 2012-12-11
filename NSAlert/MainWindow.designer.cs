// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace NSAlertSample
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField ResultLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMatrix AlertOptions { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField ModalCounter { get; set; }

		[Action ("NSAlertWithMessage:")]
		partial void NSAlertWithMessage (MonoMac.Foundation.NSObject sender);

		[Action ("NSAlertWithError:")]
		partial void NSAlertWithError (MonoMac.Foundation.NSObject sender);

		[Action ("CustomButtons:")]
		partial void CustomButtons (MonoMac.Foundation.NSObject sender);

		[Action ("CustomImage:")]
		partial void CustomImage (MonoMac.Foundation.NSObject sender);

		[Action ("DefaultSuppression:")]
		partial void DefaultSuppression (MonoMac.Foundation.NSObject sender);

		[Action ("CustomSuppression:")]
		partial void CustomSuppression (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ResultLabel != null) {
				ResultLabel.Dispose ();
				ResultLabel = null;
			}

			if (AlertOptions != null) {
				AlertOptions.Dispose ();
				AlertOptions = null;
			}

			if (ModalCounter != null) {
				ModalCounter.Dispose ();
				ModalCounter = null;
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
