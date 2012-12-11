using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace DrawerMadness
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		ParentWindowController parentWindowController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			parentWindowController = new ParentWindowController ();
			parentWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
		
	}
}

