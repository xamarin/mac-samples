
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Foundation;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using CoreImage;

namespace BackgroundFilteredView
{
	public partial class BackgroundFilteredView : AppKit.NSView
	{
		public BackgroundFilteredView (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public BackgroundFilteredView (NSCoder coder) : base(coder) {}
		
		public override void AwakeFromNib ()
		{
			WantsLayer = true;
			ApplyFilter ();
			AddAnimationToTorusFilter ();
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
		}
		
		void ApplyFilter ()
		{
			CIVector center = CIVector.Create (Bounds.GetMidX (), Bounds.GetMidY ());
			CIFilter torus = CIFilter.FromName ("CITorusLensDistortion");
			
			var keys = new NSString[] { CIFilterInputKey.Center,
						    CIFilterInputKey.Radius,
						    CIFilterInputKey.Width,
						    CIFilterInputKey.Refraction };
			var values = new NSObject[] { center,
						      NSNumber.FromFloat (150),
						      NSNumber.FromFloat (2),
						      NSNumber.FromFloat (1.7f)};
			
			torus.SetValuesForKeysWithDictionary (NSDictionary.FromObjectsAndKeys (values,keys));
			
			controls.BackgroundFilters = new CIFilter[] { torus };								
			AddAnimationToTorusFilter ();
		}
		
		public override void DrawRect (CGRect dirtyRect)
		{
			CGRect bounds = Bounds;
			CGSize stripeSize = bounds.Size;
			stripeSize.Width = bounds.Width / 10.0f;
			CGRect stripe = bounds;
			stripe.Size = stripeSize;
			NSColor[] colors = new NSColor[2] { NSColor.White, NSColor.Blue };
			for (int i = 0; i < 10; i++){
				colors[i % 2].Set();
				NSGraphics.RectFill(stripe);
				CGPoint origin = stripe.Location;
				origin.X += stripe.Size.Width;
				stripe.Location = origin;
			}
		}
		
		private void AddAnimationToTorusFilter()
		{
			string keyPath = string.Format ("backgroundFilters.torus.{0}", CIFilterInputKey.Width.ToString ());
			CABasicAnimation animation = new CABasicAnimation ();
			animation.KeyPath = keyPath;
			animation.From = NSNumber.FromFloat (50);
			animation.To = NSNumber.FromFloat (80);
			animation.Duration = 1;
			animation.RepeatCount = float.MaxValue;
			animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			animation.AutoReverses = true;
			controls.Layer.AddAnimation (animation, "torusAnimation");
		}

		private void RemoveBackgroundFilter()
		{
			controls.BackgroundFilters = null;
		}

		partial void RemoveFilter (NSButton sender)
		{
			if (controls.BackgroundFilters != null || this.BackgroundFilters.Count() > 0)
				RemoveBackgroundFilter ();
		}
		
		partial void AddFilter (NSButton sender)
		{
			if (controls.BackgroundFilters == null || this.BackgroundFilters.Count() == 0)
				ApplyFilter ();
		}
	}
}

