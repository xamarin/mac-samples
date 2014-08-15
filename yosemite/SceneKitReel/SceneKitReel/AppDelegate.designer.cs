// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SceneKitReel
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSWindow Window { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Window != null) {
				Window.Dispose ();
				Window = null;
			}
		}
	}
}
