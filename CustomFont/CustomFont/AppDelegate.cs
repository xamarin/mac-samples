using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace TestFont
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Private Variables
		private MainWindowController mainWindowController;
		#endregion

		#region Constructors
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Create an instance of the main window and display it
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);

			// Create a text field with the custom font and add it to the main window
			var lab1 = new NSTextField(new CGRect(0,0, 300, 100));
			lab1.StringValue = "This is some sample text";
			lab1.Editable = false;
			lab1.Font = NSFont.FromFontName ("SF Hollywood Hills", 20f);
			mainWindowController.Window.ContentView.AddSubview (lab1);

		}
		#endregion
	}
}

