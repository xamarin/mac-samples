using AppKit;
using Foundation;
using System.IO;
using System;

namespace SourceWriter
{
	/// <summary>
	/// The app delegate is responsible for creating our windows and listening to OS events.
	/// </summary>
	/// <remarks>See our Hello, Mac documentation for more information:
	/// https://developer.xamarin.com/guides/mac/getting_started/hello,_mac/#AppDelegate.cs</remarks>
	[Register ("AppDelegate")]
	public partial class AppDelegate : NSApplicationDelegate
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the number of the next new editor window created.
		/// </summary>
		/// <value>The new window number.</value>
		public int NewWindowNumber { get; set;} = -1;

		/// <summary>
		/// Gets or sets the system-wide shared preview window.
		/// </summary>
		/// <value>The preview window.</value>
		public PreviewWindowController PreviewWindow { get; set; }

		/// <summary>
		/// Gets or sets the preferences for the app.
		/// </summary>
		/// <value>The <see cref="SourceWriter.AppPreferences"/> for the app.</value>
		public AppPreferences Preferences { get; set; } = new AppPreferences();

		/// <summary>
		/// Gets the formatting menu.
		/// </summary>
		/// <value>The formatting menu.</value>
		public NSMenu FormattingMenu {
			get { return FormatMenu; }
		}

		/// <summary>
		/// Gets the outdent menu item.
		/// </summary>
		/// <value>The outdent <c>NSMenuItem</c>.</value>
		public NSMenuItem OutdentItem {
			get { return OutdentMenuItem; }
		}

		/// <summary>
		/// Gets the indent menu item.
		/// </summary>
		/// <value>The indent <c>NSMenuItem</c>.</value>
		public NSMenuItem IndentItem {
			get { return IndentMenuItem; }
		}

		/// <summary>
		/// Gets the definition menu item.
		/// </summary>
		/// <value>The definition <c>NSMenuItem</c>.</value>
		public NSMenuItem DefinitionItem {
			get { return DefinitionMenuItem; }
		}

		/// <summary>
		/// Gets the reformat item.
		/// </summary>
		/// <value>The reformat item.</value>
		public NSMenuItem ReformatItem {
			get { return ReformatMenuItem; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceWriter.AppDelegate"/> class.
		/// </summary>
		public AppDelegate ()
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called when the app has finished lanuching.
		/// </summary>
		/// <param name="notification">Information about the launch state.</param>
		/// <remarks>This routine is manually taking control of the Format Menu. For
		/// more information, please see our Enabling and Disabling Menus and Items
		/// documentation:
		/// https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Enabling_and_Disabling_Menus_and_Items</remarks>
		public override void DidFinishLaunching (NSNotification notification)
		{
			// Enable manual control over the items in the Format Menu
			FormatMenu.AutoEnablesItems = false;
			OutdentItem.Enabled = false;
			IndentItem.Enabled = false;
			DefinitionItem.Enabled = false;
			ReformatItem.Enabled = false;
		}

		/// <summary>
		/// Called before the app terminates to allow the app to cancel the termination
		/// based on state, such as a file not being saved.
		/// </summary>
		/// <returns>A flag stating if the app can terminate at this time.</returns>
		/// <param name="sender">A pointer to the app.</param>
		/// <remarks>For more details, see: https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Modified_Windows_Content</remarks>
		public override NSApplicationTerminateReply ApplicationShouldTerminate (NSApplication sender)
		{
			// See if any window needs to be saved first
			foreach (NSWindow window in NSApplication.SharedApplication.Windows) {
				if (window.Delegate != null && !window.Delegate.WindowShouldClose (this)) {
					// Did the window terminate the close?
					return NSApplicationTerminateReply.Cancel;
				}
			}

			// Allow normal termination
			return NSApplicationTerminateReply.Now;
		}

		/// <summary>
		/// Called before the app terminates to allow for cleanup such as saving files.
		/// </summary>
		/// <param name="notification">Information about the termination state.</param>
		/// <remarks>For more details, see: https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Modified_Windows_Content</remarks>
		public override void WillTerminate (NSNotification notification)
		{
			// Insert code here to tear down your application
		}

		/// <summary>
		/// This method is called when the user selects a file from the <c>Open Recent</c>
		/// menu item.
		/// </summary>
		/// <returns><c>true</c>, if file was opened, <c>false</c> otherwise.</returns>
		/// <param name="sender">A pointer to the app.</param>
		/// <param name="filename">The full path and filename of the file to open.</param>
		/// <remarks>For more details, see: https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Working_with_the_Open_Recent_Menu</remarks>
		public override bool OpenFile (NSApplication sender, string filename)
		{
			// Trap all errors
			try {
				// Escape any spaces (" ") or they will cause an error
				// when converted to an NSUrl.
				filename = filename.Replace (" ", "%20");
				var url = new NSUrl ("file://"+filename);
				return OpenFile(url);
			} catch {
				return false;
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Attempts to open the file at the specified URL. If the file is currently open
		/// in a window, that will be brought to the front and selected instead of opening
		/// another copy of the file.
		/// </summary>
		/// <returns><c>true</c>, if file was opened, <c>false</c> otherwise.</returns>
		/// <param name="url">A <c>NSUrl</c> pointing to the file to open.</param>
		private bool OpenFile(NSUrl url) {
			var good = false;

			// Trap all errors
			try {
				var path = url.Path;

				// Is the file already open?
				foreach (NSWindow window in NSApplication.SharedApplication.Windows)  {
					var content = window.ContentViewController as ViewController;
					if (content != null && path == content.FilePath) {
						// Bring window to front
						window.MakeKeyAndOrderFront(this);
						return true;
					}
				}

				// Get new window
				var storyboard = NSStoryboard.FromName ("Main", null);
				var controller = storyboard.InstantiateControllerWithIdentifier ("MainWindow") as NSWindowController;

				// Display
				controller.ShowWindow(this);

				// Load the text into the window
				var viewController = controller.Window.ContentViewController as ViewController;
				viewController.Text = File.ReadAllText(path);
				viewController.SetLanguageFromPath(path);
				viewController.View.Window.SetTitleWithRepresentedFilename (Path.GetFileName(path));
				viewController.View.Window.RepresentedUrl = url;

				// Add document to the Open Recent menu
				NSDocumentController.SharedDocumentController.NoteNewRecentDocumentURL(url);

				// Make as successful
				good = true;
			} catch {
				// Mark as bad file on error
				good = false;
			}

			// Return results
			return good;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// For any open windows, redefine the language to pick up any changes to the user
		/// preferences and reformat all of the text in the document.
		/// </summary>
		/// <remarks>For more details, see: https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog</remarks>
		public void UpdateWindowPreferences() {

			// Process all open windows
			for(int n=0; n<NSApplication.SharedApplication.Windows.Length; ++n) {
				var content = NSApplication.SharedApplication.Windows[n].ContentViewController as ViewController;
				if (content != null ) {
					// Reformat all text
					content.ReformatText(true);
				}
			}

		}
		#endregion

		#region Actions
		/// <summary>
		/// Displays the Open File dialog box allowing the user to select a file to open.
		/// </summary>
		/// <param name="sender">A pointer to the app.</param>
		/// <remarks>For more details, see: https://developer.xamarin.com/guides/mac/user-interface/working-with-menus/#Built-In_Menu_Functionality</remarks>
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
					// Open the document in a new window
					OpenFile (url);
				}
			}
		}
		#endregion
	}
}

