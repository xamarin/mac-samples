
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace DrawerMadness
{
	public partial class ParentWindow : MonoMac.AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public ParentWindow (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public ParentWindow (NSCoder coder) : base(coder)
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

