using System;
using AppKit;

namespace VisualEffectPlayground
{
	partial class BasicSidebarViewController : NSViewController
	{
		public BasicSidebarViewController () : base ("BasicSidebarViewController", Foundation.NSBundle.MainBundle)
		{
		}

		public BasicSidebarViewController (string nibName) : base (nibName, null)
		{
		}

		public override void AwakeFromNib ()
		{
			// This image will appear non-vibrant by virtue of it NOT being a template image. If it is a template image it will be vibrant.
			ImageLoader.LoadImage(TopImageView, "/Library/Desktop Pictures/Color Burst 1.jpg", "/Library/Desktop Pictures/Elephant.jpg");
			ImageLoader.LoadImage(SideImageView, "/Library/Desktop Pictures/Color Burst 1.jpg", "/Library/Desktop Pictures/Elephant.jpg");
		}
	}
}

