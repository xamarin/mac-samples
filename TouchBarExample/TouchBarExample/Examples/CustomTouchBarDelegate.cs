using System;
using AppKit;

namespace TouchBarExample
{
	public class CustomTouchBarDelegate : TouchBarExampleDelegate
	{
		public override int Count => 5;

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			NSCustomTouchBarItem item = new NSCustomTouchBarItem (identifier);

			switch (ParseId (identifier))
			{
				case 0:{
					item.View = NSButton.CreateButton ("1️⃣ Button", () => Console.WriteLine ("Button"));
					return item;
				}
				case 1: {
					item.View = NSSegmentedControl.FromLabels (new string [] { "Label1", "Label2" }, NSSegmentSwitchTracking.SelectAny, () => Console.WriteLine ("Seg Label"));
					return item;
				}
				case 2: {
					item.View = new NSImageView () {
						Image = NSImage.ImageNamed (NSImageName.TouchBarGetInfoTemplate),
					};
					return item;
				}
				case 3: {
					item.View = NSSegmentedControl.FromImages (
						new NSImage [] {
							NSImage.ImageNamed (NSImageName.TouchBarVolumeDownTemplate),
							NSImage.ImageNamed (NSImageName.TouchBarVolumeUpTemplate) },
						NSSegmentSwitchTracking.SelectAny, () => Console.WriteLine ("Seg Images"));
					return item;
				}
				case 4: {
					item.View = NSSlider.FromValue (5, 0, 10, () =>  Console.WriteLine ("Slider"));
					return item;
				}

			}
			return null;
		}
	}
}
