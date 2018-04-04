// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("GetStringViewController")]
	partial class GetStringViewController
	{
		[Outlet]
		AppKit.NSButton AutoRedirect { get; set; }

		[Outlet]
		AppKit.NSBox Content { get; set; }

		[Outlet]
		AppKit.NSTextField Status { get; set; }

		[Action ("DisplayModeChanged:")]
		partial void DisplayModeChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AutoRedirect != null) {
				AutoRedirect.Dispose ();
				AutoRedirect = null;
			}

			if (Content != null) {
				Content.Dispose ();
				Content = null;
			}

			if (Status != null) {
				Status.Dispose ();
				Status = null;
			}
		}
	}

	[Register ("GetStringView")]
	partial class GetStringView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
