// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacCopyPaste
{
	[Register ("AppDelegate")]
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenu EditMenu { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (EditMenu != null) {
				EditMenu.Dispose ();
				EditMenu = null;
			}
		}
	}
}
