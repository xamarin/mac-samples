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