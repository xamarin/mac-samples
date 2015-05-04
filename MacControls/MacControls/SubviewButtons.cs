using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewButtons : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewButtons (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewButtons (NSCoder coder) : base (coder)
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

			// Wireup events
			ButtonOutlet.Activated += (sender, e) => {
				FeedbackLabel.StringValue = "Button Outlet Pressed";
			};

			DisclosureButton.Activated += (sender, e) => {
				LorumIpsum.Hidden = (DisclosureButton.State == NSCellStateValue.On);
			};

			RoundedGradient.Activated += (sender, e) => {
				FeedbackLabel.StringValue = "Rounded Gradient Pressed";
			};
		}
		#endregion

		#region Button Action Handlers
		partial void ButtonAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Action Pressed";
		}

		partial void RecessedAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Recessed Pressed";
		}

		partial void RoundAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Round Pressed";
		}

		partial void RoundRectAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Round Rect Pressed";
		}

		partial void RoundTexturedAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Round Textured Pressed";
		}

		partial void SquareAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Square Pressed";
		}

		partial void TexturedAction (Foundation.NSObject sender) {
			FeedbackLabel.StringValue = "Button Textured Pressed";
		}
		#endregion
	}
}
