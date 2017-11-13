using System;
using AppKit;
using Foundation;

namespace StartupTiming
{
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			Console.WriteLine (DateTimeOffset.Now.ToUnixTimeMilliseconds ());
		}
	}
}
