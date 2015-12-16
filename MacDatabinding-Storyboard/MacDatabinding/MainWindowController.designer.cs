// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacDatabinding
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.ActivatableItem Addbutton { get; set; }

		[Outlet]
		AppKit.ActivatableItem DeleteButton { get; set; }

		[Outlet]
		AppKit.ActivatableItem EditButton { get; set; }

		[Outlet]
		AppKit.NSSearchField Search { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Addbutton != null) {
				Addbutton.Dispose ();
				Addbutton = null;
			}

			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}

			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}

			if (Search != null) {
				Search.Dispose ();
				Search = null;
			}
		}
	}
}
