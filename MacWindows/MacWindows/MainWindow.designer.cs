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
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.NSTextView documentEditor { get; set; }

		[Action ("resizeWindow:")]
		partial void resizeWindow (Foundation.NSObject sender);

		[Action ("showAlert:")]
		partial void showAlert (Foundation.NSObject sender);

		[Action ("showDialog:")]
		partial void showDialog (Foundation.NSObject sender);

		[Action ("showLayout:")]
		partial void showLayout (Foundation.NSObject sender);

		[Action ("showLogin:")]
		partial void showLogin (Foundation.NSObject sender);

		[Action ("showPrinter:")]
		partial void showPrinter (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (documentEditor != null) {
				documentEditor.Dispose ();
				documentEditor = null;
			}
		}
	}
}
