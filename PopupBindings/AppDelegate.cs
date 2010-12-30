using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace PopupBindings
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		TestWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new TestWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
		
		partial void openReadMe (NSObject sender)
		{
			string fullPath = NSBundle.MainBundle.PathForResource("ReadMe","txt");
			NSWorkspace.SharedWorkspace.OpenFile(fullPath);
		}
	}
}

