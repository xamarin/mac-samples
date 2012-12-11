using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace AnimatingViews
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		AnimatingViewsWindowController animatingViewsWindowController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			animatingViewsWindowController = new AnimatingViewsWindowController ();
			animatingViewsWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}

