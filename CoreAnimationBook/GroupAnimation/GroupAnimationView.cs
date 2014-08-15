
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Foundation;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace GroupAnimation
{
	public partial class GroupAnimationView : AppKit.NSView
	{
		NSImageView mover;
		
		[Export("initWithFrame:")]
		public GroupAnimationView(CGRect frame) : base(frame)
		{
			var xInset = 3 * (frame.Width / 8);
			var yInset = 3 * (frame.Height / 8);
			
			CGRect moverFrame = frame.Inset (xInset, yInset);
		
			CGPoint location = moverFrame.Location;
			location.X = this.Bounds.GetMidX () - moverFrame.Width / 2;
			location.Y = this.Bounds.GetMidY () - moverFrame.Height / 2;
			moverFrame.Location = location;
			mover = new NSImageView (moverFrame);
			
			mover.ImageScaling = NSImageScale.AxesIndependently;
			mover.Image = NSImage.ImageNamed ("photo.jpg");
			
			NSDictionary animations = NSDictionary.FromObjectsAndKeys (
				new object[] {GroupAnimation(moverFrame)},
				new object[] {"frameRotation"});
			mover.Animations = animations;	
			
			AddSubview (mover);
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			((NSView)mover.Animator).FrameRotation = mover.FrameRotation;
		}
		
		private CAAnimationGroup GroupAnimation (CGRect frame)
		{
			CAAnimationGroup animationGroup = CAAnimationGroup.CreateAnimation ();	
			animationGroup.Animations = new CAAnimation[] { frameAnimation (frame), 
															rotationAnimation () };
			animationGroup.Duration = 1;
			animationGroup.AutoReverses = true;
			return animationGroup;
		}
		
		private CAAnimation frameAnimation (CGRect aniFrame)
		{
			CAKeyFrameAnimation frameAni = new CAKeyFrameAnimation ();
			
			frameAni.KeyPath = "frame";
			CGRect start = aniFrame;
			CGRect end = aniFrame.Inset (-start.Width * .5f, -start.Height * 0.5f);
			frameAni.Values = new NSObject[] { 
				NSValue.FromCGRect (start),
				NSValue.FromCGRect (end) };
			return frameAni;
		}
		
		private CABasicAnimation rotationAnimation ()
		{
			CABasicAnimation rotation = new CABasicAnimation ();
			rotation.KeyPath = "frameRotation";
			rotation.From = NSNumber.FromFloat (0);
			rotation.To = NSNumber.FromFloat (45);
			return rotation;
		}
	}
}

