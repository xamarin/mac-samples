using System;
using System.Timers;
using System.Threading;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.Growl;

namespace TwoMinuteGrowler
{
	public class CountDownTimer : NSObject {
		NSTimer myTTimer;
		string timer;
 		TimeSpan currentTime;
		
		public CountDownTimer () : base()
		{
			currentTime = TimeSpan.FromMinutes (2);
			TimeLeft = formatTimeSpan (currentTime);
		}

		public void Start () 
		{
			currentTime = TimeSpan.FromMinutes (2);
			TimeLeft = formatTimeSpan (currentTime);
			
			myTTimer = NSTimer.CreateRepeatingScheduledTimer (1, delegate { 
				currentTime = currentTime - TimeSpan.FromSeconds (1);
				TimeLeft = formatTimeSpan (currentTime);
			});
		}
		
		private string formatTimeSpan (TimeSpan ts)
		{
			return String.Format ("{0}:{1:00}", ts.Minutes, ts.Seconds);	
		}
		
		public void Stop ()
		{
			myTTimer.Invalidate ();	
		}
		
		[Export ("timeLeft")]
		public string TimeLeft {
			get {
				return timer; 
			}
			set {
				WillChangeValue ("timeLeft");
				timer = value;
				DidChangeValue ("timeLeft");
			}
		}
		
		public TimeSpan TimerMark {
			get { return currentTime; }	
		}
		
	}
}

