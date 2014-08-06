using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace SearchField
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		SearchFieldWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
		{
			
			mainWindowController = new SearchFieldWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}

