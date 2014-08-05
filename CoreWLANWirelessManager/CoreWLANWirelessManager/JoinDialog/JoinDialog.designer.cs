// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CoreWLANWirelessManager
{
	[Register ("JoinDialogController")]
	partial class JoinDialogController
	{
		[Outlet]
		AppKit.NSTextField networkNameTextField { get; set; }

		[Outlet]
		AppKit.NSSecureTextField passphraseTextField { get; set; }

		[Outlet]
		AppKit.NSPopUpButton phyModePicker { get; set; }

		[Outlet]
		AppKit.NSPopUpButton securityModePicker { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator spinner { get; set; }

		[Outlet]
		AppKit.NSTextField usernameTextField { get; set; }

		[Action ("cancelButtonClicked:")]
		partial void cancelButtonClicked (Foundation.NSObject sender);

		[Action ("okButtonCkicked:")]
		partial void okButtonCkicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (networkNameTextField != null) {
				networkNameTextField.Dispose ();
				networkNameTextField = null;
			}

			if (usernameTextField != null) {
				usernameTextField.Dispose ();
				usernameTextField = null;
			}

			if (passphraseTextField != null) {
				passphraseTextField.Dispose ();
				passphraseTextField = null;
			}

			if (spinner != null) {
				spinner.Dispose ();
				spinner = null;
			}

			if (securityModePicker != null) {
				securityModePicker.Dispose ();
				securityModePicker = null;
			}

			if (phyModePicker != null) {
				phyModePicker.Dispose ();
				phyModePicker = null;
			}
		}
	}

	[Register ("JoinDialog")]
	partial class JoinDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
