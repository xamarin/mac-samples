using AppKit;
using Foundation;

namespace MacCopyPaste
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Computed Properties
		public int UntitledWindowCount { get; set; } = 1;
		#endregion

		#region Constructors
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Disable automatice item enabling on the Edit menu
			EditMenu.AutoEnablesItems = false;
			EditMenu.Delegate = new EditMenuDelegate ();
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
		#endregion

		#region Actions
		[Export ("newDocument:")]
		void NewDocument (NSObject sender) {
			// Get new window
			var storyboard = NSStoryboard.FromName ("Main", null);
			var controller = storyboard.InstantiateControllerWithIdentifier ("MainWindow") as NSWindowController;

			// Display
			controller.ShowWindow(this);

			// Set the title
			controller.Window.Title = (++UntitledWindowCount == 1) ? "untitled" : string.Format ("untitled {0}", UntitledWindowCount);
		}

		[Export("copy:")]
		void CopyImage (NSObject sender)
		{
			// Get the main window
			var window = NSApplication.SharedApplication.KeyWindow as ImageWindow;

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
			var window = NSApplication.SharedApplication.KeyWindow as ImageWindow;

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
			var window = NSApplication.SharedApplication.KeyWindow as ImageWindow;

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
			var window = NSApplication.SharedApplication.KeyWindow as ImageWindow;

			// Anything to do?
			if (window == null)
				return;

			// Clear image
			window.Image = null;
		}
		#endregion
	}
}

