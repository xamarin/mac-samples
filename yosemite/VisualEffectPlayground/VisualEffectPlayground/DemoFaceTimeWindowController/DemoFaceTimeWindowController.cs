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
			// TODO: https://bugzilla.xamarin.com/show_bug.cgi?id=24176
//			self.window.appearance = [NSAppearance appearanceNamed:NSAppearanceNameVibrantDark];

			Window.StyleMask = Window.StyleMask | NSWindowStyle.FullSizeContentView;
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;
			Window.TitlebarAppearsTransparent = true;
			Window.MovableByWindowBackground = true;

			ImageView.Image = new NSImage ("/Library/Desktop Pictures/Lion.jpg");
		}
	}
}

