using System;
using AppKit;
using AppKit.TextKit.Formatter;
using System.IO;
using Foundation;

namespace SourceWriter
{
	/// <summary>
	/// Defines a delegate for handling events on our text editor window such as asking the
	/// user to save changes to a document before closing the window.
	/// </summary>
	/// <remarks>
	/// Please see our Modified Windows Content Docs for more info:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Modified_Windows_Content
	/// </remarks>
	public class EditorWidowDelegate : NSWindowDelegate
	{
		#region Application Access
		/// <summary>
		/// A helper shortcut to the app delegate.
		/// </summary>
		/// <value>The app.</value>
		public static AppDelegate App {
			get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
		}
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the window being managed.
		/// </summary>
		/// <value>The <c>NSWindow</c> being managed by the <c>NSWindowController</c> this delegate
		/// is attached to.</value>
		public NSWindow Window { get; set;}

		/// <summary>
		/// Gets the editor window controller.
		/// </summary>
		/// <value>The editor controller.</value>
		public EditorWindowController EditorController {
			get { return Window.WindowController as EditorWindowController; }
		}
		#endregion

		#region constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceWriter.EditorWidowDelegate"/> class.
		/// </summary>
		/// <param name="window">The <c>NSWindow</c> being managed by the <c>NSWindowController</c> this delegate
		/// is attached to.</param>
		public EditorWidowDelegate (NSWindow window)
		{
			// Initialize
			this.Window = window;

		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called before an <c>NSWindow</c> is closed. If the contents of the window has changed and
		/// not been saved, display a dialog allowing the user to: a) Cancel the closing, b) Close
		/// without saving, c) Save the changes to the document.
		/// </summary>
		/// <returns><c>true</c>, if the window can be closed, else <c>false</c> if it cannot.</returns>
		/// <param name="sender">The <c>NSWindowController</c> calling this method.</param>
		public override bool WindowShouldClose (Foundation.NSObject sender)
		{
			// is the window dirty?
			if (Window.DocumentEdited) {
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = "Save changes to document before closing window?",
					MessageText = "Save Document",
				};
				alert.AddButton ("Save");
				alert.AddButton ("Lose Changes");
				alert.AddButton ("Cancel");
				var result = alert.RunSheetModal (Window);

				// Take action based on resu;t
				switch (result) {
				case 1000:
					// Grab controller
					var viewController = Window.ContentViewController as ViewController;

					// Already saved?
					if (Window.RepresentedUrl != null) {
						var path = Window.RepresentedUrl.Path;

						// Save changes to file
						File.WriteAllText (path, viewController.Text);
						return true;
					} else {
						var dlg = new NSSavePanel ();
						dlg.Title = "Save Document";
						dlg.BeginSheet (Window, (rslt) => {
							// File selected?
							if (rslt == 1) {
								var path = dlg.Url.Path;
								File.WriteAllText (path, viewController.Text);
								Window.DocumentEdited = false;
								viewController.View.Window.SetTitleWithRepresentedFilename (Path.GetFileName(path));
								viewController.View.Window.RepresentedUrl = dlg.Url;
								Window.Close();
							}
						});
						return true;
					}
					return false;
				case 1001:
					// Lose Changes
					return true;
				case 1002:
					// Cancel
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Called when the window gains focus and becomes the active window.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <remarks>We are using this method to update the preview of the document and
		/// to populate the Formatting Menu with any extra commands from the document's
		/// <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/>.</remarks>
		public override void DidBecomeKey (NSNotification notification)
		{
			// Valid
			if (EditorController == null) return;

			// Populate Formatting Command menu
			EditorController.ContentController.PopulateFormattingMenu();

			// Update preview
			EditorController.ContentController.PreviewContents();
		}

		/// <summary>
		/// Called when the window loses focus and falls into the background.
		/// </summary>
		/// <param name="notification">Notification.</param>
		/// <remarks>We are using this method to remove any custom commands added
		/// to the Formatting Menu by the <see cref="AppKit.TextKit.Formatter.LanguageFormatter"/>.</remarks>
		public override void DidResignKey (NSNotification notification)
		{
			// Valid
			if (EditorController == null) return;

			// Remove this window's extra formatting commands from the menu
			EditorController.ContentController.UnpopulateFormattingMenu ();
		}
		#endregion
	}
}

