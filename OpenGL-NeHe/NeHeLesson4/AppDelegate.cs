using System;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NeHeLesson4
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
	}
}

