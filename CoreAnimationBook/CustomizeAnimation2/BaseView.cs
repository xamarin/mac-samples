
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Foundation;
using AppKit;
using CoreAnimation;
using CoreGraphics;

namespace CustomizeAnimation
{
	public partial class BaseView : AppKit.NSView
	{
		NSImageView mover;
		bool isRight;
		CGPoint leftPosition;
		CGPoint rightPosition;
		
		public BaseView (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public BaseView (NSCoder coder) : base(coder) {}

		[Export("initWithFrame:")]
		public BaseView (CGRect frame) : base(frame)
		{
			InitializeFramePositions();
			AddImageToSubview();
		}

		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			Move ();
		}
		
		void InitializeFramePositions ()
		{
			CGRect moverRect  = Bounds.Inset(Bounds.Size.Width / 4, Bounds.Size.Height / 4);
			CGPoint origin = moverRect.Location;
			origin.X = 0;
			moverRect.Location = origin;
			mover = new NSImageView (moverRect);
			leftPosition = new CGPoint (0, moverRect.GetMinY ());
			rightPosition = new CGPoint (Bounds.GetMaxX () - moverRect.Width, moverRect.GetMinY ());
			isRight = false;
		}
		
		void AddImageToSubview ()
		{
			mover.ImageScaling = NSImageScale.AxesIndependently;
			mover.Image = NSImage.ImageNamed ("photo.jpg");
			AddSubview (mover);
		}
		
		void Move () 
		{
			CGPoint target = isRight ? leftPosition : rightPosition;
			((NSView)mover.Animator).SetFrameOrigin(target);
			isRight = !isRight;
		}
		
		public NSImageView Mover
		{
			get { return mover; }
		}
	}
}

