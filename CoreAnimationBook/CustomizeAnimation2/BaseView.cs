
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
		
		#region Constructors

		// Called when created from unmanaged code
		public BaseView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public BaseView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		[Export("initWithFrame:")]
		public BaseView (RectangleF frame) : base(frame)
		{
			initializeFramePositions();
			addImageToSubview();
			
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
		
		private void initializeFramePositions()
		{
			RectangleF moverRect  = Bounds.Inset(Bounds.Size.Width / 4.0f,
			                                     Bounds.Size.Height / 4.0f);
			PointF origin = moverRect.Location;
			origin.X = 0.0f;
			moverRect.Location = origin;
			mover = new NSImageView(moverRect);
			leftPosition = new PointF(0.0f, moverRect.GetMinY());
			rightPosition = new PointF(Bounds.GetMaxX() - moverRect.Width,
			                           moverRect.GetMinY());
			isRight = false;
			                                     
		}
		
		private void addImageToSubview()
		{
			mover.ImageScaling = NSImageScale.AxesIndependently;
			mover.Image = NSImage.ImageNamed("photo.jpg");
			AddSubview(mover);
		}
		
		private void move() 
		{
			PointF target;
			if (isRight)
				target = leftPosition;
			else
				target = rightPosition;
			((NSView)mover.Animator).SetFrameOrigin(target);
			isRight = !isRight;
		}
		
		public NSImageView Mover
		{
			get { return mover; }
		}
	}
}

