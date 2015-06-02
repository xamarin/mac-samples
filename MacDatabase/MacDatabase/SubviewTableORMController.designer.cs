// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacDatabase
{
	[Register ("SubviewTableORMController")]
	partial class SubviewTableORMController
	{
		[Outlet]
		AppKit.NSTableColumn DescriptionColumn { get; set; }

		[Outlet]
		AppKit.NSTableColumn OccupationColumn { get; set; }

		[Outlet]
		AppKit.NSTableView OccupationTable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (OccupationTable != null) {
				OccupationTable.Dispose ();
				OccupationTable = null;
			}

			if (OccupationColumn != null) {
				OccupationColumn.Dispose ();
				OccupationColumn = null;
			}

			if (DescriptionColumn != null) {
				DescriptionColumn.Dispose ();
				DescriptionColumn = null;
			}
		}
	}
}
