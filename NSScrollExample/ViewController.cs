using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace NSScrollExample
{
	public partial class ViewController : NSViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			// This is going to contains our custom controls
			NSView customContentView = new NSView (new CGRect (0, 0, 2500, 2500));

			NSScrollView scrollView = new NSScrollView (View.Bounds)
			{
				HasHorizontalScroller = true, // Yes, we want scroll bars
				HasVerticalScroller = true,
				DocumentView = customContentView // And use this frame to determine where we are scrolling
			};

			View.AddSubview(scrollView);
			for (int i = 0; i < 10; ++i)
			{
				// Setup some arbitrary custom controls
				NSButton b = new NSButton(new CGRect (i * 200, i * 200, 75, 30));
				customContentView.AddSubview (b);
			}
		}
	}
}
