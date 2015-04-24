using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacImages
{
	public partial class SubviewTableController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewTableController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTableController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewTableController () : base ("SubviewTable", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new SubviewTable View {
			get {
				return (SubviewTable)base.View;
			}
		}
	}
}
