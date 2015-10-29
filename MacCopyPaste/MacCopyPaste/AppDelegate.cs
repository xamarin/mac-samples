using AppKit;
using Foundation;

namespace MacCopyPaste
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Private Variables
		MainWindowController mainWindowController;
		#endregion

		#region Computed Properties
		public int UntitledWindowCount { get; set; } = 1;
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.Window.Title = "untitled";

			// Disable automatice item enabling on the Edit menu
			EditMenu.AutoEnablesItems = false;
			EditMenu.Delegate = new EditMenuDelegate ();
		}
		#endregion

		#region Actions
		[Export ("newDocument:")]
		void NewDocument (NSObject sender)
		{
			var newWindowController = new MainWindowController ();
			newWindowController.Window.MakeKeyAndOrderFront (this);
			newWindowController.Window.Title = (++UntitledWindowCount == 1) ? "untitled" : string.Format ("untitled {0}", UntitledWindowCount);
		}

		[Export("copy:")]
		void CopyImage (NSObject sender)
		{
			// Get the main window
			var window = NSApplication.SharedApplication.KeyWindow as MainWindow;

			// Anything to do?
			if (window == null)
				return;

			// Copy the image to the clipboard
			window.Document.CopyImage (sender);
		}

		[Export("cut:")]
		void CutImage (NSObject sender)
		{
			// Get the main window
			var window = NSApplication.SharedApplication.KeyWindow as MainWindow;

			// Anything to do?
			if (window == null)
				return;
			
			// Copy the image to the clipboard
			window.Document.CopyImage (sender);

			// Clear the existing image
			window.Image = null;
		}

		[Export("paste:")]
		void PasteImage (NSObject sender)
		{
			// Get the main window
			var window = NSApplication.SharedApplication.KeyWindow as MainWindow;

			// Anything to do?
			if (window == null)
				return;
			
			// Paste the image from the clipboard
			window.Document.PasteImage (sender);
		}

		[Export("delete:")]
		void DeleteImage (NSObject sender)
		{
			// Get the main window
			var window = NSApplication.SharedApplication.KeyWindow as MainWindow;

			// Anything to do?
			if (window == null)
				return;

			// Clear image
			window.Image = null;
		}
		#endregion
	}
}

