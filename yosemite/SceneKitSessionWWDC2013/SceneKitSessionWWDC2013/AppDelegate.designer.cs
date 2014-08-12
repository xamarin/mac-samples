// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace SceneKitSessionWWDC2013
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		MonoMac.AppKit.NSMenu GoMenu { get; set; }

		[Outlet]
		MonoMac.AppKit.NSWindow MainWindow { get; set; }

		[Action ("NextSlide:")]
		partial void NextSlide (MonoMac.Foundation.NSObject sender);

		[Action ("PreviousSlide:")]
		partial void PreviousSlide (MonoMac.Foundation.NSObject sender);

		[Action ("ToogleCursor:")]
		partial void ToogleCursor (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (MainWindow != null) {
				MainWindow.Dispose ();
				MainWindow = null;
			}

			if (GoMenu != null) {
				GoMenu.Dispose ();
				GoMenu = null;
			}
		}
	}
}
