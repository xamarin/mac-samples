// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace VisualEffectPlayground
{
	[Register ("BasicSidebarViewController")]
	partial class BasicSidebarViewController
	{
		[Outlet]
		AppKit.NSImageView SideImageView { get; set; }

		[Outlet]
		AppKit.NSImageView TopImageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SideImageView != null) {
				SideImageView.Dispose ();
				SideImageView = null;
			}

			if (TopImageView != null) {
				TopImageView.Dispose ();
				TopImageView = null;
			}
		}
	}
}
