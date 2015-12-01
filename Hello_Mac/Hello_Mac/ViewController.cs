using System;

using AppKit;
using Foundation;

namespace Hello_Mac
{
	public partial class ViewController : NSViewController
	{
		#region Private Variables
		private int numberOfTimesClicked = 0;
		#endregion

		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Set the initial value for the label
			ClickedLabel.StringValue = "Button has not been clicked yet.";
		}
		#endregion

		#region Custom Actions
		partial void ClickedButton (Foundation.NSObject sender) {

			// Update counter and label
			ClickedLabel.StringValue = string.Format("The button has been clicked {0} time{1}.",++numberOfTimesClicked, (numberOfTimesClicked < 2) ? "" : "s");
		}
		#endregion
	}
}
