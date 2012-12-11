
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace UserNotificationExample
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		partial void NotifyMeAction (MonoMac.AppKit.NSButton sender)
		{
			// First we create our notification and customize as needed
			NSUserNotification not = new NSUserNotification();
			not.Title = "Hello World";
			not.InformativeText = "This is an informative text";
			not.DeliveryDate = DateTime.Now;
			not.SoundName = NSUserNotification.NSUserNotificationDefaultSoundName;

			// We get the Default notification Center
			NSUserNotificationCenter center = NSUserNotificationCenter.DefaultUserNotificationCenter;
			
			center.DidDeliverNotification += (s, e) => 
			{
				Console.WriteLine("Notification Delivered");
				DeliveredColorWell.Color = NSColor.Green;
			};
			
			center.DidActivateNotification += (s, e) => 
			{
				Console.WriteLine("Notification Touched");
				TouchedColorWell.Color = NSColor.Green;
			};

			// If we return true here, Notification will show up even if your app is TopMost.
			center.ShouldPresentNotification = (c, n) => { return true; };
			
			center.ScheduleNotification(not);

		}

		// This will only reset colors to red
		partial void ResetAction (MonoMac.AppKit.NSButton sender)
		{
			DeliveredColorWell.Color = NSColor.Red;
			TouchedColorWell.Color = NSColor.Red;
		}
		
		#endregion
	}
}

