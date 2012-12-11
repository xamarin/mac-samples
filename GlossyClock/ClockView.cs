
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Timers;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;

namespace GlossyClock
{
	public partial class ClockView : MonoMac.AppKit.NSView
	{
		CALayer backgroundLayer;
		CATextLayer clockFaceLayer;
		ClockTimer clockTimer;

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
			
			Layer = SetupLayers();
		}
		
		public override void AwakeFromNib ()
		{
			WantsLayer = true;
		}
		
		private CALayer SetupLayers()
		{
			backgroundLayer = SetupBackgroundLayer ();
			
			backgroundLayer.AddSublayer (SetupClockFaceLayer ());
			backgroundLayer.AddSublayer (SetupBorderLayer ());
			backgroundLayer.AddSublayer (SetupGlossyLayer ());

			return backgroundLayer;
		}
		
		CALayer SetupBackgroundLayer() 
		{
			backgroundLayer = new CAGradientLayer ();
			
			CGColor gradColor1 = new CGColor (13.0f / 255.0f, 116.0f / 255.0f, 1.0f,1.0f);
			CGColor gradColor2 = new CGColor (0.0f, 53.0f / 255.0f, 126.0f / 255.0f,1.0f);
			
			((CAGradientLayer)backgroundLayer).Colors = new CGColor[2] { gradColor1, gradColor2 };
			backgroundLayer.CornerRadius = 12.0f;
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
		
		CALayer SetupGlossyLayer()
		{
			// Create the CGImage by proxying it through an NSImage
			string filePath = NSBundle.MainBundle.PathForResource("clock-gloss","png");
			var glossyImage = new NSImage (filePath).AsCGImage (RectangleF.Empty, null, null);

			CALayer glossLayer = new CALayer() {
				Opacity = 0.8f,
				CornerRadius = 12,
				MasksToBounds = true,
				Frame = this.Frame,
				Contents = glossyImage
			};
			return glossLayer;
		}
	}
}

