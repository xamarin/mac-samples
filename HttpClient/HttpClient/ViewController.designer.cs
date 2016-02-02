// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace HttpClient
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton TheButton { get; set; }

		[Outlet]
		AppKit.NSTextView TheLog { get; set; }

		[Outlet]
		AppKit.NSTableView TheTable { get; set; }

		[Action ("OnPress:")]
		partial void OnPress (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (TheButton != null) {
				TheButton.Dispose ();
				TheButton = null;
			}

			if (TheTable != null) {
				TheTable.Dispose ();
				TheTable = null;
			}

			if (TheLog != null) {
				TheLog.Dispose ();
				TheLog = null;
			}
		}
	}
}
