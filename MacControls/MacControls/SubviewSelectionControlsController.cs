using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewSelectionControlsController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewSelectionControlsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewSelectionControlsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewSelectionControlsController () : base ("SubviewSelectionControls", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewSelectionControls View {
			get {
				return (SubviewSelectionControls)base.View;
			}
		}
	}
}
