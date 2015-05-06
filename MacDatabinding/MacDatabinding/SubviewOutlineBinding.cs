using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacDatabinding
{
	public partial class SubviewOutlineBinding : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewOutlineBinding (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewOutlineBinding (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}
