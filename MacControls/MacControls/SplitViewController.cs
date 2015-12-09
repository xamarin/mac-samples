using System;

using AppKit;
using Foundation;

namespace MacControls
{
	public partial class SplitViewController : NSSplitViewController
	{
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
		public SplitViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Grab elements
			var left = LeftController.ViewController as LeftViewController;
			var right = RightController.ViewController as RightViewController;

			// Wireup events
			left.ViewTypeChanged += (viewType) => {
				right.ShowView(viewType);
			};
		}
		#endregion


	}
}
