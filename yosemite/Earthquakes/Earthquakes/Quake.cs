using System;
using CoreData;
using Foundation;
using ObjCRuntime;

namespace Earthquakes
{
	[Register ("AAPLQuake")]
	public class Quake : NSManagedObject
	{
		const string CodeKey = "code";
		const string MagnitudeKey = "mag";
		const string PlaceKey = "place";
		const string DetailURLKey = "detail";
		const string TimeKey = "time";
		const string LocationKey = "geometry";

		public NSNumber Magnitude { 
			get {
				return (NSNumber)GetObjectForKey ("magnitude");
			}
			set {
				SetValueForKey (value, (NSString)"magnitude");
			} 
		}

		public NSString Location { 
			get {
				return (NSString)GetObjectForKey ("placeName");
			} 
			set {
				SetValueForKey (value, (NSString)"placeName");
			}
		}

		public NSDate Date {
			get {
				return (NSDate)GetObjectForKey ("time");
			} 
			set {
				SetValueForKey (value, (NSString)"time");
			}
		}

		public NSString DetailURL {
			get {
				return (NSString)GetObjectForKey ("detailURL");
			} 
			set {
				SetValueForKey (value, (NSString)"detailURL");
			}
		}

		public NSNumber Latitude {
			get {
				return (NSNumber)GetObjectForKey ("latitude");
			} 
			set {
				SetValueForKey (value, (NSString)"latitude");
			}
		}

		public NSNumber Longitude {
			get {
				return (NSNumber)GetObjectForKey ("longitude");
			} 
			set {
				SetValueForKey (value, (NSString)"longitude");
			}
		}

		public NSNumber Depth {
			get {
				return (NSNumber)GetObjectForKey ("depth");
			} 
			set {
				SetValueForKey (value, (NSString)"depth");
			}
		}

		public NSString Code {
			get {
				return (NSString)GetObjectForKey ("code");
			} 
			set {
				SetValueForKey (value, (NSString)"code");
			}
		}

		public Quake (IntPtr handle) : base (handle)
		{
		}

		public void UpdateFromDictionary (NSDictionary quakeDictionary)
		{
			foreach (var keyValuePair in quakeDictionary) {

				if (keyValuePair.Value == null)
					continue;
				string key = (NSString)keyValuePair.Key;
				if (key == MagnitudeKey) {
					var value = keyValuePair.Value as NSNumber;
					if (value == null)
						continue;
					Magnitude = value.FloatValue;
				} else if (key == PlaceKey)
					Location = (NSString)keyValuePair.Value;
				else if (key == CodeKey)
					Code = (NSString)keyValuePair.Value;
				else if (key == DetailURLKey)
					DetailURL = (NSString)keyValuePair.Value;
				else if (key == TimeKey) {
					var time = (NSNumber)keyValuePair.Value;
					double timeInterval = time.DoubleValue / 1000.0;
					Date = NSDate.FromTimeIntervalSince1970 (timeInterval);
				} else if (key == LocationKey) {
					var geometry = (NSDictionary)keyValuePair.Value;
					NSObject coordinatesObject;
					geometry.TryGetValue ((NSString)"coordinates", out coordinatesObject);
					var coordinates = (NSArray)coordinatesObject;

					// The longitude, latitude, and depth values are stored in an array in JSON.
					// Access these values by index directly.
					Longitude = coordinates.GetItem<NSNumber> (0).FloatValue;
					Latitude = coordinates.GetItem<NSNumber> (1).FloatValue;
					Depth = coordinates.GetItem<NSNumber> (2).FloatValue;
				}
			}
		}

		NSObject GetObjectForKey (string key)
		{
			return Runtime.GetNSObject (ValueForKey (key));
		}
	}
}

