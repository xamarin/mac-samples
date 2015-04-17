using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class preferenceGlobalController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public preferenceGlobalController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public preferenceGlobalController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public preferenceGlobalController () : base ("preferenceGlobal", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new preferenceGlobal View {
			get {
				return (preferenceGlobal)base.View;
			}
		}
	}
}
