
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NSTableViewBinding
{
	public partial class TestWindowController : MonoMac.AppKit.NSWindowController
	{
		
		internal static NSString FIRST_NAME = new NSString("firstname");
		internal static NSString LAST_NAME = new NSString("lastname");
		internal static NSString PHONE = new NSString("phone");
		
		EditController myEditController = null;
		
		internal static List<NSString> Keys = new List<NSString> { FIRST_NAME, LAST_NAME, PHONE};			
		internal const int FIRST_NAME_IDX = 0;
		internal const int LAST_NAME_IDX = 1;
		internal const int PHONE_IDX = 2;
		
		#region Constructors

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

		#endregion

		//strongly typed window accessor
		public new TestWindow Window {
			get { return (TestWindow)base.Window; }
		}
		
		#region Overrides
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
	
			//-----------------------------------------------------------------------------

			// Compiler directive USE_BINDINGS_BY_CODE set in the projects Options
			// 1) From the MonoDevelop Menu select Project -> NSTableViewBinding Options
			// 		This will bring up the project options panel.
			// 2) Under the category Build -> Compiler look for the field labeled Define Symbols half way 
			// 		down the panel page.
			// 3) Add the symbol USE_BINDINGS_BY_CODE 
			
			//-----------------------------------------------------------------------------
			// Your NSTableView's content needs to use Cocoa Bindings,
			// use Interface Builder to setup the bindings like so:
			//
			// Each column in the NSTableView needs to use Cocoa Bindings,
			// use Interface Builder to setup the bindings like so:
			//
			//		columnIdentifier: "firstname"
			//			"value" = arrangedObjects.firstname [NSTableArray (NSArrayController)]
			//				Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "arrangedObjects"
			//				Model Key Path = "firstname" ("firstname" is a key in "TableArray")
			//
			//		columnIdentifier: "lastname"
			//			"value" = arrangedObjects.lastname [NSTableArray (NSArrayController)]
			//				Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "arrangedObjects"
			//				Model Key Path = "lastname" ("lastname" is a key in "TableArray")
			//
			//		columnIdentifier: "phone"
			//			"value" = arrangedObjects.phone [NSTableArray (NSArrayController)]
			//				Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "arrangedObjects"
			//				Model Key Path = "phone" ("phone" is a key in "TableArray")
			//
			// or do bindings by code:
				
#if USE_BINDINGS_BY_CODE
			
			NSTableColumn firstNameColumn = myTableView.FindTableColumn(FIRST_NAME);
			firstNameColumn.Bind("value",myContentArray,"arrangedObjects.firstname",null);

			NSTableColumn lastNameColumn = myTableView.FindTableColumn(LAST_NAME);
			lastNameColumn.Bind("value",myContentArray,"arrangedObjects.lastname",null);

			NSTableColumn phoneColumn = myTableView.FindTableColumn(PHONE);
			phoneColumn.Bind("value",myContentArray,"arrangedObjects.phone",null);

#endif
			
			// for NSTableView "double-click row" to work you need to use Cocoa Bindings,
			// use Interface Builder to setup the bindings like so:
			//
			//	NSTableView:
			//		"doubleClickArgument":
			//			Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "selectedObjects"
			//				Selector Name = "inspect:" (don't forget the ":")
			//
			//		"doubleClickTarget":
			//			Bind To: (File's Owner) MyWindowController
			//				Model Key Path = "self"
			//				Selector Name = "inspect:" (don't forget the ":")
			//
			//	... also make sure none of the NSTableColumns are "editable".
			//
			// or do bindings by code:
#if USE_BINDINGS_BY_CODE
	
			
			List<NSObject> doubleClickObjects = new List<NSObject> {new NSString("inspect:"),
																	NSNumber.FromBoolean(true),
																	NSNumber.FromBoolean(true)};
			
			List<NSString> doubleClickKeys = new List<NSString> {new NSString("NSSelectorName"),
																	new NSString("NSConditionallySetsHidden"),
																	new NSString("NSRaisesForNotApplicableKeys")};
			
			NSDictionary doubleClickOptionsDict = NSDictionary.FromObjectsAndKeys(doubleClickObjects.ToArray(),doubleClickKeys.ToArray());
			
			myTableView.Bind("doubleClickArgument",myContentArray,"selectedObjects",doubleClickOptionsDict);
			myTableView.Bind("doubleClickTarget",this,"self",doubleClickOptionsDict);
			
#endif
			
			// the enabled states of the two buttons "Add", "Remove" are bound to "canRemove" 
			// use Interface Builder to setup the bindings like so:
			//
			//	NSButton ("Add")
			//		"enabled":
			//			Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "canAdd"
			//
			//	NSButton ("Remove")
			//		"enabled":
			//			Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "canRemove"
			//
			// or do bindings by code:
#if USE_BINDINGS_BY_CODE
			
			List<NSObject> enableOptionsObjects = new List<NSObject> {NSNumber.FromBoolean(true)};
			
			List<NSString> enableOptionsKeys = new List<NSString> {new NSString("NSRaisesForNotApplicableKeys")};
			
			NSDictionary enableOptionsDict = NSDictionary.FromObjectsAndKeys(enableOptionsObjects.ToArray(),enableOptionsKeys.ToArray());
			addButton.Bind("enabled",myContentArray,"canAdd",enableOptionsDict);
			removeButton.Bind("enabled",myContentArray,"canRemove",enableOptionsDict);
						
#endif			
			// the NSForm's text fields is bound to the current selection in the NSTableView's content array controller,
			// use Interface Builder to setup the bindings like so:
			//
			//	NSFormCell:
			//		"value":
			//			Bind To: "TableArray" object (NSArrayController)
			//				Controller Key = "selection"
			//				Model Key Path = "firstname"
			//
			// or do bindings by code:
#if USE_BINDINGS_BY_CODE

			List<NSObject> valueOptionsObjects = new List<NSObject> {NSNumber.FromBoolean(true),
																		NSNumber.FromBoolean(true),
																		NSNumber.FromBoolean(true)};
			
			List<NSString> valueOptionsKeys = new List<NSString> {new NSString("NSAllowsEditingMultipleValuesSelection"),
																	new NSString("NSConditionallySetsEditable"),
																	new NSString("NSRaisesForNotApplicableKeys")};
			
			NSDictionary valueOptionsDict = NSDictionary.FromObjectsAndKeys(valueOptionsObjects.ToArray(),valueOptionsKeys.ToArray());
	
			myFormFields.CellAtIndex(FIRST_NAME_IDX).Bind("value",myContentArray,"selection.firstname",valueOptionsDict);
			myFormFields.CellAtIndex(LAST_NAME_IDX).Bind("value",myContentArray,"selection.lastname",valueOptionsDict);
			myFormFields.CellAtIndex(PHONE_IDX).Bind("value",myContentArray,"selection.phone",valueOptionsDict);
			
					
#endif
			// start listening for selection changes in our NSTableView's array controller
			myContentArray.AddObserver(this,new NSString("selectionIndexes"),NSKeyValueObservingOptions.New,IntPtr.Zero);
			
			// finally, add the first record in the table as a default value.
			//
			// note: to allow the external NSForm fields to alter the table view selection through the "value" bindings,
			// added objects to the content array needs to be an "NSMutableDictionary" -
			//
			List<NSString> objects = new List<NSString> {new NSString("John"),
														new NSString("Doe"),
														new NSString("(333) 333-3333)")};

			var dict = NSMutableDictionary.FromObjectsAndKeys(objects.ToArray(), Keys.ToArray());
			myContentArray.AddObject(dict);
			
		}
		
		#endregion
		
		// 
		// Inspect our selected objects (user double-clicked them).
		// 
		// Note: this method will not get called until you make all columns in the table
		// as "non-editable".  So long as they are editable, double clicking a row will
		// cause the current cell to be editied.
		// 
		partial void inspect (NSArray sender)
		{
			NSArray selectedObjects = sender;
			Console.WriteLine("inspect");
			
			int index;
			uint numItems = selectedObjects.Count;
			for (index = 0; index < numItems; index++)
			{
				NSDictionary objectDict =  new NSDictionary(selectedObjects.ValueAt(0));

				if (objectDict != null)
				{
					Console.WriteLine(string.Format("inspector item: [ {0} {1}, {2} ]",
					                                (NSString)objectDict[FIRST_NAME].ToString(),
					                                (NSString)objectDict[LAST_NAME].ToString(),
					                                (NSString)objectDict[PHONE].ToString()));
				}
				
				// setup the edit sheet controller if one hasn't been setup already
				if (myEditController == null)
					myEditController = new EditController();
				
				// remember which selection index we are changing
				int savedSelectionIndex = myContentArray.SelectionIndex;
				
				NSDictionary editItem =  new NSDictionary(selectedObjects.ValueAt(0));
				
				// get the current selected object and start the edit sheet
				NSMutableDictionary newValues = myEditController.edit(editItem, this);

				if (!myEditController.Cancelled)
				{
					// remove the current selection and replace it with the newly edited one
					var currentObjects = myContentArray.SelectedObjects;
					myContentArray.Remove(currentObjects);
					
					// make sure to add the new entry at the same selection location as before
					myContentArray.Insert(newValues,savedSelectionIndex);
				}
			}
		}
		
		/// <summary>
		/// This method demonstrates how to observe selection changes in our NSTableView's array controller
		/// </summary>
		/// <param name="keyPath">
		/// A <see cref="NSString"/>
		/// </param>
		/// <param name="ofObject">
		/// A <see cref="NSArrayController"/>
		/// </param>
		/// <param name="change">
		/// A <see cref="NSDictionary"/>
		/// </param>
		/// <param name="context">
		/// A <see cref="IntPtr"/>
		/// </param>
		[Export("observeValueForKeyPath:ofObject:change:context:")]
		private void observeValueForKeyPath(NSString keyPath, NSArrayController ofObject, NSDictionary change, IntPtr context)
		{
			Console.Write(String.Format("Table selection changed: keyPath = {0} : ",
			                                keyPath.ToString()));
			for(uint idx = 0; idx < ofObject.SelectionIndexes.Count; idx++)
			{
				Console.Write(ofObject.SelectionIndexes.IndexGreaterThanOrEqual(idx) + " ");
			}
			Console.WriteLine();
		}

		/// <summary>
		/// Ask the edit form to display itself to enter new record values
		/// </summary>
		/// <param name="sender">
		/// A <see cref="NSButton"/>
		/// </param>
		partial void add (NSButton sender)
		{
				// setup the edit sheet controller if one hasn't been setup already
				if (myEditController == null)
					myEditController = new EditController();
				
				// ask our edit sheet for information on the record we want to add
				NSMutableDictionary newValues = myEditController.edit(null, this);

				if (!myEditController.Cancelled)
				{
					// add the new entry
					myContentArray.AddObject(newValues);
				}			
		}
		
		/// <summary>
		/// Remove an entry.
		/// </summary>
		/// <param name="sender">
		/// A <see cref="NSButton"/>
		/// </param>
		partial void remove (NSButton sender)
		{
			myContentArray.RemoveAt(myContentArray.SelectionIndex);
		}
	}
}

