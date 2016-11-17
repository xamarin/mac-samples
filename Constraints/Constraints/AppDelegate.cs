﻿using System;
using AppKit;
using CoreGraphics;
using Foundation;

namespace Constraints
{
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		int PADDING = 10;
		
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}
