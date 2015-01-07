using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace VisualEffectPlayground
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MyKeyWindow mainWindow;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindow = new MyKeyWindow (CGRect.Empty, NSWindowStyle.Borderless, NSBackingStore.Buffered, false) {
				IsOpaque = false,
				IsMovable = true,
				MovableByWindowBackground = true,
				ReleasedWhenClosed = false,
				BackgroundColor = NSColor.Clear,
				ContentViewController = new TestLauncherViewController ()
			};

			mainWindow.Center ();
			mainWindow.MakeKeyAndOrderFront (null);
		}
	}
}
