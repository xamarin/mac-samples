// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacCollectionNew
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MacCollectionNew.ManualToolbarItem AddToolbarItem { get; set; }

		[Outlet]
		MacCollectionNew.ManualToolbarItem DeleteToolbarItem { get; set; }

		[Outlet]
		MacCollectionNew.ManualToolbarItem EditToolbarItem { get; set; }

		[Outlet]
		MacCollectionNew.ManualToolbarItem InfoToolbarItem { get; set; }

		[Action ("AddEmployee:")]
		partial void AddEmployee (Foundation.NSObject sender);

		[Action ("DeleteEmployee:")]
		partial void DeleteEmployee (Foundation.NSObject sender);

		[Action ("EditEmployee:")]
		partial void EditEmployee (Foundation.NSObject sender);

		[Action ("ShowInfo:")]
		partial void ShowInfo (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AddToolbarItem != null) {
				AddToolbarItem.Dispose ();
				AddToolbarItem = null;
			}

			if (DeleteToolbarItem != null) {
				DeleteToolbarItem.Dispose ();
				DeleteToolbarItem = null;
			}

			if (EditToolbarItem != null) {
				EditToolbarItem.Dispose ();
				EditToolbarItem = null;
			}

			if (InfoToolbarItem != null) {
				InfoToolbarItem.Dispose ();
				InfoToolbarItem = null;
			}
		}
	}
}
