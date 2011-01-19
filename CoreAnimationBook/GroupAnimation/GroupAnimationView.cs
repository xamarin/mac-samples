
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
		
		#region Constructors

		[Export("initWithFrame:")]
		public GroupAnimationView(RectangleF frame) : base(frame)
		{
			float xInset = 3.0f * (frame.Width / 8.0f);
			float yInset = 3.0f * (frame.Height / 8.0f);
			
			RectangleF moverFrame = frame.Inset(xInset, yInset);
		
			PointF location = moverFrame.Location;
			location.X = this.Bounds.GetMidX() - moverFrame.Width / 2.0f;
			location.Y = this.Bounds.GetMidY() - moverFrame.Height / 2.0f;
			moverFrame.Location = location;
			mover = new NSImageView(moverFrame);
			
			mover.ImageScaling = NSImageScale.AxesIndependently;
			mover.Image = NSImage.ImageNamed("photo.jpg");
			
			NSDictionary animations = NSDictionary.FromObjectsAndKeys(new object[] {groupAnimation(moverFrame)},
																		new object[] {"frameRotation"});
			mover.Animations = animations;	
			
			AddSubview(mover);
		}
		
		#endregion
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			((NSView)mover.Animator).FrameRotation = mover.FrameRotation;
		}
		
		private CAAnimationGroup groupAnimation(RectangleF frame)
		{
			CAAnimationGroup animationGroup = CAAnimationGroup.CreateAnimation();	
			animationGroup.Animations = new CAAnimation[] { frameAnimation(frame), 
															rotationAnimation() };
			animationGroup.Duration = 1.0f;
			animationGroup.AutoReverses = true;
			return animationGroup;
		}
		
		private CAAnimation frameAnimation(RectangleF aniFrame)
		{
			CAKeyFrameAnimation frameAni = new CAKeyFrameAnimation();
			
			frameAni.KeyPath = "frame";
			RectangleF start = aniFrame;
			RectangleF end = aniFrame.Inset(-start.Width * .5f, -start.Height * 0.5f);
			frameAni.Values = new NSObject[] {NSValue.FromRectangleF(start),
							NSValue.FromRectangleF(end)};
				
			return frameAni;
		}
		
		private CABasicAnimation rotationAnimation()
		{
			CABasicAnimation rotation = new CABasicAnimation();
			rotation.KeyPath = "frameRotation";
			rotation.From = NSNumber.FromFloat(0.0f);
			rotation.To = NSNumber.FromFloat(45.0f);
			return rotation;
		}
	}
}

