using System;
using AppKit;

namespace TouchBarExample
{
	public class CustomizeBarDelegate : TouchBarExampleDelegate
	{
		public override int Count => 4;
		public override bool AllowCustomization => true;

		public override NSTouchBarItem MakeItem (NSTouchBar touchBar, string identifier)
		{
			NSCustomTouchBarItem item = new NSCustomTouchBarItem (identifier);
			item.CustomizationLabel = "Bar " + ParseId (identifier);
			switch (ParseId (identifier)) {
				case 0: {
					item.View = NSButton.CreateButton ("Custom 1", () => Console.WriteLine ("Custom First"));
					return item;
				}
				case 1: {
					item.View = NSButton.CreateButton ("Custom 2", () => Console.WriteLine ("Custom Second"));
					return item;
				}
				case 2: {
					item.View = NSButton.CreateButton ("Custom 3", () => Console.WriteLine ("Custom Third"));
					return item;
				}
				case 3: {
					item.View = NSButton.CreateButton ("Custom 4", () => Console.WriteLine ("Custom Fourth"));
					return item;
				}
			}
			return null;
		}
	}
}
