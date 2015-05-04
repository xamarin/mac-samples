using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewContentViews : AppKit.NSView
	{
		#region Private Variables
		private bool PopoverShown = false;
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewContentViews (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewContentViews (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion

		#region Override Methods

		#endregion

		#region Actions
		partial void DisplayPopover (Foundation.NSObject sender) {

			var button = sender as NSButton;

			if (PopoverShown) {
				Popover.Close();
				PopoverShown = false;
			} else {
				Popover.Show(button.Bounds, button, NSRectEdge.MaxXEdge);
				PopoverShown = true;
			}
		}
		#endregion
	}
}
