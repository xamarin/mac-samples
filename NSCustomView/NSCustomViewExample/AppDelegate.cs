using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace NSCustomViewExample
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();

			// We create a tab control to insert both examples into, and set it to take the entire window and resize
			RectangleF frame = mainWindowController.Window.ContentView.Frame;
			NSTabView tabView = new NSTabView (frame) {
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
			};

			NSTabViewItem firstTab = new NSTabViewItem () {
				View = new CustomDrawRectView (tabView.ContentRect),
				Label = "CustomDrawRectView"
			};
			tabView.Add (firstTab);

			NSTabViewItem secondTab = new NSTabViewItem () {
				View = new CustomLayerBasedView (tabView.ContentRect),
				Label = "CustomLayerBasedView"
			};
			tabView.Add (secondTab);

			mainWindowController.Window.ContentView.AddSubview (tabView);
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
	}

	// This is a NSView which is layer backed
	// See https://developer.apple.com/library/ios/documentation/Cocoa/Conceptual/CoreAnimation_guide/SettingUpLayerObjects/SettingUpLayerObjects.html
	// for differences between layer backed and layer hosted
	public class CustomLayerBasedView : NSView
	{
		public CustomLayerBasedView (RectangleF rect) : base (rect)
		{
			WantsLayer = true;

			CAShapeLayer shapeLayer = new CAShapeLayer ();
		
			// Create a circle path
			CGPath path = new CGPath ();
			path.AddArc (rect.Width / 2, rect.Height / 2, (rect.Height / 2) - 10, (float)(-(Math.PI / 2)), (float)(3 * Math.PI / 2), false);
			shapeLayer.Path = path;
			shapeLayer.Position = new PointF (Bounds.Width / 2, Bounds.Height / 2);
			shapeLayer.FillColor = NSColor.LightGray.CGColor;
			shapeLayer.StrokeColor = NSColor.Blue.CGColor;
			shapeLayer.LineWidth = 2;
			Layer = shapeLayer;
		}
	}

	// This is a NSView which overrides DrawRect which is repeatly called to draw its contents
	public class CustomDrawRectView : NSView
	{
		const int StrokeWidth = 4;
		const int Offset = 5;
		NSColor lineColor = NSColor.Blue;
		NSColor fillColor = NSColor.LightGray;

		public CustomDrawRectView (RectangleF rect) : base (rect)
		{
		}

		NSBezierPath CreatePathForOval (RectangleF bounds)
		{
			RectangleF rect = new RectangleF (bounds.X + Offset, bounds.Y + Offset, bounds.Width - (2 * Offset), bounds.Height - (2 * Offset));
			rect.X += StrokeWidth / 2;
			rect.Y += StrokeWidth / 2;
			rect.Size = new SizeF (rect.Size.Width - StrokeWidth / 2, rect.Size.Height - StrokeWidth / 2);
			NSBezierPath path = new NSBezierPath ();
			path.AppendPathWithOvalInRect (rect);
			path.LineWidth = StrokeWidth;
			return path;
		}

		// This is called repeatedly to draw the contents of the view. 
		// As an optimization, you can only redrawn the elements in the dirtyRect (not done here)
		public override void DrawRect (RectangleF dirtyRect)
		{
			lineColor.Set (); // You call set on each color to set it as the current global color
			var path = CreatePathForOval (this.Bounds);
			path.Stroke (); // This uses the color set above
			fillColor.Set (); // Now this is the global color
			path.Fill (); // Which is used by this fill.
		}
	}
}

