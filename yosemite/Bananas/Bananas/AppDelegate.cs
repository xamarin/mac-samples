using AppKit;
using Foundation;

namespace Bananas
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		[Export("applicationDidFinishLaunching:")]
		public override void DidFinishLaunching (NSNotification notification)
		{
			SharedAppDelegate.AppDelegate = new SharedAppDelegate (scnView);
			window.DisableSnapshotRestoration ();
			SharedAppDelegate.AppDelegate.CommonApplicationDidFinishLaunching (null);
		}

		partial void Pause (NSObject sender)
		{
			SharedAppDelegate.AppDelegate.TogglePaused ();
		}
	}
}
