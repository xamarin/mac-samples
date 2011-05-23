using System;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace ScriptingBridgeFinder
{
	public partial class AppDelegate : NSApplicationDelegate
	{
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
		
		public override void DidBecomeActive (NSNotification notification)
		{
			mainWindowController.LoadApps();
		}
	}
}

