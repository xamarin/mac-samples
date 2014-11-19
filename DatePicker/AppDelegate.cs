using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace DatePicker
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		TestWindowController testWindowController;
		
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			testWindowController = new TestWindowController ();
			testWindowController.Window.MakeKeyAndOrderFront (this);
			
			
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

	}
}

