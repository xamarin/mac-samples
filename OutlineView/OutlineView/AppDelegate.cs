using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
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

