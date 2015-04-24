// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacImages
{
	[Register ("SubviewTable")]
	partial class SubviewTable
	{
		[Outlet]
		AppKit.NSTableView ProductTable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ProductTable != null) {
				ProductTable.Dispose ();
				ProductTable = null;
			}
		}
	}
}
