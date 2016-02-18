using System;
using AppKit;
using System.IO;
using Foundation;

namespace SourceWriter
{
	/// <summary>
	/// Defines a delegate for handling events on our preference window such updating any
	/// open windows when the preference window closes.
	/// </summary>
	/// <remarks>
	/// For more information, please see our Modified Windows Content Docs:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Modified_Windows_Content
	/// </remarks>
	public class PreferenceWidowDelegate : NSWindowDelegate
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
		#endregion

		#region constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SourceWriter.PreferenceWidowDelegate"/> class.
		/// </summary>
		/// <param name="window">The <c>NSWindow</c> being managed by the <c>NSWindowController</c> this delegate
		/// is attached to.</param>
		public PreferenceWidowDelegate (NSWindow window)
		{
			// Initialize
			this.Window = window;

		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called before an <c>NSWindow</c> is closed. Applies any preference changes to all open
		/// windows in the app.
		/// </summary>
		/// <returns><c>true</c>, if the window can be closed, else <c>false</c> if it cannot.</returns>
		/// <param name="sender">The <c>NSWindowController</c> calling this method.</param>
		public override bool WindowShouldClose (Foundation.NSObject sender)
		{
			
			// Apply any changes to open windows
			App.UpdateWindowPreferences();

			return true;
		}
		#endregion
	}
}

