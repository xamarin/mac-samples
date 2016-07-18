using System;
using AppKit;
using CoreGraphics;
using Foundation;
using NotificationCenter;

namespace TodayExtension
{
	public partial class TodayViewController : NSViewController, INCWidgetProviding
	{
		// Building and run will launch the extesnion in the Widget Simulator
		// The green plus button in the Widget Simulator will add it to the Today / Notificaiton 
		// The plugin show up under the share menu, and running will launch Safari

		// Open "Console" application to view the system log to view NSLog / Errors / Crashes of extension
		// Cleaning this project will unregister this plugin from PluginKit.
		// PluginKit register/unregister can be done manually through the Apple pluginkit command line tool.
		// man pluginkit for details

		public TodayViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// See https://developer.xamarin.com/guides/ios/platform_features/introduction_to_extensions/
			var todayMessage = new NSTextField (new CGRect (0, 0, 250, View.Frame.Height))
			{
				Alignment = NSTextAlignment.Center,
				Selectable = false,
				Bordered = false
			};

			View.AddSubview (todayMessage);

			var dayOfYear = DateTime.Now.DayOfYear;
			var leapYearExtra = DateTime.IsLeapYear (DateTime.Now.Year) ? 1 : 0;
			var daysRemaining = 365 + leapYearExtra - dayOfYear;

			if (daysRemaining == 1)
				todayMessage.StringValue = String.Format ("Today is day {0}. There is one day remaining in the year.", dayOfYear);
			else
				todayMessage.StringValue = String.Format ("Today is day {0}. There are {1} days remaining in the year.", dayOfYear, daysRemaining);

			// See NSLogHelper for details on this vs Console.WriteLine
			ExtensionSamples.NSLogHelper.NSLog ("TodayViewController - LoadView - " + todayMessage.StringValue);
		}

		[Export ("widgetPerformUpdateWithCompletionHandler:")]
		public void WidgetPerformUpdate (Action<NCUpdateResult> completionHandler)
		{
			// Perform any setup necessary in order to update the view.

			// If an error is encoutered, use NCUpdateResultFailed
			// If there's no update required, use NCUpdateResultNoData
			// If there's an update, use NCUpdateResultNewData

			completionHandler (NCUpdateResult.NewData);
		}
	}
}

