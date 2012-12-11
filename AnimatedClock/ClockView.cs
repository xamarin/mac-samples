
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Timers;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace AnimatedClock
{
	public partial class ClockView : MonoMac.AppKit.NSView
	{
		ClockLayer backgroundLayer;
		CATextLayer clockFaceLayer;
		ClockTimer clockTimer;

		CAAnimationGroup backgroundAnimation;

		CGColor red = new CGColor (1, 0, 0, 1);
		CGColor green = new CGColor (0, 1, 0, 1);
		CGColor blue = new CGColor (0, 0, 1, 1);

		public ClockView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public ClockView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		void Initialize ()
		{
			clockTimer = new ClockTimer ();
			
			Layer = SetupLayers ();
		}
		
		public override void AwakeFromNib ()
		{
			WantsLayer = true;
		}
		
		private CALayer SetupLayers()
		{
			backgroundLayer = SetupBackgroundLayer ();
			backgroundLayer.AddSublayer (SetupClockFaceLayer ());
			return backgroundLayer;
		}
		
		ClockLayer SetupBackgroundLayer() 
		{
			// Create the color animation
			var rg = CABasicAnimation.FromKeyPath ("clockColor");
			rg.Duration = 3;
			rg.From = new NSObject (red.Handle);
			rg.To = new NSObject (green.Handle);

			var gb = CABasicAnimation.FromKeyPath ("clockColor");
			gb.Duration = 3;
			gb.BeginTime = 3;
			gb.From = rg.To;
			gb.To = new NSObject (blue.Handle);

			var br = CABasicAnimation.FromKeyPath ("clockColor");
			br.Duration = 3;
			br.BeginTime = 6;
			br.From = gb.To;
			br.To = rg.From;
			
			backgroundAnimation = new CAAnimationGroup ();
			backgroundAnimation.RepeatCount = 1000;
			backgroundAnimation.Duration = 9;
			backgroundAnimation.Animations = new CAAnimation [] { rg, gb, br };

			// Create the background layer
			backgroundLayer = new ClockLayer ();
			backgroundLayer.ClockColor = new CGColor (0.5f, 1f, 0f, 1.0f);
			backgroundLayer.AddAnimation (backgroundAnimation, "colorAnimation");

			CAConstraintLayoutManager layout = CAConstraintLayoutManager.LayoutManager;
			backgroundLayer.LayoutManager = layout;
			
			return backgroundLayer;
		}
		
		CALayer SetupBorderLayer()
		{
			CALayer borderLayer = CALayer.Create();
			
			RectangleF borderRect = Frame.Inset (8, 8);
			borderLayer.CornerRadius = 12;
			borderLayer.BorderColor = new CGColor (1, 1, 1, 1);
			borderLayer.BorderWidth = 2;
			borderLayer.Frame = borderRect;
			
			return borderLayer;
		}

		CALayer SetupClockFaceLayer()
		{
			clockFaceLayer = new CATextLayer (){
				FontSize = 60,
				ShadowOpacity = .9f
			};
			clockFaceLayer.Bind ("string", clockTimer, "outputString", null);
			
			clockFaceLayer.SetFont ("Menlo");
			
			// Constrain the text layer in the middle
			var constraint = CAConstraint.Create (CAConstraintAttribute.MidX, "superlayer", CAConstraintAttribute.MidX);
			clockFaceLayer.AddConstraint (constraint);
			
			constraint = CAConstraint.Create (CAConstraintAttribute.MidY, "superlayer", CAConstraintAttribute.MidY);
			
			clockFaceLayer.AddConstraint (constraint);
			return clockFaceLayer;
		}
	}
}

