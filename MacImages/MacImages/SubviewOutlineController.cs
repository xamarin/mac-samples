using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacImages
{
	public partial class SubviewOutlineController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewOutlineController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewOutlineController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewOutlineController () : base ("SubviewOutline", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewOutline View {
			get {
				return (SubviewOutline)base.View;
			}
		}
	}
}
