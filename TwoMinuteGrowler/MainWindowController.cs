
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.Growl;

namespace TwoMinuteGrowler {
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController {
		CountDownTimer counter;

		public MainWindowController (IntPtr handle) : base (handle) { }

		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder) { }

		public MainWindowController () : base("MainWindow") { }


		public override void AwakeFromNib () 
		{			
			GrowlApplicationBridge.WeakDelegate = this;
			counter = new CountDownTimer ();
			Bind ("countDown", counter, "timeLeft", null);
		}

		partial void startStopAction (NSButton sender) 
		{
			if (sender.Title == "Start") {
				counter.Start ();
				sender.Title = "Stop";
				GrowlApplicationBridge.Notify ("The two-minute rule is magic.", 
				                               "You now have two minutes to Get Your Things Done.", "Start", null, 0, false, null);
			} else {
				counter.Stop ();
				sender.Title = "Start";
				if (counter.TimerMark.Minutes > 0 && counter.TimerMark.Seconds > 0)
					GrowlApplicationBridge.Notify ("Action Completed", String.Format ("You still have {0} left.  Step back and breath.  " + "Take a second and contemplate what you have achieved.  " + "You'll be suprised how many two-minute actions you can " + "perform even on your most critical projects", counter.TimeLeft), "Stop", null, 0, true, null);
			}
		}

		[Export("countDown")]
		public string CountDown {
			get { return countDownLabel.StringValue; }
			set {
				countDownLabel.StringValue = counter.TimeLeft;
				if (counter.TimerMark.Minutes == 0 && counter.TimerMark.Seconds == 0) {
					GrowlApplicationBridge.Notify ("Time is up", 
					                               "Your two minutes is up.  Did you get everything done?  " 
					                               + "You need to clarify what is next and then manage that accordingly.", "Stop", null, 0, false, null);
					counter.Stop ();
					startStopButton.Title = "Start";
					
				} else if (counter.TimerMark.Minutes == 1 && counter.TimerMark.Seconds == 0) {
					GrowlApplicationBridge.Notify ("One Minute Warning", "This is your one minute warning.  " 
					                               + "Not to put more pressure on you but you had better get a move on!", "Info", null, 0, false, null);
				}
			}
		}

		[Export("registrationDictionaryForGrowl")]
		NSDictionary RegistrationDictionaryForGrowl () 
		{
			var regPath = NSBundle.MainBundle.PathForResource ("GrowlRegistrationTicket", "plist");
			var reg = NSDictionary.FromFile (regPath);
			return reg;
		}
	
	}
}

