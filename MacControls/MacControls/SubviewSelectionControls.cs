using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewSelectionControls : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewSelectionControls (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewSelectionControls (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Wire-up controls
			TickedSlider.Activated += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Stepper Value: {0:###}",TickedSlider.IntValue);
			};

			SliderValue.Activated += (sender, e) => {
				AmountField.StringValue = string.Format("{0:###}",SliderValue.IntValue);
				AmountStepper.IntValue = SliderValue.IntValue;
			};

			AmountStepper.Activated += (sender, e) => {
				AmountField.StringValue = string.Format("{0:###}",SliderValue.IntValue);
				SliderValue.IntValue = AmountStepper.IntValue;
			};

			CollorWell.Color = NSColor.Red;
			CollorWell.Activated += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Color Changed: {0}", CollorWell.Color);
			};

			ImageWell.Image = NSImage.ImageNamed ("tag.png");
			ImageWell.Activated += (sender, e) => {
				FeedbackLabel.StringValue = "Image Well Clicked";
			};

			DateTime.Activated += (sender, e) => {
				FeedbackLabel.StringValue = DateTime.StringValue;
			};

		}
		#endregion

		#region Actions
		partial void SegmentButtonPressed (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = string.Format("Button {0} Pressed",SegmentButtons.SelectedSegment);
		}

		partial void SegmentSelected (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = string.Format("Segment {0} Selected",SegmentSelection.SelectedSegment);
		}
		#endregion
	}
}
