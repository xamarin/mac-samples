
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreLocation;
using WebKit;
using System.IO;

namespace WhereIsMyMac
{
	public partial class MainWindowController : AppKit.NSWindowController {

		CLLocationManager locationManager;
		CLLocationCoordinate2D locationCoordinate;
		CLAuthorizationStatus status;
		
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
			if (locationManager != null)
				return;

			locationManager = new CLLocationManager ();

			locationManager.AuthorizationChanged += OnAuthorizationChanged;
			locationManager.LocationsUpdated += OnLocationsUpdated;
		}

		void OnAuthorizationChanged (object sender, CLAuthorizationChangedEventArgs e)
		{
			Console.WriteLine ("new authorization state = {0}", e.Status);
			status = e.Status;
			locationManager.StartUpdatingLocation ();
		}

		void OnLocationsUpdated (object sender, CLLocationsUpdatedEventArgs e)
		{
			locationCoordinate = e.Locations.Last ().Coordinate;

			// Load the HTML for displaying the Google map from a file and replace the
			// format placeholders with our location data
			string path = NSBundle.MainBundle.PathForResource ("HTMLFormatString","html");
			var formatString = File.OpenText (path).ReadToEnd ();
			var htmlString = String.Format (
				formatString,
				locationCoordinate.Latitude,locationCoordinate.Longitude,
				latitudeRangeForLocation (e.Locations.Last ()), longitudeRangeForLocation (e.Locations.Last ()));
		
			webView.MainFrame.LoadHtmlString (htmlString, null);
	
			locationLabel.StringValue = string.Format ("{0}, {1}", locationCoordinate.Latitude, locationCoordinate.Longitude);
			accuracyLabel.StringValue = e.Locations.Last ().HorizontalAccuracy.ToString ();
		}

		void HandleLocationManagerFailed (object sender, Foundation.NSErrorEventArgs e)
		{
			Console.WriteLine ("Failed");
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

