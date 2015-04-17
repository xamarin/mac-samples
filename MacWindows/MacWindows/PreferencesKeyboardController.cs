using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class PreferencesKeyboardController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public PreferencesKeyboardController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferencesKeyboardController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public PreferencesKeyboardController () : base ("PreferencesKeyboard", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new PreferencesKeyboard View {
			get {
				return (PreferencesKeyboard)base.View;
			}
		}
	}
}
