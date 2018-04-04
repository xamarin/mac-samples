// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("URLViewController")]
	partial class URLViewController
	{
		[Outlet]
		AppKit.NSComboBox URLBox { get; set; }

		[Outlet]
		AppKit.NSTextField StatusLabel { get; set; }

		[Action ("Load:")]
		partial void Load (Foundation.NSObject sender);

		[Action ("Stop:")]
		partial void Stop (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (URLBox != null) {
				URLBox.Dispose ();
				URLBox = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
		}
	}

	[Register ("URLView")]
	partial class URLView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
