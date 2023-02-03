// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CameraBrowserSample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTableView CameraContentTableView { get; set; }

		[Outlet]
		AppKit.NSArrayController CamerasController { get; set; }

		[Outlet]
		AppKit.NSTableView CamerasTableView { get; set; }

		[Outlet]
		AppKit.NSArrayController MediaFilesController { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CameraContentTableView != null) {
				CameraContentTableView.Dispose ();
				CameraContentTableView = null;
			}

			if (CamerasController != null) {
				CamerasController.Dispose ();
				CamerasController = null;
			}

			if (CamerasTableView != null) {
				CamerasTableView.Dispose ();
				CamerasTableView = null;
			}

			if (MediaFilesController != null) {
				MediaFilesController.Dispose ();
				MediaFilesController = null;
			}
		}
	}
}
