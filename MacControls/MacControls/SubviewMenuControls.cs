using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewMenuControls : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewMenuControls (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewMenuControls (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region Actions
		partial void ItemOne (Foundation.NSObject sender) {
			DropDownSelected.Title = "Item 1";
			FeedbackLabel.StringValue = "Item One Selected";
		}

		partial void ItemTwo (Foundation.NSObject sender) {
			DropDownSelected.Title = "Item 2";
			FeedbackLabel.StringValue = "Item Two Selected";
		}

		partial void ItemThree (Foundation.NSObject sender) {
			DropDownSelected.Title = "Item 3";
			FeedbackLabel.StringValue = "Item Three Selected";
		}
		#endregion
	}
}
