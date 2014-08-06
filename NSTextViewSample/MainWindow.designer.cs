// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace NSTextViewSample
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSToolbarItem insertCatToolbarItem { get; set; }

		[Outlet]
		AppKit.NSToolbarItem insertCustomCatToolbarItem { get; set; }

		[Outlet]
		AppKit.NSTextView textView { get; set; }

		[Outlet]
		AppKit.NSToolbar toolbar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (insertCatToolbarItem != null) {
				insertCatToolbarItem.Dispose ();
				insertCatToolbarItem = null;
			}

			if (insertCustomCatToolbarItem != null) {
				insertCustomCatToolbarItem.Dispose ();
				insertCustomCatToolbarItem = null;
			}

			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
