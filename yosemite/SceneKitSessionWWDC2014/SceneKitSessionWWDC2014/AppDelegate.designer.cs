// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SceneKitSessionWWDC2014
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenu GoMenu { get; set; }

		[Outlet]
		AppKit.NSWindow MainWindow { get; set; }

		[Action ("NextSlide:")]
		partial void NextSlide (Foundation.NSObject sender);

		[Action ("PreviousSlide:")]
		partial void PreviousSlide (Foundation.NSObject sender);

		[Action ("ToogleCursor:")]
		partial void ToogleCursor (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (GoMenu != null) {
				GoMenu.Dispose ();
				GoMenu = null;
			}

			if (MainWindow != null) {
				MainWindow.Dispose ();
				MainWindow = null;
			}
		}
	}
}
