using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace MacDatabinding
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
			var source = SourceController as MainViewController;
			var destination = DestinationController as NSViewController;

			// Remove any existing view
			if (source.Content.Subviews.Length > 0) {
				source.Content.Subviews [0].RemoveFromSuperview ();
			}

			// Adjust sizing and add new view
			destination.View.Frame = new CGRect(0 ,0 ,source.Content.Frame.Width, source.Content.Frame.Height);
			destination.View.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
			source.Content.AddSubview(destination.View);
		
		}
		#endregion

	}

}