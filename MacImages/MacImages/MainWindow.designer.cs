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
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.SourceListView SourceList { get; set; }

		[Outlet]
		AppKit.NSView ViewContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SourceList != null) {
				SourceList.Dispose ();
				SourceList = null;
			}

			if (ViewContainer != null) {
				ViewContainer.Dispose ();
				ViewContainer = null;
			}
		}
	}
}
