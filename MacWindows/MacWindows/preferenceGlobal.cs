using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class preferenceGlobal : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public preferenceGlobal (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public preferenceGlobal (NSCoder coder) : base (coder)
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
