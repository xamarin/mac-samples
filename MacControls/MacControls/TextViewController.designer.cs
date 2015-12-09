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
	[Register ("TextViewController")]
	partial class TextViewController
	{
		[Outlet]
		AppKit.NSTextField FeedbackLabel { get; set; }

		[Outlet]
		AppKit.NSTextField NumberField { get; set; }

		[Outlet]
		AppKit.NSSecureTextField PasswordField { get; set; }

		[Outlet]
		AppKit.NSTextView TextEditor { get; set; }

		[Outlet]
		AppKit.NSTextField UserField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FeedbackLabel != null) {
				FeedbackLabel.Dispose ();
				FeedbackLabel = null;
			}

			if (NumberField != null) {
				NumberField.Dispose ();
				NumberField = null;
			}

			if (PasswordField != null) {
				PasswordField.Dispose ();
				PasswordField = null;
			}

			if (TextEditor != null) {
				TextEditor.Dispose ();
				TextEditor = null;
			}

			if (UserField != null) {
				UserField.Dispose ();
				UserField = null;
			}
		}
	}
}
