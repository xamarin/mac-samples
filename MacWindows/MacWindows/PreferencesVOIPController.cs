using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class PreferencesVOIPController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public PreferencesVOIPController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferencesVOIPController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public PreferencesVOIPController () : base ("PreferencesVOIP", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new PreferencesVOIP View {
			get {
				return (PreferencesVOIP)base.View;
			}
		}
	}
}
