// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacControls
{
	[Register ("IndicatorViewController")]
	partial class IndicatorViewController
	{
		[Outlet]
		AppKit.NSProgressIndicator AsyncProgress { get; set; }

		[Outlet]
		AppKit.NSTextField FeedbackLabel { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator Indeterminate { get; set; }

		[Outlet]
		AppKit.NSLevelIndicator LevelIndicator { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator ProgressIndicator { get; set; }

		[Outlet]
		AppKit.NSLevelIndicator Rating { get; set; }

		[Outlet]
		AppKit.NSLevelIndicator Relevance { get; set; }

		[Action ("HundredPercent:")]
		partial void HundredPercent (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AsyncProgress != null) {
				AsyncProgress.Dispose ();
				AsyncProgress = null;
			}

			if (FeedbackLabel != null) {
				FeedbackLabel.Dispose ();
				FeedbackLabel = null;
			}

			if (Indeterminate != null) {
				Indeterminate.Dispose ();
				Indeterminate = null;
			}

			if (LevelIndicator != null) {
				LevelIndicator.Dispose ();
				LevelIndicator = null;
			}

			if (ProgressIndicator != null) {
				ProgressIndicator.Dispose ();
				ProgressIndicator = null;
			}

			if (Rating != null) {
				Rating.Dispose ();
				Rating = null;
			}

			if (Relevance != null) {
				Relevance.Dispose ();
				Relevance = null;
			}
		}
	}
}
