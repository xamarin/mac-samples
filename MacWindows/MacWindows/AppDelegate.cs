using System;

using Foundation;
using AppKit;
using System.IO;

namespace MacWindows
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;
		public int UntitledWindowCount { get; set;} =1;
		public bool ShowSaveAsSheet { get; set;} = true;

		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			// Open main window
			mainWindowController = new MainWindowController ();
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.Window.Title = "untitled";

			// Display panel
			var panel = new DocumentPanelController ();
			panel.Window.MakeKeyAndOrderFront (this);
		}

		#region Menu Handlers
		[Export ("newDocument:")]
		void NewDocument (NSObject sender) {
			var newWindowController = new MainWindowController ();
			newWindowController.Window.MakeKeyAndOrderFront (this);
			newWindowController.Window.Title = (++UntitledWindowCount == 1) ? "untitled" : string.Format ("untitled {0}", UntitledWindowCount);
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

					// Create a new window to hold the text
					var newWindowController = new MainWindowController ();
					newWindowController.Window.MakeKeyAndOrderFront (this);

					// Load the text into the window
					var window = newWindowController.Window as MainWindow;
					window.Text = File.ReadAllText(path);
					window.SetTitleWithRepresentedFilename (Path.GetFileName(path));
					window.RepresentedUrl = url;

				}
			}

		}

		[Export("applicationPreferences:")]
		void ShowPreferences (NSObject sender)
		{
			var preferences = new PreferencesWindowController ();
			preferences.Window.MakeKeyAndOrderFront (this);
		}

		[Export("saveDocumentAs:")]
		void ShowSaveAs (NSObject sender)
		{
			var dlg = new NSSavePanel ();
			dlg.Title = "Save Text File";

			if (ShowSaveAsSheet) {
				dlg.BeginSheet(mainWindowController.Window,(result) => {
					var alert = new NSAlert () {
						AlertStyle = NSAlertStyle.Critical,
						InformativeText = "We need to save the document here...",
						MessageText = "Save Document",
					};
					alert.RunModal ();
				});
			} else {
				if (dlg.RunModal () == 1) {
					var alert = new NSAlert () {
						AlertStyle = NSAlertStyle.Critical,
						InformativeText = "We need to save the document here...",
						MessageText = "Save Document",
					};
					alert.RunModal ();
				}
			}

		}
		#endregion
	}
}

