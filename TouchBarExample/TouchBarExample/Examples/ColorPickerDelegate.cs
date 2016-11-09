using System;
using AppKit;
using Foundation;

namespace TouchBarExample
{
	public class ColorPickerDelegate : TouchBarExampleDelegate
	{
		public override int Count => 5;

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			switch (ParseId (identifier)) {
				case 0: 
					return CreateColorPicker (identifier, "Apple");
				case 1:
					return CreateColorPicker (identifier, "System");
				case 2:
					return CreateColorPicker (identifier, "Crayons");
				case 3:
					return CreateColorPicker (identifier, "Text");
				case 4:
					return CreateColorPicker (identifier, "Stroke");
			}
			return null;
		}

		public static NSColorPickerTouchBarItem CreateColorPicker (string identifier, string listName)
		{
			NSColorPickerTouchBarItem item;
			switch (listName) {
				case "Text":
					item = NSColorPickerTouchBarItem.CreateTextColorPicker (identifier);
					break;
				case "Stroke":
					item = NSColorPickerTouchBarItem.CreateStrokeColorPicker (identifier);
					break;
				default:
					item = NSColorPickerTouchBarItem.CreateColorPicker (identifier, NSImage.ImageNamed (NSImageName.TouchBarColorPickerFill));
					item.ColorList = NSColorList.ColorListNamed (listName);
					item.ShowsAlpha = true;
					break;
			}
			item.Activated += (sender, e) => OnColorChange (item.CustomizationLabel, item.Color);
			return item;
		}

		static void OnColorChange (string name, NSColor color)
		{
			// Prevent crashes when asking for components
			color = color.UsingColorSpace (NSColorSpace.CalibratedRGB);
			Console.WriteLine ("Color changed on {0} ({1}, {2}, {3}, {4})", name, color.RedComponent, color.GreenComponent, color.BlueComponent, color.AlphaComponent);
		}
	}
}
