using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class PreferencesProfile : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public PreferencesProfile (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferencesProfile (NSCoder coder) : base (coder)
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
