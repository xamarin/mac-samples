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
	[Register ("CustomDialog")]
	partial class CustomDialog
	{
		[Outlet]
		AppKit.NSTextField dialogMessage { get; set; }

		[Outlet]
		AppKit.NSTextField dialogTitle { get; set; }

		[Action ("dialogCancel:")]
		partial void dialogCancel (Foundation.NSObject sender);

		[Action ("dialogClose:")]
		partial void dialogClose (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (dialogTitle != null) {
				dialogTitle.Dispose ();
				dialogTitle = null;
			}

			if (dialogMessage != null) {
				dialogMessage.Dispose ();
				dialogMessage = null;
			}
		}
	}
}
