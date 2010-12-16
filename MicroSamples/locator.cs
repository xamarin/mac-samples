//
// Original sample by Matt Aimonetti
//
// This sample finds your location and displays it in Google maps
//
using System;
using MonoMac.CoreLocation;
using MonoMac.Foundation;
using MonoMac.AppKit;

class Locator {
	const string googleUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=#{0},#{1}";
	
	static void Main ()
	{
		NSApplication.Init ();
		
		var locationManager = new CLLocationManager ();
		locationManager.UpdatedLocation += (sender, args) => {
			var coord = args.NewLocation.Coordinate;
			Console.WriteLine ("At {0}", args.NewLocation.Description ());
			locationManager.StopUpdatingLocation ();

			Console.WriteLine (googleUrl, coord.Latitude, coord.Longitude);
			NSWorkspace.SharedWorkspace.OpenUrl (new Uri (String.Format (googleUrl, coord.Latitude, coord.Longitude)));
			
		};
		locationManager.StartUpdatingLocation ();
		NSRunLoop.Current.RunUntil (NSDate.DistantFuture);
	}
}