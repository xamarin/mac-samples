// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacControls
{
	[Register ("SubviewContentViews")]
	partial class SubviewContentViews
	{
		[Outlet]
		AppKit.NSPopover Popover { get; set; }

		[Action ("DisplayPopover:")]
		partial void DisplayPopover (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Popover != null) {
				Popover.Dispose ();
				Popover = null;
			}
		}
	}
}
