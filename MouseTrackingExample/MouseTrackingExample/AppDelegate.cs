using AppKit;
using Foundation;
using ObjCRuntime;

namespace MouseTrackingExample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			this.TrackingView.OnDragChange += (sender, e) => TheLabel.StringValue = e.Description;
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
	}
}
