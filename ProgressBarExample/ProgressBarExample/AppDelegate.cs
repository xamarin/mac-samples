using System;

using Foundation;
using AppKit;
using CoreGraphics;
using System.Threading.Tasks;

namespace ProgressBarExample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		NSTimer t;
		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

			NSProgressIndicator i = new NSProgressIndicator (new CGRect (50, 0, 400, 200));
			i.DoubleValue = 0;
			i.Indeterminate = false;

			double progressValue = 0;
			t = NSTimer.CreateRepeatingScheduledTimer (.5, (timer) => {
				if (!NSThread.Current.IsMainThread)
					throw new System.InvalidOperationException ("NSTimer should invoke on main?");

				if (progressValue >= 100)
					progressValue = 0;
				progressValue += 20;
				i.DoubleValue = progressValue;;
			});

			mainWindowController.Window.ContentView.AddSubview (i);
		}
	}
}

