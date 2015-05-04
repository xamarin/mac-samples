using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewChecksRadio : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewChecksRadio (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewChecksRadio (NSCoder coder) : base (coder)
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

			// Wireup controls
			AdjustTime.Activated += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Adjust Time: {0}",AdjustTime.State == NSCellStateValue.On);
			};

			TransportationCar.Activated += (sender, e) => {
				FeedbackLabel.StringValue = "Car Selected";
			};

			TransportationPublic.Activated += (sender, e) => {
				FeedbackLabel.StringValue = "Public Selected";
			};

			TransportationWalking.Activated += (sender, e) => {
				FeedbackLabel.StringValue = "Walking Selected";
			};

		}
		#endregion

		#region Button Actions
		partial void SelectCar (Foundation.NSObject sender) {
			Transportation.SelectCell(TransportationCar);
			FeedbackLabel.StringValue = "Car Selected";
		}
		#endregion
	}
}
