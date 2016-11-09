using System;
using AppKit;
using Foundation;

namespace TouchBarExample
{
	public class SharingDelegate : TouchBarExampleDelegate, INSSharingServicePickerTouchBarItemDelegate
	{
		public override int Count => 1;

		public INSPasteboardWriting [] ItemsForSharingServicePickerTouchBarItem (NSSharingServicePickerTouchBarItem pickerTouchBarItem)
		{
			return new INSPasteboardWriting [] {
				(NSString)"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua"
			};
		}

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			NSSharingServicePickerTouchBarItem item = new NSSharingServicePickerTouchBarItem (identifier);
			item.ButtonTitle = "Share!";
			item.ButtonImage = NSImage.ImageNamed (NSImageName.ShareTemplate);
			item.Delegate = this;
			return item;
		}
	}
}
