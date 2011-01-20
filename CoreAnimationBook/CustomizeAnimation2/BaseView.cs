
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
	public partial class BaseView : MonoMac.AppKit.NSView
	{
		NSImageView mover;
		bool isRight;
		PointF leftPosition;
		PointF rightPosition;
		
		public BaseView (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public BaseView (NSCoder coder) : base(coder) {}

		[Export("initWithFrame:")]
		public BaseView (RectangleF frame) : base(frame)
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
			RectangleF moverRect  = Bounds.Inset(Bounds.Size.Width / 4, Bounds.Size.Height / 4);
			PointF origin = moverRect.Location;
			origin.X = 0;
			moverRect.Location = origin;
			mover = new NSImageView (moverRect);
			leftPosition = new PointF (0, moverRect.GetMinY ());
			rightPosition = new PointF (Bounds.GetMaxX () - moverRect.Width, moverRect.GetMinY ());
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
			PointF target = isRight ? leftPosition : rightPosition;
			((NSView)mover.Animator).SetFrameOrigin(target);
			isRight = !isRight;
		}
		
		public NSImageView Mover
		{
			get { return mover; }
		}
	}
}

