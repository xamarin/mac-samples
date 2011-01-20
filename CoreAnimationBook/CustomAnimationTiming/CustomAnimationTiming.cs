
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
	public partial class CustomAnimationTiming : MonoMac.AppKit.NSView
	{
		
		NSImageView mover;
		CABasicAnimation moveAnimation;
		
		#region Constructors

		// Called when created from unmanaged code
		public CustomAnimationTiming (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public CustomAnimationTiming (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public CustomAnimationTiming (RectangleF frame) : base(frame)
		{
			setupMover();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			move();
		}
		
		private void setupMover()
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
		
		private CABasicAnimation moveItAnimation()
		{
			if (moveAnimation == null)
			{
				moveAnimation = new CABasicAnimation();
				moveAnimation.Duration = 2.0f;
				moveAnimation.TimingFunction = 
					new CAMediaTimingFunction(0.5f, 1.0f, 0.5f, 0.0f);
				
			}
			return moveAnimation;
		}
		
		private void move() 
		{
			NSDictionary animations = NSDictionary.FromObjectAndKey(moveItAnimation(),
			                                                        (NSString)"frameOrigin");
			mover.Animations = animations;
			PointF origin = mover.Frame.Location;
			origin.X += mover.Frame.Width;
			((NSView)mover.Animator).SetFrameOrigin(origin);
		}
	}
}

