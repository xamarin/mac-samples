using System;

using Foundation;
using AppKit;

namespace MacMenus
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		#region Computed Properties
		public MainWindow textEditor { get; set;}
		#endregion

		#region Constructors
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

			// Save access to the main window
			textEditor = mainWindowController.Window;

			// Create a Status Bar Menu
			NSStatusBar statusBar = NSStatusBar.SystemStatusBar;

			var item = statusBar.CreateStatusItem (NSStatusItemLength.Variable);
			item.Title = "Text";
			item.HighlightMode = true;
			item.Menu = new NSMenu ("Text");

			var address = new NSMenuItem ("Address");
			address.Activated += (sender, e) => {
				PhraseAddress(address);
			};
			item.Menu.AddItem (address);

			var date = new NSMenuItem ("Date");
			date.Activated += (sender, e) => {
				PhraseDate(date);
			};
			item.Menu.AddItem (date);

			var greeting = new NSMenuItem ("Greeting");
			greeting.Activated += (sender, e) => {
				PhraseGreeting(greeting);
			};
			item.Menu.AddItem (greeting);

			var signature = new NSMenuItem ("Signature");
			signature.Activated += (sender, e) => {
				PhraseSignature(signature);
			};
			item.Menu.AddItem (signature);

		}
		#endregion

		#region Menu Handlers
		//
		// File open dialog
		//
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

		[Export ("phraseAddress:")]
		void PhraseAddress (NSObject sender) {

			textEditor.Text += "Xamarin HQ\n394 Pacific Ave, 4th Floor\nSan Francisco CA 94111\n\n";
		}

		[Export ("phraseDate:")]
		void PhraseDate (NSObject sender) {

			textEditor.Text += DateTime.Now.ToString("D");
		}

		[Export ("phraseGreeting:")]
		void PhraseGreeting (NSObject sender) {

			textEditor.Text += "Dear Sirs,\n\n";
		}

		[Export ("phraseSignature:")]
		void PhraseSignature (NSObject sender) {

			textEditor.Text += "Sincerely,\n\nKevin Mullins\nXamarin,Inc.\n";
		}
		#endregion
	}
}

