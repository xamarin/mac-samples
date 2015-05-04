using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewTextControlsController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewTextControlsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTextControlsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewTextControlsController () : base ("SubviewTextControls", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewTextControls View {
			get {
				return (SubviewTextControls)base.View;
			}
		}
	}
}
