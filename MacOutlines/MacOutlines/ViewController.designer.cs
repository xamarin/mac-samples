// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacOutlines
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTableColumn DetailsColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn ProductColumn { get; set; }

		[Outlet]
		AppKit.NSOutlineView ProductOutline { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProductOutline != null) {
				ProductOutline.Dispose ();
				ProductOutline = null;
			}

			if (ProductColumn != null) {
				ProductColumn.Dispose ();
				ProductColumn = null;
			}

			if (DetailsColumn != null) {
				DetailsColumn.Dispose ();
				DetailsColumn = null;
			}
		}
	}
}
