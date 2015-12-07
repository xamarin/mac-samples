// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacDialog
{
	[Register ("SheetViewController")]
	partial class SheetViewController
	{
		[Outlet]
		AppKit.NSTextField NameField { get; set; }

		[Outlet]
		AppKit.NSTextField PasswordField { get; set; }

		[Action ("AcceptSheet:")]
		partial void AcceptSheet (Foundation.NSObject sender);

		[Action ("CancelSheet:")]
		partial void CancelSheet (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (NameField != null) {
				NameField.Dispose ();
				NameField = null;
			}

			if (PasswordField != null) {
				PasswordField.Dispose ();
				PasswordField = null;
			}
		}
	}
}
