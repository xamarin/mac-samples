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
	[Register ("SelectionViewController")]
	partial class SelectionViewController
	{
		[Outlet]
		AppKit.NSTextField AmountField { get; set; }

		[Outlet]
		AppKit.NSStepper AmountStepper { get; set; }

		[Outlet]
		AppKit.NSColorWell ColorWell { get; set; }

		[Outlet]
		AppKit.NSDatePicker DateTime { get; set; }

		[Outlet]
		AppKit.NSTextField FeedbackLabel { get; set; }

		[Outlet]
		AppKit.NSImageView ImageWell { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl SegmentButtons { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl SegmentSelection { get; set; }

		[Outlet]
		AppKit.NSSlider SliderValue { get; set; }

		[Outlet]
		AppKit.NSSliderCell TickedSlider { get; set; }

		[Action ("SegmentButtonPressed:")]
		partial void SegmentButtonPressed (Foundation.NSObject sender);

		[Action ("SegmentSelected:")]
		partial void SegmentSelected (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AmountField != null) {
				AmountField.Dispose ();
				AmountField = null;
			}

			if (AmountStepper != null) {
				AmountStepper.Dispose ();
				AmountStepper = null;
			}

			if (ColorWell != null) {
				ColorWell.Dispose ();
				ColorWell = null;
			}

			if (DateTime != null) {
				DateTime.Dispose ();
				DateTime = null;
			}

			if (FeedbackLabel != null) {
				FeedbackLabel.Dispose ();
				FeedbackLabel = null;
			}

			if (ImageWell != null) {
				ImageWell.Dispose ();
				ImageWell = null;
			}

			if (SegmentButtons != null) {
				SegmentButtons.Dispose ();
				SegmentButtons = null;
			}

			if (SegmentSelection != null) {
				SegmentSelection.Dispose ();
				SegmentSelection = null;
			}

			if (SliderValue != null) {
				SliderValue.Dispose ();
				SliderValue = null;
			}

			if (TickedSlider != null) {
				TickedSlider.Dispose ();
				TickedSlider = null;
			}
		}
	}
}
