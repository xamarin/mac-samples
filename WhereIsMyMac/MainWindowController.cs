
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreLocation;
using MonoMac.WebKit;
using System.IO;

namespace WhereIsMyMac
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController {		   
		CLLocationManager locationManager;
		
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder)
		{
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			locationManager = new CLLocationManager();
			locationManager.UpdatedLocation += HandleLocationManagerUpdatedLocation;
			locationManager.Failed += HandleLocationManagerFailed;
			locationManager.StartUpdatingLocation();
		}

		void HandleLocationManagerFailed (object sender, MonoMac.Foundation.NSErrorEventArgs e)
		{
			Console.WriteLine ("Failed");
		}

		void HandleLocationManagerUpdatedLocation (object sender, CLLocationUpdatedEventArgs e)
		{
			// Ignore updates where nothing we care about changed
			if (e.OldLocation != null && e.NewLocation.Coordinate.Longitude == e.OldLocation.Coordinate.Longitude &&
			    e.NewLocation.Coordinate.Latitude == e.OldLocation.Coordinate.Latitude &&
			    e.NewLocation.HorizontalAccuracy == e.OldLocation.HorizontalAccuracy)
				return;	   
	
			// Load the HTML for displaying the Google map from a file and replace the
			// format placeholders with our location data
			string path = NSBundle.MainBundle.PathForResource ("HTMLFormatString","html");
				
			var formatString = File.OpenText (path).ReadToEnd ();
			var htmlString = String.Format (
				formatString,
				e.NewLocation.Coordinate.Latitude,e.NewLocation.Coordinate.Longitude,
				latitudeRangeForLocation (e.NewLocation), longitudeRangeForLocation (e.NewLocation));
		
			webView.MainFrame.LoadHtmlString (htmlString, null);
	
			locationLabel.StringValue = string.Format ("{0}, {1}", e.NewLocation.Coordinate.Latitude, e.NewLocation.Coordinate.Longitude);
			accuracyLabel.StringValue = e.NewLocation.HorizontalAccuracy.ToString();
		}
		
		double latitudeRangeForLocation(CLLocation location)
		{
			const double M = 6367000.0; // approximate average meridional radius of curvature of earth
			const double metersToLatitude = 1.0 / ((Math.PI / 180.0) * M);
			const double accuracyToWindowScale = 2.0; 
			
			return location.HorizontalAccuracy * metersToLatitude * accuracyToWindowScale;
		}
		    
		double longitudeRangeForLocation(CLLocation location)
		{
			double latitudeRange = latitudeRangeForLocation(location);
			return latitudeRange * Math.Cos (location.Coordinate.Latitude * Math.PI / 180);
		}
		
		partial void openInDefaultBrowser (NSButton sender)
		{
			CLLocation currentLocation = locationManager.Location;
			
			// it can take a few seconds before a location is returned
			if (currentLocation == null) {
				AppKitFramework.NSBeep ();
				return;
			}
			
			var urlPath = String.Format("http://maps.google.com/maps?ll={0},{1}&amp;spn={2},{3}",
						    currentLocation.Coordinate.Latitude,currentLocation.Coordinate.Longitude,
						    latitudeRangeForLocation (currentLocation), longitudeRangeForLocation (currentLocation));

			var externalBrowserURL = new NSUrl (urlPath);
			NSWorkspace.SharedWorkspace.OpenUrl (externalBrowserURL);
		}
	}
}

