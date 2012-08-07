// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("LogViewerController")]
	partial class LogViewerController
	{
		[Outlet]
		MonoMac.AppKit.NSTextView Text { get; set; }

		[Action ("Clear:")]
		partial void Clear (MonoMac.Foundation.NSObject sender);

		[Action ("Quit:")]
		partial void Quit (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Text != null) {
				Text.Dispose ();
				Text = null;
			}
		}
	}

	[Register ("LogViewer")]
	partial class LogViewer
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
