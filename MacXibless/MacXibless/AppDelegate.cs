using System;

using Foundation;
using AppKit;

namespace MacXibless
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);


			// Create a Status Bar Menu
			NSStatusBar statusBar = NSStatusBar.SystemStatusBar;

			var item = statusBar.CreateStatusItem (NSStatusItemLength.Variable);
			item.Title = "Phrases";
			item.HighlightMode = true;
			item.Menu = new NSMenu ("Phrases");

			var address = new NSMenuItem ("Address");
			address.Activated += (sender, e) => {
				Console.WriteLine("Address Selected");
			};
			item.Menu.AddItem (address);

			var date = new NSMenuItem ("Date");
			date.Activated += (sender, e) => {
				Console.WriteLine("Date Selected");
			};
			item.Menu.AddItem (date);

			var greeting = new NSMenuItem ("Greeting");
			greeting.Activated += (sender, e) => {
				Console.WriteLine("Greetings Selected");
			};
			item.Menu.AddItem (greeting);

			var signature = new NSMenuItem ("Signature");
			signature.Activated += (sender, e) => {
				Console.WriteLine("Signature Selected");
			};
			item.Menu.AddItem (signature);
		}
	}
}

