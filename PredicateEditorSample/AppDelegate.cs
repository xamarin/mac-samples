using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace PredicateEditorSample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MyWindowController myWindowController;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			myWindowController = new MyWindowController ();
			myWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}

