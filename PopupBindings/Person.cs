using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PopupBindings
{
	public partial class Person : NSObject
	{
		Dictionary<string, object> personValues;
		static string NAME = "name";
		static string AGE = "age";
		static string ADDRESS_STREET = "addressStreet";
		static string ADDRESS_CITY = "addressCity";
		static string ADDRESS_STATE = "addressState";
		static string ADDRESS_ZIP = "addressZip";

		string[] keys = new string[] { NAME, AGE, ADDRESS_STREET, ADDRESS_CITY, ADDRESS_STATE, ADDRESS_ZIP };

		public Person (object[] attributes)
		{
			var newAttributes = new Dictionary<string, object> ();
			for (int x = 0; x < keys.Length; x++)
				newAttributes.Add(keys[x],attributes[x]);

			this.personValues = newAttributes;	
		}
		
		[Export ("name")]
		public string Name {
			get {
				return personValues [NAME].ToString ();	
			}
			
			set {
				personValues [NAME] = value ?? String.Empty;
			}
		}

		[Export ("age")]
		public int Age {
			get {
				return (int)personValues [AGE];
			}
			set {
				personValues [AGE] = value;
			}
		}
		
		[Export("addressStreet")]
		public string AddressStreet {
			get {
				return personValues [ADDRESS_STREET].ToString ();
			}
			set {
				personValues [ADDRESS_STREET] = value ?? String.Empty;
			}
		}
		
		[Export("addressCity")]
		public string AddressCity {
			get {
				return personValues [ADDRESS_CITY].ToString ();	
			}
			set {
				personValues [ADDRESS_CITY] = value ?? String.Empty;
			}
		}
		
		[Export ("addressState")]
		public string AddressState {
			get {
				return personValues [ADDRESS_STATE].ToString ();	
			}
			set
			{
				personValues [ADDRESS_STATE] = value ?? String.Empty;
			}
		}
		
		[Export ("addressZip")]
		public string AddressZip {
			get {
				return personValues [ADDRESS_ZIP].ToString ();	
			}
			set {
				personValues [ADDRESS_ZIP] = value ?? String.Empty;
			}
		}
		
		public Dictionary<string,object> Attributes
		{
			get { return personValues; }
			set {
				personValues = value;	
			}
		}
		
	}
}

