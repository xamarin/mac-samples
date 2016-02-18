using System;
using AppKit;
using System.IO;
using Foundation;

namespace SourceWriter
{
	/// <summary>
	/// Defines a delegate for handling events on our text editor window such as asking the
	/// user to save changes to a document before closing the window.
	/// </summary>
	/// <remarks>
	/// For more information, please see our Modified Windows Content Docs:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Modified_Windows_Content
	/// </remarks>
	public class PreviewWindowDelegate : NSWindowDelegate
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
		/// Gets or sets the window controller.
		/// </summary>
		/// <value>The <see cref="SourceWriter.PreviewWindowController"/>.</value>
		public PreviewWindowController WindowController { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceWriter.PreviewWindowDelegate"/> class.
		/// </summary>
		/// <param name="windowController">Window controller.</param>
		public PreviewWindowDelegate (PreviewWindowController windowController)
		{
			// Initialize
			this.WindowController = windowController;

		}
		#endregion

		#region Override Methods
		/// <summary>
		/// This method is called when a windows is closed
		/// </summary>
		/// <param name="notification">The close notification..</param>
		public override void WillClose (NSNotification notification)
		{
			// Release 
			App.PreviewWindow = null;
		}

		/// <summary>
		/// This method is called before a window is closed.
		/// </summary>
		/// <returns><c>true</c>, if should close was windowed, <c>false</c> otherwise.</returns>
		/// <param name="sender">The object requesting the close.</param>
		public override bool WindowShouldClose (NSObject sender)
		{
			// Release 
			App.PreviewWindow = null;

			return true;
		}
		#endregion
	}
}

