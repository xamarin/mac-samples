using AppKit;
using Foundation;
using System;

namespace MacMenus
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Computed Properties
		public ViewController textEditor { get; set;} = null;
		#endregion

		#region Constructors
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Create a Status Bar Menu
			NSStatusBar statusBar = NSStatusBar.SystemStatusBar;

			var item = statusBar.CreateStatusItem (NSStatusItemLength.Variable);
			item.Title = "Text";
			item.HighlightMode = true;
			item.Menu = new NSMenu ("Text");

			var address = new NSMenuItem ("Address");
			address.Activated += (sender, e) => {
				phrasesAddress(address);
			};
			item.Menu.AddItem (address);

			var date = new NSMenuItem ("Date");
			date.Activated += (sender, e) => {
				phrasesDate(date);
			};
			item.Menu.AddItem (date);

			var greeting = new NSMenuItem ("Greeting");
			greeting.Activated += (sender, e) => {
				phrasesGreeting(greeting);
			};
			item.Menu.AddItem (greeting);

			var signature = new NSMenuItem ("Signature");
			signature.Activated += (sender, e) => {
				phrasesSignature(signature);
			};
			item.Menu.AddItem (signature);
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
		#endregion

		#region Custom Actions
		[Export ("openDocument:")]
		void OpenDialog (NSObject sender)
		{
			var dlg = NSOpenPanel.OpenPanel;
			dlg.CanChooseFiles = false;
			dlg.CanChooseDirectories = true;

			if (dlg.RunModal () == 1) {
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Informational,
					InformativeText = "At this point we should do something with the folder that the user just selected in the Open File Dialog box...",
					MessageText = "Folder Selected"
				};
				alert.RunModal ();
			}
		}

		partial void phrasesAddress (Foundation.NSObject sender) {

			if (textEditor == null) return;
			textEditor.Text += "Xamarin HQ\n394 Pacific Ave, 4th Floor\nSan Francisco CA 94111\n\n";
		}

		partial void phrasesDate (Foundation.NSObject sender) {

			if (textEditor == null) return;
			textEditor.Text += DateTime.Now.ToString("D");
		}

		partial void phrasesGreeting (Foundation.NSObject sender) {

			if (textEditor == null) return;
			textEditor.Text += "Dear Sirs,\n\n";
		}

		partial void phrasesSignature (Foundation.NSObject sender) {

			if (textEditor == null) return;
			textEditor.Text += "Sincerely,\n\nKevin Mullins\nXamarin,Inc.\n";
		}
		#endregion
	}
}

