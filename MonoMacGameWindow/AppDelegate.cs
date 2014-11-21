using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace MonoMacGameView
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MonoMacGameWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MonoMacGameWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}

