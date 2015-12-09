using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace AppKit
{
	[Register("ReplaceViewSeque")]
	public class ReplaceViewSeque : NSStoryboardSegue
	{
		#region Constructors
		public ReplaceViewSeque() {

		}

		public ReplaceViewSeque (string identifier, NSObject sourceController, NSObject destinationController) : base(identifier,sourceController,destinationController) {

		}

		public ReplaceViewSeque (IntPtr handle) : base(handle) {
		}

		public ReplaceViewSeque (NSObjectFlag x) : base(x) {
		}
		#endregion

		#region Override Methods
		public override void Perform ()
		{
			// Cast the source and destination controllers
			var source = SourceController as NSViewController;
			var destination = DestinationController as NSViewController;

			// Remove any existing view
			if (source.View.Subviews.Length > 0) {
				source.View.Subviews [0].RemoveFromSuperview ();
			}

			// Adjust sizing and add new view
			destination.View.Frame = new CGRect(0 ,0 ,source.View.Frame.Width, source.View.Frame.Height);
			destination.View.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			source.View.AddSubview(destination.View);
		
		}
		#endregion

	}

}