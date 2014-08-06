using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;
using System.IO;

namespace OutlineView
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		
		public AppDelegate ()
		{
		}
		
		public override void FinishedLaunching (NSObject notification)
		{
			mainWindowController = new MainWindowController ( );
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
		
	}
}

