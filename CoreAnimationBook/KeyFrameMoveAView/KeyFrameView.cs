
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace KeyFrameMoveAView
{
	public partial class KeyFrameView : MonoMac.AppKit.NSView
	{
		NSImageView mover;
		CGPath heartPath;
		
		[Export("initWithFrame:")]
		public KeyFrameView(RectangleF frame) : base(frame)
		{
			float xInset = 3 * (frame.Width / 8);
			float yInset = 3 * (frame.Height / 8);
			
			RectangleF moverFrame = frame.Inset (xInset, yInset);

			mover = new NSImageView (moverFrame);
			
			mover.ImageScaling = NSImageScale.AxesIndependently;
			mover.Image = NSImage.ImageNamed ("photo.jpg");
			AddSubview (mover);
			addBounceAnimation ();
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			bounce ();
		}
		
		public override void SetFrameOrigin (PointF newOrigin)
		{
			Console.WriteLine ("setting new origin");
			base.SetFrameOrigin (newOrigin);
		}
		
		private void bounce()
		{
			RectangleF rect = mover.Frame;
			((NSView)mover.Animator).SetFrameOrigin (rect.Location);
		}
		
		private void addBounceAnimation ()
		{
			mover.Animations = NSDictionary.FromObjectsAndKeys (new object[] { OriginAnimation }, new object[] {(NSString)"frameOrigin"});	
		}
		
		private CAKeyFrameAnimation OriginAnimation {
			get {
				CAKeyFrameAnimation originAnimation = new CAKeyFrameAnimation ();
				originAnimation.Path = HeartPath;
				originAnimation.Duration = 2.0f;
				originAnimation.CalculationMode = CAAnimation.AnimationPaced;
				return originAnimation;
			}
		}
		
		private CGPath HeartPath {
			get {
				RectangleF frame = mover.Frame;
				if (heartPath == null) {	
					float minX = frame.GetMinX ();
					float minY = frame.GetMinY ();
					
					heartPath = new CGPath ();
					
					heartPath.MoveToPoint (minX, minY);
					heartPath.AddLineToPoint (minX - frame.Width, minY + frame.Height * 0.85f);
					heartPath.AddLineToPoint (minX, minY - frame.Height * 1.5f);
					heartPath.AddLineToPoint (minX + frame.Width, minY + frame.Height * 0.85f);
					heartPath.AddLineToPoint (minX, minY);
					heartPath.CloseSubpath ();
				}
				return heartPath;
			}	
		}		
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			CGContext ctx = NSGraphicsContext.CurrentContext.GraphicsPort;
			
			ctx.AddPath (HeartPath);
			ctx.StrokePath ();
		}
	}
}

