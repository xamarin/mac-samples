using System;
using Foundation;
using AppKit;

namespace NeHeLesson4
{
	public partial class AppDelegate : NSApplicationDelegate
	{
	    MainWindowController mainWindowController;
	
	    public AppDelegate ()
	    {
	    }
	
	    public override void DidFinishLaunching (NSNotification notification)
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

