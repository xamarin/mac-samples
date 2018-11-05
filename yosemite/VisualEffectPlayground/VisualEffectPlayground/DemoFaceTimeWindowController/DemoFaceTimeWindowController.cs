using System;
using AppKit;
using Foundation;

namespace VisualEffectPlayground
{
	partial class DemoFaceTimeWindowController : NSWindowController
	{
		public DemoFaceTimeWindowController () : base ("DemoFaceTimeWindowController")
		{
		}

		public override void WindowDidLoad ()
		{
			Window.Appearance = NSAppearance.GetAppearance (NSAppearance.NameVibrantDark);
			Window.StyleMask = Window.StyleMask | NSWindowStyle.FullSizeContentView;
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;
			Window.TitlebarAppearsTransparent = true;
			Window.MovableByWindowBackground = true;

			ImageLoader.LoadImage(ImageView, "/Library/Desktop Pictures/Moon.jpg", "/Library/Desktop Pictures/Lion.jpg");
		}
	}
}

