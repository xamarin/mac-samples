using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewMenuControlsController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewMenuControlsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewMenuControlsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewMenuControlsController () : base ("SubviewMenuControls", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewMenuControls View {
			get {
				return (SubviewMenuControls)base.View;
			}
		}
	}
}
