// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace CoreWLANWirelessManager
{
	[Register ("JoinDialogController")]
	partial class JoinDialogController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField networkNameTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSecureTextField passphraseTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton phyModePicker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton securityModePicker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator spinner { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField usernameTextField { get; set; }

		[Action ("cancelButtonClicked:")]
		partial void cancelButtonClicked (MonoMac.Foundation.NSObject sender);

		[Action ("okButtonCkicked:")]
		partial void okButtonCkicked (MonoMac.Foundation.NSObject sender);
		
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
