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
	[Register ("MenuViewController")]
	partial class MenuViewController
	{
		[Outlet]
		AppKit.NSMenuItem DropDownSelected { get; set; }

		[Outlet]
		AppKit.NSTextField FeedbackLabel { get; set; }

		[Action ("ItemOne:")]
		partial void ItemOne (Foundation.NSObject sender);

		[Action ("ItemThree:")]
		partial void ItemThree (Foundation.NSObject sender);

		[Action ("ItemTwo:")]
		partial void ItemTwo (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DropDownSelected != null) {
				DropDownSelected.Dispose ();
				DropDownSelected = null;
			}

			if (FeedbackLabel != null) {
				FeedbackLabel.Dispose ();
				FeedbackLabel = null;
			}
		}
	}
}
