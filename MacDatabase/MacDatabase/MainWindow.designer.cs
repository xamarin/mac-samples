// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacDatabase
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		[Outlet]
		AppKit.ActivatableItem AddButton { get; set; }

		[Outlet]
		AppKit.ActivatableItem DeleteButton { get; set; }

		[Outlet]
		AppKit.ActivatableItem EditButton { get; set; }

		[Outlet]
		AppKit.NSSearchField Search { get; set; }

		[Outlet]
		AppKit.SourceListView SourceList { get; set; }

		[Outlet]
		AppKit.NSView ViewContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}

			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}

			if (Search != null) {
				Search.Dispose ();
				Search = null;
			}

			if (DeleteButton != null) {
				DeleteButton.Dispose ();
				DeleteButton = null;
			}

			if (SourceList != null) {
				SourceList.Dispose ();
				SourceList = null;
			}

			if (ViewContainer != null) {
				ViewContainer.Dispose ();
				ViewContainer = null;
			}
		}
	}
}
