// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace StillMotion
{
	[Register ("MyDocument")]
	partial class MyDocument
	{
		[Outlet]
		QTKit.QTCaptureView captureView { get; set; }

		[Outlet]
		QTKit.QTMovieView movieView { get; set; }

		[Action ("AddFrame:")]
		partial void AddFrame (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (captureView != null) {
				captureView.Dispose ();
				captureView = null;
			}

			if (movieView != null) {
				movieView.Dispose ();
				movieView = null;
			}
		}
	}
}
