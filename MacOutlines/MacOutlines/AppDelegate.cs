using System;

using Foundation;
using AppKit;

namespace MacOutlines
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

			var rotation = new RotationWindowController ();
			rotation.Window.MakeKeyAndOrderFront (this);
		}
	}
}

