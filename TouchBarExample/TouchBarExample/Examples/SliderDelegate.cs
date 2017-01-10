using System;
using AppKit;

namespace TouchBarExample
{
	public class SliderDelegate : TouchBarExampleDelegate
	{
		public override int Count => 2;

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			switch (ParseId (identifier)) {
				case 0: {
					var item = new NSSliderTouchBarItem (identifier) {
						MinimumValueAccessory = NSSliderAccessory.CreateAccessory (NSImage.ImageNamed (NSImageName.GoLeftTemplate)),
						MaximumValueAccessory = NSSliderAccessory.CreateAccessory (NSImage.ImageNamed (NSImageName.GoRightTemplate)),
					};
					item.Activated += (sender, e) => {
						NSSliderTouchBarItem activatedItem = (NSSliderTouchBarItem)sender;
						Console.WriteLine ("Position: " + activatedItem.Slider.DoubleValue);
					};
					return item;
				}
				case 1: {
						var item = new NSSliderTouchBarItem (identifier);
						return item;
					}
				}
			return null;
		}
	}
}
