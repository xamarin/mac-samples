// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacInspector
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSToolbarItem FormatItem { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl FormatSegment { get; set; }

		[Action ("FormatSelected:")]
		partial void FormatSelected (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (FormatItem != null) {
				FormatItem.Dispose ();
				FormatItem = null;
			}

			if (FormatSegment != null) {
				FormatSegment.Dispose ();
				FormatSegment = null;
			}
		}
	}
}
