// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("CheckHeadersViewController")]
	partial class CheckHeadersViewController
	{
		[Outlet]
		MonoMac.AppKit.NSTableView HeaderTable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeaderTable != null) {
				HeaderTable.Dispose ();
				HeaderTable = null;
			}
		}
	}

	[Register ("CheckHeadersView")]
	partial class CheckHeadersView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
