// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacToolbar
{
	[Register ("WindowController")]
	partial class WindowController
	{
		[Outlet]
		MacToolbar.ActivatableItem trashItem { get; set; }

		[Action ("trashDocument:")]
		partial void trashDocument (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (trashItem != null) {
				trashItem.Dispose ();
				trashItem = null;
			}
		}
	}
}
