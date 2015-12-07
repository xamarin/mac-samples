using System;
using AppKit;
using Foundation;

namespace MacWindows
{
	[Register("ReplaceViewSeque")]
	public class ReplaceViewSeque : NSStoryboardSegue
	{
		#region Constructors
		public ReplaceViewSeque() {

		}

		public ReplaceViewSeque (string identifier, NSObject sourceController, NSObject destinationController) : base(identifier,sourceController,destinationController) {

		}

		public ReplaceViewSeque (IntPtr handle) : base(handle) {
		}

		public ReplaceViewSeque (NSObjectFlag x) : base(x) {
		}
		#endregion

		#region Override Methods
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

				// Release memory
				window.ContentViewController?.RemoveFromParentViewController ();
			} else {
				// Swap the controllers
				source.View.Window.ContentViewController = destination;

				// Release memory
				source.RemoveFromParentViewController ();
			}
		
		}
		#endregion

	}

}