using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacControls
{
	public partial class SubviewButtonsController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewButtonsController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewButtonsController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewButtonsController () : base ("SubviewButtons", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewButtons View {
			get {
				return (SubviewButtons)base.View;
			}
		}
	}
}
