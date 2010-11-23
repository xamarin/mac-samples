
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PopupBindings
{
	public partial class TestWindowController : MonoMac.AppKit.NSWindowController
	{
		
		static NSString NAME = new NSString("name");
		static NSString AGE = new NSString("age");
		static NSString ADDRESS_STREET = new NSString("addressStreet");
		static NSString ADDRESS_CITY = new NSString("addressCity");
		static NSString ADDRESS_STATE = new NSString("addressState");
		static NSString ADDRESS_ZIP = new NSString("addressZip");

		List<NSObject> keys = new List<NSObject> {NAME,AGE,ADDRESS_STREET,ADDRESS_CITY,
														ADDRESS_STATE,ADDRESS_ZIP};
		
		#region Constructors

		// Called when created from unmanaged code
		public TestWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public TestWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public TestWindowController () : base("TestWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new TestWindow Window {
			get { return (TestWindow)base.Window; }
		}
		
		public override void AwakeFromNib ()
		{
			//base.AwakeFromNib ();
			
			List<NSObject> values = new List<NSObject> {new NSString("Joe Smith"),
				                                        NSNumber.FromInt32(21),
				                                        new NSString("451 University Avenue"),
				                                        new NSString("Palo Alto"), 
														new NSString("CA"),
				                                        new NSString("94301")};

			addNewPerson(values);

			values = new List<NSObject> {new NSString("John Doe"),
				                                        NSNumber.FromInt32(31),
				                                        new NSString("767 Fifth Ave."),
				                                        new NSString("New York"), 
														new NSString("NY"),
				                                        new NSString("10153")};

			addNewPerson(values);
			
			values = new List<NSObject> {new NSString("Sally Sixpack"),
				                                        NSNumber.FromInt32(41),
				                                        new NSString("679 North Michigan Ave."),
				                                        new NSString("Chicago"), 
														new NSString("IL"),
				                                        new NSString("60611")};

			addNewPerson(values);
			
			values = new List<NSObject> {new NSString("John Q. Public"),
				                                        NSNumber.FromInt32(141),
				                                        new NSString("5085 Westheimer Rd."),
				                                        new NSString("Houston"), 
														new NSString("TX"),
				                                        new NSString("77056")};

			addNewPerson(values);			

			// select the first popup menu item
			arrayController.SelectionIndex = 0;
		}
		
		private void addNewPerson(List<NSObject> properties)
		{
			NSMutableDictionary propertiesDict = NSMutableDictionary.FromObjectsAndKeys(properties.ToArray(),keys.ToArray());
			arrayController.AddObject(new Person(propertiesDict));			
		}
	}
}

