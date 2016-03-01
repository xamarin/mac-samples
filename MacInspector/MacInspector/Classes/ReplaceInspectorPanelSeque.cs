using System;
using AppKit;
using Foundation;

namespace MacInspector
{
	/// <summary>
	/// A custom segue type that replaces the existing View Controller with the new View Controller.
	/// This is different than most segues that push and pop the new View Controller on and off
	/// the stack.
	/// </summary>
	/// <remarks>
	/// This segue type is used to manage Inspector Panels. Please see:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Inspectors
	/// https://developer.xamarin.com/guides/mac/platform-features/storyboards/quickstart/
	/// </remarks>
	[Register("ReplaceInspectorPanelSeque")]
	public class ReplaceInspectorPanelSeque : NSStoryboardSegue
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.ReplaceInspectorPanelSeque"/> class.
		/// </summary>
		public ReplaceInspectorPanelSeque() {

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.ReplaceInspectorPanelSeque"/> class.
		/// </summary>
		/// <param name="identifier">Identifier.</param>
		/// <param name="sourceController">Source controller.</param>
		/// <param name="destinationController">Destination controller.</param>
		public ReplaceInspectorPanelSeque (string identifier, NSObject sourceController, NSObject destinationController) : base(identifier,sourceController,destinationController) {

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.ReplaceInspectorPanelSeque"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ReplaceInspectorPanelSeque (IntPtr handle) : base(handle) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.ReplaceInspectorPanelSeque"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		public ReplaceInspectorPanelSeque (NSObjectFlag x) : base(x) {
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Finds the Inspector View Controller in the <c>NSSplitViewController</c> and swaps the 
		/// currently displayed Inspector Panel for the one being called in the segue.
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
				var controller = window.ContentViewController as MainSplitViewController;
				var inspector = controller.SplitViewItems [1].ViewController as InspectorViewController;
				inspector.SetInspectorPanel (destination);
			} else {
				// Swap the controllers
				var controller = source.View.Window.ContentViewController as MainSplitViewController;
				var inspector = controller.SplitViewItems [1].ViewController as InspectorViewController;
				inspector.SetInspectorPanel (destination);
			}

		}
		#endregion

	}

}
