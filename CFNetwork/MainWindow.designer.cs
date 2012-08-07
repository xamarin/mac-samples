// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MonoMac.CFNetwork.Test
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField StatusLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView View { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView URLView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}

			if (View != null) {
				View.Dispose ();
				View = null;
			}

			if (URLView != null) {
				URLView.Dispose ();
				URLView = null;
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
