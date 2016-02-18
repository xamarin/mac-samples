using System;
using AppKit;
using Foundation;

namespace AppKit
{
	/// <summary>
	/// A custom segue type that replaces the existing View Controller with the new View Controller.
	/// This is different than most view controllers that push and pop the new View Controller on and off
	/// the stack.
	/// </summary>
	/// <remarks>This segue type is good for creating Preference Windows. See our Preference Window
	/// documentation for more details:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-dialogs/#Creating_a_Preferences_Dialog</remarks>
	[Register("ReplaceViewSeque")]
	public class ReplaceViewSeque : NSStoryboardSegue
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AppKit.ReplaceViewSeque"/> class.
		/// </summary>
		public ReplaceViewSeque() {

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppKit.ReplaceViewSeque"/> class.
		/// </summary>
		/// <param name="identifier">Identifier.</param>
		/// <param name="sourceController">Source controller.</param>
		/// <param name="destinationController">Destination controller.</param>
		public ReplaceViewSeque (string identifier, NSObject sourceController, NSObject destinationController) : base(identifier,sourceController,destinationController) {

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppKit.ReplaceViewSeque"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ReplaceViewSeque (IntPtr handle) : base(handle) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AppKit.ReplaceViewSeque"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		public ReplaceViewSeque (NSObjectFlag x) : base(x) {
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Removes the Source View Controller from the stack and replaces it with the 
		/// Destination View Controller.
		/// </summary>
		public override void Perform ()
		{
			// Cast the source and destination controllers
			var source = SourceController as NSViewController;
			var destination = DestinationController as NSViewController;

			// Is there a source?
			if (source == null) {
				// No, get the current key window
				var window = NSApplication.SharedApplication.KeyWindow;

				// Swap the controllers
				window.ContentViewController = destination;

				// Release reference to previous controller
				window.ContentViewController?.RemoveFromParentViewController ();
			} else {
				// Swap the controllers
				source.View.Window.ContentViewController = destination;

				// Release Reference to previous controller
				source.RemoveFromParentViewController ();
			}

		}
		#endregion

	}

}
