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
	[Register ("PreferencesWindow")]
	partial class PreferencesWindow
	{
		[Outlet]
		AppKit.NSToolbarItem globalItem { get; set; }

		[Outlet]
		AppKit.NSToolbar mainToolbar { get; set; }

		[Outlet]
		AppKit.NSView panelContainer { get; set; }

		[Action ("preferencesGlobal:")]
		partial void preferencesGlobal (Foundation.NSObject sender);

		[Action ("preferencesKeyboard:")]
		partial void preferencesKeyboard (Foundation.NSObject sender);

		[Action ("preferencesProfile:")]
		partial void preferencesProfile (Foundation.NSObject sender);

		[Action ("preferencesVIOP:")]
		partial void preferencesVIOP (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (globalItem != null) {
				globalItem.Dispose ();
				globalItem = null;
			}

			if (mainToolbar != null) {
				mainToolbar.Dispose ();
				mainToolbar = null;
			}

			if (panelContainer != null) {
				panelContainer.Dispose ();
				panelContainer = null;
			}
		}
	}
}
