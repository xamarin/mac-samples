using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;


namespace TwoMinuteGrowler {
	public partial class AppDelegate : NSApplicationDelegate {
		MainWindowController mainWindowController;

		public AppDelegate () 
		{
		}

		public override void FinishedLaunching (NSObject notification) 
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender) 
		{
			return true;
		}
	}
}

