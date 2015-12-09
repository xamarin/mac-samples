// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacControls
{
	[Register ("SplitViewController")]
	partial class SplitViewController
	{
		[Outlet]
		AppKit.NSSplitViewItem LeftController { get; set; }

		[Outlet]
		AppKit.NSSplitViewItem RightController { get; set; }

		[Outlet]
		AppKit.NSSplitView SplitView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SplitView != null) {
				SplitView.Dispose ();
				SplitView = null;
			}

			if (LeftController != null) {
				LeftController.Dispose ();
				LeftController = null;
			}

			if (RightController != null) {
				RightController.Dispose ();
				RightController = null;
			}
		}
	}
}
