using AppKit;
using Foundation;

namespace MouseTrackingExample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public override void DidFinishLaunching (NSNotification notification)
		{
			TrackingView.OnDragChange += (sender, e) => TheLabel.StringValue = e.Description;
		}
	}
}
