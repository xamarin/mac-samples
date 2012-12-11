
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace SearchField
{
	public partial class SearchFieldWindowController : MonoMac.AppKit.NSWindowController
	{
		
		// Search menu template tags. Special items in the search menu are tagged so when the actual dynamic search menu is constructed, we know which items to show or hide.
		
		const int	NSSearchFieldRecentsTitleMenuItemTag  =   1000;
		// Title of recents menu group. Hidden if no recents. Also use for separators that my go away with recents
		
		const int	NSSearchFieldRecentsMenuItemTag       =   1001;
		// Recent items have this tag. Use to indicate location of recents in custom menu if no title specified
		
		const int	NSSearchFieldClearRecentsMenuItemTag  =   1002;
		// The 'Clear Recents' item has this tag. Hidden if no recents
		
		const int	NSSearchFieldNoRecentsMenuItemTag     =   1003;
		// The item describing a lack of recents uses this tag. Hidden if recents
		
		NSApplication NSApp = NSApplication.SharedApplication;
		
		bool completePosting = false;
		bool commandHandling = false;
		
		List<String> builtInKeywords;
		
		// Called when created from unmanaged code
		public SearchFieldWindowController (IntPtr handle) : base(handle)
		{
		}

		// Call to load from the XIB/NIB file
		public SearchFieldWindowController () : base("SearchFieldWindow")
		{
		}

		//strongly typed window accessor
		public override void AwakeFromNib ()
		{
			// add the searchMenu to this control, allowing recent searches to be added.
			//
			// note that we could build this menu inside our nib, but for clarity we're
			// building the menu in code to illustrate the use of tags:
			//		NSSearchFieldRecentsTitleMenuItemTag, NSSearchFieldNoRecentsMenuItemTag, etc.
			//			
			if (searchField.RespondsToSelector(new Selector("setRecentSearches:")))
			{
				NSMenu searchMenu = new NSMenu("Search Menu") {
					AutoEnablesItems = true
				};
				
				var item = new NSMenuItem("Custom","",(o,e) => actionMenuItem());
				searchMenu.InsertItematIndex(item,0);
				
				var separator = NSMenuItem.SeparatorItem;
				searchMenu.InsertItematIndex(separator,1);
				
				var recentsTitleItem = new NSMenuItem("Recent Searches","");
				// tag this menu item so NSSearchField can use it and respond to it appropriately
				recentsTitleItem.Tag = NSSearchFieldRecentsTitleMenuItemTag;
				searchMenu.InsertItematIndex(recentsTitleItem,2);

				var norecentsTitleItem = new NSMenuItem("No recent searches","");
				// tag this menu item so NSSearchField can use it and respond to it appropriately
				norecentsTitleItem.Tag = NSSearchFieldNoRecentsMenuItemTag;
				searchMenu.InsertItematIndex(norecentsTitleItem,3);
				
				var recentsItem = new NSMenuItem("Recents","");
				// tag this menu item so NSSearchField can use it and respond to it appropriately
				recentsItem.Tag = NSSearchFieldRecentsMenuItemTag;
				searchMenu.InsertItematIndex(recentsItem,4);

				var separatorItem = NSMenuItem.SeparatorItem;
				// tag this menu item so NSSearchField can use it, by hiding/show it appropriately:
				separatorItem.Tag = NSSearchFieldRecentsTitleMenuItemTag;
				searchMenu.InsertItematIndex (separatorItem,5);

				var clearItem = new NSMenuItem ("Clear", "");
				// tag this menu item so NSSearchField can use it
				clearItem.Tag = NSSearchFieldClearRecentsMenuItemTag;
				searchMenu.InsertItematIndex (clearItem, 6);
				
				var searchCell = searchField.Cell;
				searchCell.MaximumRecents = 20;
				searchCell.SearchMenuTemplate = searchMenu;
				
				// with lamda
				//searchField.ControlTextDidChange += (o,e) => controlTextDidChange((NSNotification)o);
				// or delegate
				searchField.Changed += delegate (object sender, EventArgs e) {
					handleTextDidChange ((NSNotification) sender);
				};
				searchField.DoCommandBySelector = handleCommandSelectors;
				searchField.GetCompletions = handleFilterCompletions;
			}
				
			// build the list of keyword strings for our type completion dropdown list in NSSearchField
			builtInKeywords = new List<string>() {"Favorite", "Favorite1", "Favorite11", "Favorite3", "Vacations1", "Vacations2",
					"Hawaii", "Family", "Important", "Important2", "Personal"};
		}
		
		#region Custom Sheet
		
		private void actionMenuItem()
		{
			NSApp.BeginSheet (simpleSheet, this.Window);
			NSApp.RunModalForWindow (simpleSheet);
			// sheet is up here.....
			
			// when StopModal is called will continue here ....
			NSApp.EndSheet (simpleSheet);
			simpleSheet.OrderOut (this);			
		}
		
		partial void sheetDone (NSButton sender)
		{
			NSApp.StopModal();
		}
		
		#endregion
		
		#region Keyword search handling
		
		// -------------------------------------------------------------------------------
		//	control:textView:completions:forPartialWordRange:indexOfSelectedItem:
		//
		//	Use this method to override NSFieldEditor's default matches (which is a much bigger
		//	list of keywords).  By not implementing this method, you will then get back
		//	NSSearchField's default feature.
		// -------------------------------------------------------------------------------		
		//public string[] FilterCompletions (NSControl control, NSTextView textView, string [] words, NSRange charRange, int index)
		string[] handleFilterCompletions (NSControl control, NSTextView textView, string[] words, NSRange charRange, int index) 
		{
			
			var partialString = textView.Value;
			List<string> matches = new List<string> ();

			if (partialString.Length > 0) {
				// find any match in our keyword array against what was typed -
				matches = (from c in builtInKeywords
					where c.StartsWith (partialString, StringComparison.OrdinalIgnoreCase)
					orderby c select c).ToList ();
			}

			return matches.ToArray();

		}

		// -------------------------------------------------------------------------------
		//	handleTextDidChange:
		//
		//	The text in NSSearchField has changed, try to attempt type completion.
		// -------------------------------------------------------------------------------
		public void handleTextDidChange(NSNotification obj)
		{
			
			// As per the documentation: 
			//  Use the key "NSFieldEditor" to obtain the field editor from the userInfo 
			//	dictionary of the notification object
			NSTextView textView = (NSTextView)obj.UserInfo.ObjectForKey ((NSString) "NSFieldEditor");
			
			// prevent calling "complete" too often
			if (!completePosting && !commandHandling) {
				completePosting = true;
				textView.Complete(null);
				completePosting = false;
			}
			
			if (commandHandling)
				commandHandling = false;
		}

		// -------------------------------------------------------------------------------
		//	handleCommandSelectors
		//
		//	Handle all command selectors that we can handle here
		// -------------------------------------------------------------------------------		
		private bool handleCommandSelectors(NSControl control, NSTextView textView, Selector commandSelector)
		{
			
			bool result = false;
			
			if (textView.RespondsToSelector (commandSelector)){
				commandHandling = true;
				textView.PerformSelector (commandSelector,null,-1);
				//commandHandling = false;
				result = true;
			}
			
			return result;
			
		}
		
		#endregion
	}
}

