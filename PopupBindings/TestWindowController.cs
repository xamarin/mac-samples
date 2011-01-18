using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PopupBindings
{
	public partial class TestWindowController : MonoMac.AppKit.NSWindowController {
		// Called when created from unmanaged code
		public TestWindowController (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public TestWindowController (NSCoder coder) : base(coder)
		{
		}

		// Call to load from the XIB/NIB file
		public TestWindowController () : base("TestWindow")
		{
		}


		//strongly typed window accessor
		public new TestWindow Window {
			get { return (TestWindow)base.Window; }
		}
		
		public override void AwakeFromNib ()
		{
			object [] values;

			values = new object[] {"Joe Smith",
					       21,
					       "451 University Avenue",
					       "Palo Alto", 
					       "CA",
					       "94301"};
			
			addNewPerson (values);

			values = new object[] {"John Doe",
					       31,
					       "767 Fifth Ave.",
					       "New York", 
					       "NY",
					       "10153"};
			
			addNewPerson(values);
			
			values = new object[] {"Sally Sixpack",
					       41,
					       "679 North Michigan Ave.",
					       "Chicago", 
					       "IL",
					       "60611"};
			
			addNewPerson(values);
			
			values = new object[] {"John Q. Public",
					       141,
					       "5085 Westheimer Rd.",
		                                        "Houston", 
							"TX",
		                                        "77056"};

			addNewPerson (values);

			// select the first popup menu item
			arrayController.SelectionIndex = 0;
		}
		
		private void addNewPerson(object[] properties)
		{
			arrayController.AddObject (new Person (properties));			
		}
	}
}

