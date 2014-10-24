// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Bananas
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		Bananas.SceneView scnView { get; set; }

		[Outlet]
		AppKit.NSWindow window { get; set; }

		[Action ("pause:")]
		partial void Pause (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (scnView != null) {
				scnView.Dispose ();
				scnView = null;
			}

			if (window != null) {
				window.Dispose ();
				window = null;
			}
		}
	}
}
