// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MouseTrackingExample
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSTextField TheLabel { get; set; }

		[Outlet]
		MouseTrackingExample.MyTrackingView TrackingView { get; set; }

		[Outlet]
		AppKit.NSWindow window { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TrackingView != null) {
				TrackingView.Dispose ();
				TrackingView = null;
			}

			if (TheLabel != null) {
				TheLabel.Dispose ();
				TheLabel = null;
			}

			if (window != null) {
				window.Dispose ();
				window = null;
			}
		}
	}
}
