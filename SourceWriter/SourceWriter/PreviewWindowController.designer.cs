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
	[Register ("PreviewWindowController")]
	partial class PreviewWindowController
	{
		[Outlet]
		AppKit.ManualToolbarItem ToolbarPrint { get; set; }

		[Action ("ErasePreview:")]
		partial void ErasePreview (Foundation.NSObject sender);

		[Action ("OriginalSize:")]
		partial void OriginalSize (Foundation.NSObject sender);

		[Action ("RefreshPreview:")]
		partial void RefreshPreview (Foundation.NSObject sender);

		[Action ("ScrollToBottom:")]
		partial void ScrollToBottom (Foundation.NSObject sender);

		[Action ("ScrollToTop:")]
		partial void ScrollToTop (Foundation.NSObject sender);

		[Action ("ZoomIn:")]
		partial void ZoomIn (Foundation.NSObject sender);

		[Action ("ZoomOut:")]
		partial void ZoomOut (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ToolbarPrint != null) {
				ToolbarPrint.Dispose ();
				ToolbarPrint = null;
			}
		}
	}
}
