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
	[Register ("PersonEditSheet")]
	partial class PersonEditSheet
	{
		[Outlet]
		AppKit.NSButton CancelButton { get; set; }

		[Outlet]
		MacDatabase.PersonEditSheet window { get; set; }

		[Action ("CancelAction:")]
		partial void CancelAction (Foundation.NSObject sender);

		[Action ("OkAction:")]
		partial void OkAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (CancelButton != null) {
				CancelButton.Dispose ();
				CancelButton = null;
			}
		}
	}
}
