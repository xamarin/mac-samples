using AppKit;
using CoreGraphics;
using Foundation;

namespace ProgressBarExample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

			var progressIndicator = new NSProgressIndicator (new CGRect (50, 0, 400, 200)) {
				DoubleValue = 0,
				Indeterminate = false
			};

			double progressValue = 0;
			NSTimer.CreateRepeatingScheduledTimer (.5, timer => {
				if (!NSThread.Current.IsMainThread)
					throw new System.InvalidOperationException ("NSTimer should invoke on main?");

				if (progressValue >= 100)
					progressValue = 0;
				progressValue += 20;
				progressIndicator.DoubleValue = progressValue;
			});

			mainWindowController.Window.ContentView.AddSubview (progressIndicator);
		}
	}
}

