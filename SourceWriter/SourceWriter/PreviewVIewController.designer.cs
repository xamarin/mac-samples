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
	[Register ("PreviewVIewController")]
	partial class PreviewVIewController
	{
		[Outlet]
		AppKit.NSTextField StatusBar { get; set; }

		[Outlet]
		WebKit.WebView WebPreview { get; set; }

		[Outlet]
		AppKit.NSTextField ZoomLevel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WebPreview != null) {
				WebPreview.Dispose ();
				WebPreview = null;
			}

			if (ZoomLevel != null) {
				ZoomLevel.Dispose ();
				ZoomLevel = null;
			}

			if (StatusBar != null) {
				StatusBar.Dispose ();
				StatusBar = null;
			}
		}
	}
}
