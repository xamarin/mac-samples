
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace CustomizeAnimation
{
	public partial class MyView : MonoMac.AppKit.NSView
	{
		float drawnLineWidth = 1;
		NSColor lineColor;
		NSBezierPath path;
		static CABasicAnimation drawnLineWidthBasicAnimation;

		public MyView (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MyView (NSCoder coder) : base(coder) {}
		
		[Export("initWithFrame:")]
		public MyView (RectangleF frame) : base (frame)
		{
			lineColor = NSColor.Blue;
			path = new NSBezierPath ();
			path.MoveTo (Bounds.Location);
			path.LineTo (new PointF (Bounds.GetMaxX (),Bounds.GetMaxY ()));
		}
		
		public override void AwakeFromNib ()
		{
			slider.FloatValue = DrawnLineWidth;
		}
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			lineColor.SetStroke ();
			path.Stroke ();
		}
		
		partial void setWidth (NSSlider sender)
		{			
			((MyView)Animator).setLineWidth(sender.FloatValue);
			//((MyView)Animator).DrawnLineWidth = sender.FloatValue;
			//this.setLineWidth(sender.FloatValue);
			//((MyView)Animator).SetValueForKeyPath((NSNumber)sender.FloatValue,(NSString)"drawnLineWidth");
			//((MyView)Animator).DrawnLineWidth = sender.FloatValue;
			//DrawnLineWidth = sender.FloatValue;
			
		}
		
		public void setLineWidth(float value)
		{
			SetValueForKeyPath((NSNumber) value,(NSString)"drawnLineWidth");
		}
		
		[Export("drawnLineWidth")]
		public float DrawnLineWidth {
			get {
				return drawnLineWidth;	
			}
			set {
				WillChangeValue ("drawnLineWidth");
				drawnLineWidth = value;
				path.LineWidth = drawnLineWidth;
				NeedsDisplay = true;				
				DidChangeValue ("drawnLineWidth");
			}
		}
		
		
		[Export ("defaultAnimationForKey:")]
		static new NSObject DefaultAnimationFor (NSString key)
		{
			if (key == "drawnLineWidth"){
				if (drawnLineWidthBasicAnimation == null) {
					drawnLineWidthBasicAnimation = new CABasicAnimation ();
					//drawnLineWidthBasicAnimation.Duration = 2.0f;
				}
				return drawnLineWidthBasicAnimation;
			} else
				return NSView.DefaultAnimationFor (key);
		}
		
		
		
	}
}

