/// Converted from Apple Sample of the same name by Kenneth J. Pouncey 2010/11/22

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace PredicateEditorSample
{
	
	public partial class MyWindowController : MonoMac.AppKit.NSWindowController {
		NSMetadataQuery query;
		int previousRowCount = 0;
		
		NSApplication NSApp = NSApplication.SharedApplication;

		NSString emptyStr = new NSString ();
		
		public MyWindowController (IntPtr handle) : base (handle) {}
		public MyWindowController () : base ("MyWindow") {} 
		
		public override void AwakeFromNib ()
		{
			// no vertical scrolling, we always want to show all rows
			predicateEditor.EnclosingScrollView.HasVerticalScroller = true;
			
			previousRowCount = 3;
			predicateEditor.AddRow (this);
			
			// put the focus in the first text field
			var displayValue = new List<NSObject> (predicateEditor.DisplayValues (1)).Last ();
			if (displayValue is NSControl)
				this.Window.MakeFirstResponder ((NSResponder)displayValue);
			
			// create and initalize our query
			query = new NSMetadataQuery ();
			
			// setup our Spotlight notifications 
			var nf = NSNotificationCenter.DefaultCenter;
			nf.AddObserver (this, new Selector ("queryNotification:"), null, query);
			
			// initialize our Spotlight query, sort by contact name
			query.SortDescriptors = new NSSortDescriptor [] { new NSSortDescriptor ("kMDItemContactKeywords",true) };
			
			// start with our progress search label empty
			progressSearchLabel.StringValue = "";
			 
			
		}
		
		[Export ("queryNotification:")]
		public void queryNotification (NSNotification note) 
		{
			// the NSMetadataQuery will send back a note when updates are happening. By looking at the [note name], we can tell what is happening

			// the query has just started
			if (note.Name == NSMetadataQuery.DidStartGatheringNotification) {
				Console.WriteLine ("search: started gathering");
				progressSearch.Hidden = false;
				progressSearch.StartAnimation (this);
				progressSearchLabel.StringValue = "Searching....";
			}

			// at this point, the query will be done. You may recieve an update later on.
			if (note.Name == NSMetadataQuery.DidFinishGatheringNotification) {
				Console.WriteLine ("search: finished gathering");
				progressSearch.Hidden = true;
				progressSearch.StopAnimation (this);
				
				loadResultsFromQuery (note);
			} 

			// the query is still gathering results...
			if (note.Name == NSMetadataQuery.GatheringProgressNotification){
				Console.WriteLine ("search: progressing....");
				progressSearch.StartAnimation (this);

			}

			// an update will happen when Spotlight notices that a file as added, removed, or modified that affected the search results.
			if (note.Name == NSMetadataQuery.DidUpdateNotification)
				Console.WriteLine ("search: an updated happened.");
		}
		
		// -------------------------------------------------------------------------------
		//	inspect:selectedObjects
		//
		//	This method obtains the selected object (in our case for single selection,
		//	it's the first item), and opens its URL.
		// -------------------------------------------------------------------------------
		[Export ("inspect:")]
		private void inspect (NSArray selectedObjects)
		{
			NSDictionary objectDict = new NSDictionary (selectedObjects.ValueAt (0));
			
			if (objectDict != null) {
				NSString sss = new NSString ("url");
				NSUrl url = new NSUrl (objectDict.ValueForKey (sss).ToString ());
				NSWorkspace.SharedWorkspace.OpenUrl (url);                      
			}
		}
		
		private void loadResultsFromQuery (NSNotification notif)
		{
			var results = new List<NSMetadataItem> (((NSMetadataQuery)notif.Object).Results);
			
			Console.WriteLine ("search count = {0}",results.Count);
			string foundResultStr = string.Format ("Results found: {0}" , results.Count);
			progressSearchLabel.StringValue = foundResultStr;
			
			// iterate through the array of results, and match to the existing stores
			foreach (NSMetadataItem item in results) {
				
				var cityStr = item.ValueForAttribute ("kMDItemCity");
				var nameStr = item.ValueForAttribute ("kMDItemDisplayName");
				var stateStr = item.ValueForAttribute ("kMDItemStateOrProvince");
				
				NSArray phoneNumbers = (NSArray)item.ValueForAttribute ("kMDItemPhoneNumbers");
				// grab only the first phone number
				var phoneStr = phoneNumbers == null ? null : new NSString (phoneNumbers.ValueAt (0));
				var storePath = (NSString) item.ValueForAttribute ("kMDItemPath");
				
				// create a dictionary entry to be added to our search results array
				NSObject [] objects = new NSObject [] {
					nameStr  ?? emptyStr,
					phoneStr ?? emptyStr,
					cityStr  ?? emptyStr,
					stateStr ?? emptyStr,
					new NSUrl (storePath, false) };
				
				var keys = new object [] {
					"name", "phone", "city", "state", "url" 
				};
				NSDictionary dict = NSDictionary.FromObjectsAndKeys (objects, keys);
				mySearchResults.AddObject (dict);
			}
		}
		
		// -------------------------------------------------------------------------------
		//	predicateEditorChanged:sender
		//
		//  This method gets called whenever the predicate editor changes.
		//	It is the action of our predicate editor and the single plate for all our updates.
		//	
		//	We need to do potentially three things:
		//		1) Fire off a search if the user hits enter.
		//		2) Add some rows if the user deleted all of them, so the user isn't left without any rows.
		//		3) Resize the window if the number of rows changed (the user hit + or -).
		// -------------------------------------------------------------------------------
		partial void predicateEditorChanged (NSObject sender)
		{
			// check NSApp currentEvent for the return key
			NSEvent currentEvent = NSApp.CurrentEvent;
			if (currentEvent != null && currentEvent.Type == NSEventType.KeyDown) {
				string characters = currentEvent.Characters;
				
				if (characters.Length > 0 && characters.First () == 0x0D) {
					// get the predicate, which is the object value of our view
					NSPredicate predicate = (NSPredicate)predicateEditor.ObjectValue;
					
					// make it Spotlight friendly
					predicate = this.spotlightFriendlyPredicate (predicate);
					if (predicate != null) {
						int searchIndex = 0;
						string title = string.Format ("Search #{0}",searchIndex);
						this.createNewSearchForPredicate (predicate,title);
					}
				}
					
			}
			
			// if the user deleted the first row, then add it again - no sense leaving the user with no rows
			if (predicateEditor.NumberOfRows == 0)
				predicateEditor.AddRow (this);
    		
			// resize the window vertically to accomodate our views:
        
			// get the new number of rows, which tells us the needed change in height,
			// note that we can't just get the view frame, because it's currently animating - this method is called before the animation is finished.
			var newRowCount = predicateEditor.NumberOfRows;
			
			// if there's no change in row count, there's no need to resize anything
			if (newRowCount == previousRowCount)
				return;
			
			// The autoresizing masks, by default, allows the NSTableView to grow and keeps the predicate editor fixed.
			// We need to temporarily grow the predicate editor, and keep the NSTableView fixed, so we have to change the autoresizing masks.
			// Save off the old ones; we'll restore them after changing the window frame.
			var tableScrollView = myTableView.EnclosingScrollView;
			var oldOutlineViewMask = tableScrollView.AutoresizingMask;

			var predicateEditorScrollView = predicateEditor.EnclosingScrollView;
			var oldPredicateEditorViewMask = predicateEditorScrollView.AutoresizingMask;
			
			tableScrollView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.MaxYMargin;
			predicateEditorScrollView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
			
			// determine if we need to grow or shrink the window
			bool growing = (newRowCount > previousRowCount);
			
			// if growing, figure out by how much.  Sizes must contain nonnegative values, which is why we avoid negative floats here.
			float heightDifference = Math.Abs (predicateEditor.RowHeight * (newRowCount - previousRowCount));
			
			// convert the size to window coordinates -
			// if we didn't do this, we would break under scale factors other than 1.
			// We don't care about the horizontal dimension, so leave that as 0.
			SizeF sizeChange = predicateEditor.ConvertSizeToView (new SizeF (0.0f, heightDifference), null);
			
			// offset our status view
			RectangleF frame = progressView.Frame;
			progressView.SetFrameOrigin (new PointF (frame.Location.X, frame.Location.Y - predicateEditor.RowHeight * (newRowCount - previousRowCount)));
			
			// change the window frame size:
			// - if we're growing, the height goes up and the origin goes down (corresponding to growing down).
			// - if we're shrinking, the height goes down and the origin goes up.
			RectangleF windowFrame = this.Window.Frame;
			
			SizeF size = windowFrame.Size;
			size.Height += growing ? sizeChange.Height : -sizeChange.Height;
			windowFrame.Size = size;
			
			PointF origin = windowFrame.Location;
			origin.Y -= growing ? sizeChange.Height : -sizeChange.Height;
			windowFrame.Location = origin;

			this.Window.SetFrame (windowFrame, true, true);

			// restore the autoresizeing masks
			tableScrollView.AutoresizingMask = oldOutlineViewMask;
			predicateEditorScrollView.AutoresizingMask = oldPredicateEditorViewMask;
	   
			previousRowCount = newRowCount; // save our new row count

		}
		
		// -------------------------------------------------------------------------------
		//	spotlightFriendlyPredicate:predicate
		//
		//	This method will "clean up" an NSPredicate to make it ready for Spotlight, or return nil if the predicate can't be cleaned.
		//
		//	Foundation's Spotlight support in NSMetdataQuery places the following requirements on an NSPredicate:
		//		- Value-type (always YES or NO) predicates are not allowed
		//		- Any compound predicate (other than NOT) must have at least two subpredicates
		// -------------------------------------------------------------------------------
		private NSPredicate spotlightFriendlyPredicate (NSPredicate predicate) 
		{
			if (predicate.Equals (NSPredicate.FromValue (true)) || predicate.Equals (NSPredicate.FromValue (false)))
				return null;
			
			if (predicate is NSCompoundPredicate) {
				NSCompoundPredicate compoundPredicate = predicate as NSCompoundPredicate;
				NSCompoundPredicateType type = compoundPredicate.Type; 
				
				List<NSPredicate> cleanSubPredicates = new List<NSPredicate> ();
				
				foreach (var dirtySubpredicate in compoundPredicate.Subpredicates) {
					NSPredicate cleanSubPredicate = this.spotlightFriendlyPredicate (dirtySubpredicate);
					if (cleanSubPredicate != null)
						cleanSubPredicates.Add (cleanSubPredicate);
				}
				
				if (cleanSubPredicates.Count == 0)
					return null;
				
				if (cleanSubPredicates.Count == 1 && type != NSCompoundPredicateType.Not)
					return cleanSubPredicates.First ();
				else
					return new NSCompoundPredicate (type,cleanSubPredicates.ToArray ());
			} else
				return predicate;	
		}
		
		private void createNewSearchForPredicate (NSPredicate predicate, string title)
		{
			if (predicate == null)
				return;
					
			// remove the old search results.
			mySearchResults.Remove (mySearchResults.ArrangedObjects ());
			
			// always search for items in the Address Book
			//NSPredicate addrBookPredicate = NSPredicate.FromFormat (" (kMDItemKind == 'Address Book Person Data')",new NSObject[0]);
			NSPredicate addrBookPredicate = NSPredicate.FromFormat (" (kMDItemContentType == 'com.apple.addressbook.person')",new NSObject[0]);
			predicate = NSCompoundPredicate.CreateAndPredicate (new NSPredicate[2] {addrBookPredicate, predicate});
			
			// set the query predicate....
			query.Predicate = predicate;
			
			// and send it off for processing...
			query.StartQuery ();
		}
		
		//  Not used in the program was just a test for the Delegate Class
		public NSObject MetadataQueryReplacementObjectForResultObject (NSMetadataQuery query, NSMetadataItem result)
		{
			Console.WriteLine ("delegate object");
			return null;
		}

		//  Not used in the program was just a test for the Delegate Class
		public NSObject MetadataQueryReplacementValueForAttributevalue (NSMetadataQuery query, string attrName, NSObject attrValue)
		{
			Console.WriteLine ("delegate value");
			return null;
		}
	}
}

