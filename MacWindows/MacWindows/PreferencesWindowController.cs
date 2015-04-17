using System;

using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class PreferencesWindowController : NSWindowController
	{
		public PreferencesWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public PreferencesWindowController (NSCoder coder) : base (coder)
		{
		}

		public PreferencesWindowController () : base ("PreferencesWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public new PreferencesWindow Window {
			get { return (PreferencesWindow)base.Window; }
		}
	}
}
