using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace CoreWLANWirelessManager
{
	public partial class JoinDialog : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public JoinDialog (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public JoinDialog (NSCoder coder) : base (coder)
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

