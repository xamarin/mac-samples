using AppKit;
using CoreGraphics;
using Foundation;

namespace TestFont
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Private Variables
		MainWindowController mainWindowController;
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Create an instance of the main window and display it
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

			// Create a text field with the custom font and add it to the main window
			var lab1 = new NSTextField (new CGRect (0.0, 0.0, 300.0, 100.0)) {
				StringValue = "This is some sample text",
				Editable = false,
				Font = NSFont.FromFontName ("Lobster-Regular", 20f)
			};

			mainWindowController.Window.ContentView.AddSubview (lab1);
		}
		#endregion
	}
}

