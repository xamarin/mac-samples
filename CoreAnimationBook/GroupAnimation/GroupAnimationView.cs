
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace GroupAnimation
{
	public partial class GroupAnimationView : MonoMac.AppKit.NSView
	{
		NSImageView mover;
		
		[Export("initWithFrame:")]
		public GroupAnimationView(RectangleF frame) : base(frame)
		{
			float xInset = 3 * (frame.Width / 8);
			float yInset = 3 * (frame.Height / 8);
			
			RectangleF moverFrame = frame.Inset (xInset, yInset);
		
			PointF location = moverFrame.Location;
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
		
		private CAAnimationGroup GroupAnimation (RectangleF frame)
		{
			CAAnimationGroup animationGroup = CAAnimationGroup.CreateAnimation ();	
			animationGroup.Animations = new CAAnimation[] { frameAnimation (frame), 
															rotationAnimation () };
			animationGroup.Duration = 1;
			animationGroup.AutoReverses = true;
			return animationGroup;
		}
		
		private CAAnimation frameAnimation (RectangleF aniFrame)
		{
			CAKeyFrameAnimation frameAni = new CAKeyFrameAnimation ();
			
			frameAni.KeyPath = "frame";
			RectangleF start = aniFrame;
			RectangleF end = aniFrame.Inset (-start.Width * .5f, -start.Height * 0.5f);
			frameAni.Values = new NSObject[] { 
				NSValue.FromRectangleF (start),
				NSValue.FromRectangleF (end) };
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

