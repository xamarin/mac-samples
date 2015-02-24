
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using AppKit;
using CoreAnimation;

namespace KeyFrameMoveAView
{
	public partial class KeyFrameView : AppKit.NSView
	{
		NSImageView mover;
		CGPath heartPath;
		
		[Export("initWithFrame:")]
		public KeyFrameView(CGRect frame) : base(frame)
		{
			nfloat xInset = 3 * (frame.Width / 8);
			nfloat yInset = 3 * (frame.Height / 8);
			
			CGRect moverFrame = frame.Inset (xInset, yInset);

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
		
		public override void SetFrameOrigin (CGPoint newOrigin)
		{
			Console.WriteLine ("setting new origin");
			base.SetFrameOrigin (newOrigin);
		}
		
		private void bounce()
		{
			NSAnimationContext.RunAnimation ((context) => {
				var newFrame = mover.Frame;
				newFrame.Offset (1, 0);
				((NSView)mover.Animator).SetFrameOrigin (newFrame.Location);
			}, () => {});
		}
		
		private void addBounceAnimation ()
		{
			mover.Animations = NSDictionary.FromObjectAndKey (OriginAnimation, (NSString)"frameOrigin");
		}
		
		private CAKeyFrameAnimation OriginAnimation {
			get {
				return new CAKeyFrameAnimation {
					Path = HeartPath,
					Duration = 2.0f,
					CalculationMode = CAAnimation.AnimationPaced
				};
			}
		}
		
		private CGPath HeartPath {
			get {
				CGRect frame = mover.Frame;
				if (heartPath == null) {	
					nfloat minX = frame.GetMinX ();
					nfloat minY = frame.GetMinY ();
					
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
		
		public override void DrawRect (CGRect dirtyRect)
		{
			CGContext ctx = NSGraphicsContext.CurrentContext.GraphicsPort;
			
			ctx.AddPath (HeartPath);
			ctx.StrokePath ();
		}
	}
}

