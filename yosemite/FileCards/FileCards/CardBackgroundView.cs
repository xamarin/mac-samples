using System;

using AppKit;
using Foundation;
using CoreGraphics;

namespace FileCards
{
	[Register ("CardBackgroundView")]
	public class CardBackgroundView : NSView
	{
		const float kRoundedRadius = 5;

		[Outlet ("representedObject")]
		public NSObject RepresentedObject { get; set; }

		public override bool IsOpaque {
			get {
				return false;
			}
		}

		public CardBackgroundView (IntPtr handle)
			: base (handle)
		{
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			NSBezierPath bezierPath = NSBezierPath.FromRoundedRect (Bounds, kRoundedRadius, kRoundedRadius);
			bezierPath.LineWidth = 1;

			NSColor.White.Set ();
			bezierPath.Fill ();
		}

		public override NSMenu MenuForEvent (NSEvent theEvent)
		{
			return null;
		}
	}
}