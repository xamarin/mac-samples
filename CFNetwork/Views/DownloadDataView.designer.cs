// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("DownloadDataViewController")]
	partial class DownloadDataViewController
	{
		[Outlet]
		MonoMac.AppKit.NSView HeaderView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeaderView != null) {
				HeaderView.Dispose ();
				HeaderView = null;
			}
		}
	}

	[Register ("DownloadDataView")]
	partial class DownloadDataView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
