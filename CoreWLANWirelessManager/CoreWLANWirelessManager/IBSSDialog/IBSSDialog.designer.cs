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
	[Register ("IBSSDialogController")]
	partial class IBSSDialogController
	{
		[Outlet]
		AppKit.NSPopUpButton channelPicker { get; set; }

		[Outlet]
		AppKit.NSTextField nameTextField { get; set; }

		[Outlet]
		AppKit.NSSecureTextField passwordTextField { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator spinner { get; set; }

		[Action ("cancelButtonClicked:")]
		partial void cancelButtonClicked (AppKit.NSButton sender);

		[Action ("okButtonClicked:")]
		partial void okButtonClicked (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (nameTextField != null) {
				nameTextField.Dispose ();
				nameTextField = null;
			}

			if (passwordTextField != null) {
				passwordTextField.Dispose ();
				passwordTextField = null;
			}

			if (channelPicker != null) {
				channelPicker.Dispose ();
				channelPicker = null;
			}

			if (spinner != null) {
				spinner.Dispose ();
				spinner = null;
			}
		}
	}

	[Register ("IBSSDialog")]
	partial class IBSSDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
