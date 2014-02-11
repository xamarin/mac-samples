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
	[Register ("IBSSDialogController")]
	partial class IBSSDialogController
	{
		[Outlet]
		MonoMac.AppKit.NSPopUpButton channelPicker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField nameTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSecureTextField passwordTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator spinner { get; set; }

		[Action ("cancelButtonClicked:")]
		partial void cancelButtonClicked (MonoMac.AppKit.NSButton sender);

		[Action ("okButtonClicked:")]
		partial void okButtonClicked (MonoMac.AppKit.NSButton sender);
		
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
