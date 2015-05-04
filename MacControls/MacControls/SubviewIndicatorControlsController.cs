using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewIndicatorControlsController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewIndicatorControlsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewIndicatorControlsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewIndicatorControlsController () : base ("SubviewIndicatorControls", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewIndicatorControls View {
			get {
				return (SubviewIndicatorControls)base.View;
			}
		}
	}
}
