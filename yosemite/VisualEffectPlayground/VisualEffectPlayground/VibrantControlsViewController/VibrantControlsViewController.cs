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
			ImageLoader.LoadImage(TopImageView, "/Library/Desktop Pictures/Frog.jpg", "/Library/Desktop Pictures/Poppies.jpg");
		}
	}
}

