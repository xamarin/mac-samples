// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace Test_Hello_Mac
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.NSTextField OutputLabel { get; set; }

		[Outlet]
		AppKit.NSButton ClickMeButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (OutputLabel != null) {
				OutputLabel.Dispose ();
				OutputLabel = null;
			}

			if (ClickMeButton != null) {
				ClickMeButton.Dispose ();
				ClickMeButton = null;
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
