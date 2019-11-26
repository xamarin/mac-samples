//
// Original sample by Matt Aimonetti
//
// This sample finds your location and displays it in Google maps
//
using System;

using CoreLocation;
using Foundation;
using AppKit;

class Locator {
	const string googleUrl = "http://maps.google.com/maps?f=q&source=s_q&hl=en&geocode=&q=#{0},#{1}";

	public class LocationDelegate : NSObject, ICLLocationManagerDelegate
	{
		[Export ("locationManager:didChangeAuthorizationStatus:")]
		public void AuthorizationChanged (CLLocationManager manager, CLAuthorizationStatus status)
		{
		}

		[Export ("locationManager:didUpdateLocations:")]
		public void LocationsUpdated (CLLocationManager manager, CLLocation[] locations)
		{
			var coord = locations[0].Coordinate;
			Console.WriteLine ("At {0}", locations[0].Description ());
			manager.StopUpdatingLocation ();

			Console.WriteLine (googleUrl, coord.Latitude, coord.Longitude);
			NSString s = new NSString (String.Format (googleUrl, coord.Latitude, coord.Longitude));
			s = s.CreateStringByAddingPercentEncoding (NSUrlUtilities_NSCharacterSet.UrlQueryAllowedCharacterSet);
			NSWorkspace.SharedWorkspace.OpenUrl (new NSUrl (s));
		}

		[Export ("locationManager:didFailWithError:")]
		public void Failed (CLLocationManager manager, NSError error)
		{
			Console.WriteLine ("Error: " + error);
		}
	}

	static void Main ()
	{
		NSApplication.Init ();
		
		var locationManager = new CLLocationManager ();
		locationManager.Delegate = new LocationDelegate ();
		locationManager.DesiredAccuracy = CLLocation.AccuracyBest;
		locationManager.RequestAlwaysAuthorization ();
		locationManager.RequestLocation ();

		NSRunLoop.Current.RunUntil (NSDate.DistantFuture);
	}
}
