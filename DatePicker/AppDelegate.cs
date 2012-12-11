using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace DatePicker
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		TestWindowController testWindowController;
		
		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
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

