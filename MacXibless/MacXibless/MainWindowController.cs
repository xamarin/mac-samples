using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace MacXibless
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
			// Construct the window from code here
			CGRect contentRect = new CGRect (0, 0, 1000, 500);
			base.Window = new MainWindow(contentRect, (NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable | NSWindowStyle.Resizable), NSBackingStore.Buffered, false);


			// Simulate Awaking from Nib
			Window.AwakeFromNib ();
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
	}
}
