using System;
using AppKit;

namespace VisualEffectPlayground
{
	partial class VibrantControlsViewController : NSViewController
	{
		public VibrantControlsViewController () : base ("VibrantControlsViewController", Foundation.NSBundle.MainBundle)
		{
		}

		public VibrantControlsViewController (string nibName) : base (nibName, null)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			if (TopImageView != null)
				TopImageView.Image = new NSImage ("/Library/Desktop Pictures/Frog.jpg");
		}
	}
}

