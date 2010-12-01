using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PopupBindings
{
	public partial class Person : NSObject
	{
		NSMutableDictionary personValues;
		static NSString NAME = new NSString ("name");
		static NSString AGE = new NSString ("age");
		static NSString ADDRESS_STREET = new NSString ("addressStreet");
		static NSString ADDRESS_CITY = new NSString ("addressCity");
		static NSString ADDRESS_STATE = new NSString ("addressState");
		static NSString ADDRESS_ZIP = new NSString ("addressZip");
		
		public Person (NSMutableDictionary attributes)
		{
			this.personValues = new NSMutableDictionary(attributes);	
		}
		
		[Export ("name")]
		public NSString Name
		{
			get {
				return (NSString) personValues.ObjectForKey (NAME);	
			}
		}

		[Export ("age")]
		public NSNumber Age {
			get {
				return (NSNumber) personValues.ObjectForKey (AGE);	
			}
		}
		
		// Get a value for a key.  Not using this method but instead
		//  used the [Export("xxxxxx")] method.
		//public override NSObject ValueForKey (NSString key)
		//{
		//	return attributeValues[key];
		//}
		
		public override void SetValueForKey (NSObject value, NSString key)
		{
			
			if (personValues.ContainsKey (key))
				personValues [key] = value;	
			else
				base.SetValueForKey (value, key);

			// you can also just do a simple:
			//attributeValues[key] = value;
			
		}
		
		[Export("addressStreet")]
		public NSString AddressStreet {
			get {
				return (NSString) personValues [ADDRESS_STREET];	
			}
		}
		
		[Export("addressCity")]
		public NSString AddressCity {
			get {
				return (NSString)personValues[ADDRESS_CITY];	
			}
		}
		
		[Export ("addressState")]
		public NSString AddressState {
			get {
				return (NSString)personValues[ADDRESS_STATE];	
			}
		}
		
		[Export ("addressZip")]
		public NSString AddressZip {
			get {
				return (NSString)personValues.ObjectForKey(ADDRESS_ZIP);	
			}
		}
		
		public NSMutableDictionary Attributes
		{
			get { return personValues; }
			set {
				personValues = NSMutableDictionary.FromDictionary(value);	
			}
		}
		
	}
}

