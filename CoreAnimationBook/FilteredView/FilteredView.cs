
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Foundation;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using CoreImage;

namespace FilteredView
{
	public partial class FilteredView : AppKit.NSView
	{
		public FilteredView (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public FilteredView (NSCoder coder) : base(coder) {}

		public override void AwakeFromNib ()
		{
			controls.WantsLayer = true;
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
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
				colors [i % 2].Set ();
				NSGraphics.RectFill (stripe);
				CGPoint origin = stripe.Location;
				origin.X += stripe.Size.Width;
				stripe.Location = origin;
			}
		}
		
		private void Pointalize ()
		{
			CIVector center = CIVector.Create (Bounds.GetMidX (), Bounds.GetMidY ());
			
			CIFilter pointalize = CIFilter.FromName ("CIPointillize");
			pointalize.SetValueForKey (NSNumber.FromFloat (1), CIFilterInputKey.Radius);
			pointalize.SetValueForKey (center, CIFilterInputKey.Center);
			
			controls.ContentFilters = new CIFilter[] { pointalize };
		}
		
		partial void lightPointalize (NSButton sender)
		{
			if (controls.ContentFilters == null || controls.ContentFilters.Count() == 0)
				Pointalize();	
			
			var path = string.Format ("contentFilters.pointalize.{0}", CIFilterInputKey.Radius);
			controls.SetValueForKeyPath (NSNumber.FromFloat (1.0f), (NSString)path);
		}
		
		partial void heavyPointalize (NSButton sender)
		{
			if (controls.ContentFilters == null || controls.ContentFilters.Count() == 0)
				Pointalize();	
			
			string path = string.Format ("contentFilters.pointalize.{0}", CIFilterInputKey.Radius);
			controls.SetValueForKeyPath (NSNumber.FromFloat (5), (NSString)path);
		}
	}
}

