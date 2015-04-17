using System;

using Foundation;
using AppKit;

namespace MacMenus
{
	public partial class MainWindowController : NSWindowController
	{
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			var n = new NSMenu ("Open");
			n.AutoEnablesItems = false;
		}

		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
	}
}
