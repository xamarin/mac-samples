using System;

using Foundation;
using AppKit;

namespace Hello_Mac
{
	public partial class MainWindow : NSWindow
	{
		#region Private Variables
		private int numberOfTimesClicked = 0;
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Private Methods
		partial void ClickedButton (Foundation.NSObject sender) {

			// Update counter and label
			ClickedLabel.StringValue = string.Format("The button has been clicked {0} time{1}.",++numberOfTimesClicked, (numberOfTimesClicked < 2) ? "" : "s");
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Set the initial value for the label
			ClickedLabel.StringValue = "Button has not been clicked yet.";
		}
		#endregion
	}
}
