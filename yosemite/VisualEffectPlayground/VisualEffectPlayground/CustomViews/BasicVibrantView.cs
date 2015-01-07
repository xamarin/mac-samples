using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace VisualEffectPlayground
{
	[Register ("BasicVibrantView")]
	public class BasicVibrantView : NSView
	{
		public override bool AllowsVibrancy {
			get {
				return true;
			}
		}

		public BasicVibrantView (IntPtr handle) : base (handle)
		{
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			NSColor.SecondaryLabelColor.Set ();
			NSGraphics.FrameRectWithWidth (Bounds, 10);
		}
	}
}

