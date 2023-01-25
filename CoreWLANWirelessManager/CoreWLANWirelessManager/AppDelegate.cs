using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;
using CoreLocation;

namespace CoreWLANWirelessManager
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public AppDelegate ()
		{
		}
		
		public override void WillFinishLaunching(NSNotification notification)
		{
			var manager = new CLLocationManager();
			manager.RequestAlwaysAuthorization();
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
	}
}

