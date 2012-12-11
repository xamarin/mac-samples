
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace CustomAnimationTiming
{
	public partial class CustomAnimationTiming : MonoMac.AppKit.NSView {
		NSImageView mover;
		CABasicAnimation moveAnimation;
		
		public CustomAnimationTiming (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public CustomAnimationTiming (NSCoder coder) : base(coder) {}

		[Export("initWithFrame:")]
		public CustomAnimationTiming (RectangleF frame) : base(frame)
		{
			SetupMover ();
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			Move();
		}
		
		void SetupMover()
		{
			RectangleF moverFrame = Bounds.Inset(Bounds.Width / 4.0f,
			                                    	Bounds.Height / 4.0f);
			PointF origin = moverFrame.Location;
			origin.X = 0.0f;
			moverFrame.Location = origin;
			
			mover = new NSImageView(moverFrame);
			mover.ImageScaling = NSImageScale.AxesIndependently;
			mover.Image = NSImage.ImageNamed("photo.jpg");
			AddSubview(mover);
		}
		
		CABasicAnimation MoveItAnimation()
		{
			if (moveAnimation == null)
				moveAnimation = new CABasicAnimation() {
					Duration = 2,
					TimingFunction = new CAMediaTimingFunction (0.5f, 1, 0.5f, 0)
				};
			return moveAnimation;
		}
		
		void Move() 
		{
			var animations = NSDictionary.FromObjectAndKey (MoveItAnimation (), (NSString)"frameOrigin");
			mover.Animations = animations;
			PointF origin = mover.Frame.Location;
			origin.X += mover.Frame.Width;
			((NSView)mover.Animator).SetFrameOrigin(origin);
		}
	}
}

