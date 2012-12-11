using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace PredicateEditorSample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MyWindowController myWindowController;

		public AppDelegate ()
		{
		}

		public override void FinishedLaunching (NSObject notification)
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

