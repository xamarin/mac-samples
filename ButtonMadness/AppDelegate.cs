using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace SamplesButtonMadness
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
	}
}

