using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace VisualEffectPlayground
{
	/*
	 * Implementation for two custom views that demonstrate vibrant drawing when inside a vibrant appearance. 
	 * CustomView does normal drawing, while CustomView2 utilizes named system colors and draws significantly 
	 * differently when the appearance is not vibrant.
	 */

	[Register ("CustomView")]
	public class CustomView : NSView
	{
		public override bool AllowsVibrancy {
			get {
				return true;
			}
		}

		public override bool IsFlipped {
			get {
				return true;
			}
		}

		public CustomView (IntPtr handle) : base (handle)
		{
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			/* 
			 * Things look good no matter where the view is located; either a vibrant appearance or 
			 * a non-vibrant appearance. Since the view says YES to allowsVibrancy, everything drawn in 
			 * drawRect will be vibrant; all colors, images, etc.
			 */

			NSColor.FromDeviceWhite (0f, 0.85f).Set ();
			NSBezierPath path = NSBezierPath.FromOvalInRect (Bounds.Inset (5f, 5f));
			path.LineWidth = 5;
			path.Stroke ();

			NSColor.FromDeviceWhite (0f, 0.48f).Set ();
			path = NSBezierPath.FromOvalInRect (Bounds.Inset (10f, 10f));
			path.Fill ();
		}
	}

	[Register ("CustomView2")]
	public class CustomView2 : NSView, INSAppearanceCustomization
	{
		public override bool AllowsVibrancy {
			get {
				return true;
			}
		}

		public override bool IsFlipped {
			get {
				return true;
			}
		}

		public CustomView2 (IntPtr handle) : base (handle)
		{
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			//TODO: uncomment code https://bugzilla.xamarin.com/show_bug.cgi?id=24144
//			if (EffectiveAppearance.AllowsVibrancy) {
//				// Vibrant drawing codepath.
//				NSColor.LabelColor.Set ();
//				NSBezierPath path = NSBezierPath.FromOvalInRect (Bounds.Inset (5, 5));
//				path.LineWidth = 5;
//				path.Stroke ();
//
//				NSColor.SecondaryLabelColor.Set ();
//				path = NSBezierPath.FromOvalInRect (Bounds.Inset (10, 10));
//				path.Fill ();
//			} else {
			NSColor.Red.Set ();
			NSBezierPath path = NSBezierPath.FromOvalInRect (Bounds.Inset (5f, 5f));
			path.LineWidth = 5;
			path.Stroke ();

			NSColor.Purple.Set ();
			path = NSBezierPath.FromOvalInRect (Bounds.Inset (10f, 10f));
			path.Fill ();
//			}
		}
	}
}

