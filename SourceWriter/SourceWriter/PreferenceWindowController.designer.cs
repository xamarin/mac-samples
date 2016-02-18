// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SourceWriter
{
	[Register ("PreferenceWindowController")]
	partial class PreferenceWindowController
	{
		[Outlet]
		AppKit.NSToolbarItem FormatsItem { get; set; }

		[Outlet]
		AppKit.NSToolbarItem GeneralItem { get; set; }

		[Outlet]
		AppKit.NSToolbar Toolbar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Toolbar != null) {
				Toolbar.Dispose ();
				Toolbar = null;
			}

			if (GeneralItem != null) {
				GeneralItem.Dispose ();
				GeneralItem = null;
			}

			if (FormatsItem != null) {
				FormatsItem.Dispose ();
				FormatsItem = null;
			}
		}
	}
}
