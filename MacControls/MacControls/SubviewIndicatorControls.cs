using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewIndicatorControls : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewIndicatorControls (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewIndicatorControls (NSCoder coder) : base (coder)
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

			// Start animation on progress bars
			Indeterminate.StartAnimation (this);
			AsyncProgress.StartAnimation (this);

			// Wireup controls
			LevelIndicator.Activated += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Level: {0:###}",LevelIndicator.DoubleValue);
			};

			Rating.Activated += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Rating: {0:###}",Rating.DoubleValue);
			};

			Relevance.Activated += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Relevance: {0:###}",Relevance.DoubleValue);
			};
		}
		#endregion

		#region Actions
		partial void HundredPercent (Foundation.NSObject sender) {
			ProgressIndicator.DoubleValue = 100;
		}
		#endregion
	}
}
