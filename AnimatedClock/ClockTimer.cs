#pragma warning disable 414

using System;
using System.Timers;

using Foundation;
using AppKit;
using CoreAnimation;

namespace AnimatedClock
{
	public class ClockTimer : NSObject
	{
		NSTimer myTTimer;
		string property;
		
		public ClockTimer () : base()
		{
			outputString = DateTime.Now.ToString("hh:mm:ss");
			myTTimer = NSTimer.CreateRepeatingScheduledTimer (1,delegate { 
				outputString = DateTime.Now.ToString("hh:mm:ss");
			});
		}
		
		[Export("outputString")]
		public string outputString {
			get {
				return property; 
			}

			set {
				WillChangeValue("outputString");
				property = value;
				DidChangeValue("outputString");
			}
		}
	}
}

