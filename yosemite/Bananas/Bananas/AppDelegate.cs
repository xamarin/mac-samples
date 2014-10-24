using System;
using AppKit;
using Foundation;


namespace Bananas
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public override void FinishedLaunching (NSObject notification)
		{
			SharedAppDelegate.AppDelegate = new SharedAppDelegate (scnView);
			window.DisableSnapshotRestoration ();
			SharedAppDelegate.AppDelegate.CommonApplicationDidFinishLaunching (null);
		}

		partial void Pause (Foundation.NSObject sender)
		{
			SharedAppDelegate.AppDelegate.TogglePaused ();
		}
	}
}
