using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewTextControls : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewTextControls (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTextControls (NSCoder coder) : base (coder)
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
			UserField.EditingEnded += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("User ID: {0}",UserField.StringValue);
			};

			PasswordField.EditingEnded += (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Password: {0}",PasswordField.StringValue);
			};

			NumberField.EditingEnded+= (sender, e) => {
				FeedbackLabel.StringValue = string.Format("Number: {0}",NumberField.IntValue);
			};
		}
		#endregion
	}
}
