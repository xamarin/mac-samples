using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace PopupBindings
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		TestWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
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

