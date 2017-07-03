using System;
using AppKit;

namespace TouchBarExample
{
	public class PopoverDelegate : TouchBarExampleWithPopover
	{
		public override int Count => 2;

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			if (IsPopupID (identifier))
			{
				switch (ParseNestedId (identifier))
				{
					case 0: 
						return ColorPickerDelegate.CreateColorPicker (identifier, "Apple");
					case 1:
						return new NSCustomTouchBarItem (identifier) 
							{ View = NSButton.CreateButton ("NSPopup Button", () => Console.WriteLine ("Popup Button"))};
				}
			}
			else 
			{
				switch (ParseId (identifier)) 
				{
					case 0: 
					{
						NSPopoverTouchBarItem item = new NSPopoverTouchBarItem (identifier);
						item.PopoverTouchBar = new NSTouchBar () {
							Delegate = this,
							DefaultItemIdentifiers = new string [] { CreateNestedID (0) }
						};
						item.CollapsedRepresentationLabel = "Popup Color";
						return item;
					}
					case 1: 
					{
						NSPopoverTouchBarItem item = new NSPopoverTouchBarItem (identifier);
						item.PopoverTouchBar = new NSTouchBar () {
							Delegate = this,
							DefaultItemIdentifiers = new string [] { CreateNestedID (1) }
						};
						item.CollapsedRepresentationLabel = "Popup Text";
						return item;
					}
				}
			}
			return null;
		}
	}
}
