
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace UserNotificationExample
{
	public partial class MainWindow : AppKit.NSWindow
	{
	
		NSUserNotificationCenter center { get; set; }
	
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
			// We get the Default notification Center
			if (center == null )
				center = NSUserNotificationCenter.DefaultUserNotificationCenter;

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

		}

		partial void NotifyMeAction (AppKit.NSButton sender)
		{
			// First we create our notification and customize as needed
			NSUserNotification not = null;

			try {
				not = new NSUserNotification();
			} catch {
				new NSAlert {
					MessageText = "NSUserNotification Not Supported",
					InformativeText = "This API was introduced in OS X Mountain Lion (10.8)."
				}.RunSheetModal (this);
				return;
			}

			not.Title = "Hello World";
			not.InformativeText = "This is an informative text";
			not.DeliveryDate = (NSDate)DateTime.Now;
			not.SoundName = NSUserNotification.NSUserNotificationDefaultSoundName;
			
			center.ScheduleNotification(not);
		}

		// This will only reset colors to red
		partial void ResetAction (AppKit.NSButton sender)
		{
			DeliveredColorWell.Color = NSColor.Red;
			TouchedColorWell.Color = NSColor.Red;
		}
		
		#endregion
	}
}

