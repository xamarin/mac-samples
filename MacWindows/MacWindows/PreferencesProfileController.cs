using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class PreferencesProfileController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public PreferencesProfileController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferencesProfileController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public PreferencesProfileController () : base ("PreferencesProfile", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new PreferencesProfile View {
			get {
				return (PreferencesProfile)base.View;
			}
		}
	}
}
