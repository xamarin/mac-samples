using AppKit;
using Foundation;
using System.IO;

namespace MacWindows
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Computed Properties
		public int UntitledWindowCount { get; set;} =1;
		#endregion

		#region Constructors
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Insert code here to initialize your application
		}

		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}
		#endregion


		#region Custom Actions
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

		[Export ("openDocument:")]
		void OpenDialog (NSObject sender)
		{
			var dlg = NSOpenPanel.OpenPanel;
			dlg.CanChooseFiles = true;
			dlg.CanChooseDirectories = false;

			if (dlg.RunModal () == 1) {
				// Nab the first file
				var url = dlg.Urls [0];

				if (url != null) {
					var path = url.Path;

					// Get new window
					var storyboard = NSStoryboard.FromName ("Main", null);
					var controller = storyboard.InstantiateControllerWithIdentifier ("MainWindow") as NSWindowController;

					// Display
					controller.ShowWindow(this);

					// Load the text into the window
					var viewController = controller.Window.ContentViewController as ViewController;
					viewController.Text = File.ReadAllText(path);
					viewController.View.Window.SetTitleWithRepresentedFilename (Path.GetFileName(path));
					viewController.View.Window.RepresentedUrl = url;

				}
			}

		}
		#endregion

	}
}

