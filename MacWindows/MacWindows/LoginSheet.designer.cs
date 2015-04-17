// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacWindows
{
	[Register ("LoginSheet")]
	partial class LoginSheet
	{
		[Outlet]
		AppKit.NSTextField loginPassword { get; set; }

		[Outlet]
		AppKit.NSTextField loginUser { get; set; }

		[Outlet]
		MacWindows.LoginSheet window { get; set; }

		[Action ("loginCancel:")]
		partial void loginCancel (Foundation.NSObject sender);

		[Action ("loginOK:")]
		partial void loginOK (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (window != null) {
				window.Dispose ();
				window = null;
			}

			if (loginUser != null) {
				loginUser.Dispose ();
				loginUser = null;
			}

			if (loginPassword != null) {
				loginPassword.Dispose ();
				loginPassword = null;
			}
		}
	}
}
