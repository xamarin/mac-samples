
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;

namespace BackgroundFilteredView
{
	public partial class BackgroundFilteredView : MonoMac.AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public BackgroundFilteredView (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public BackgroundFilteredView (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		public override void AwakeFromNib ()
		{
			WantsLayer = true;
			applyFilter();
			addAnimationToTorusFilter();
		}
		
		public override bool AcceptsFirstResponder ()
		{
			return true;
		}
		
		public override void KeyDown (NSEvent theEvent)
		{
			base.KeyDown (theEvent);
		}
		
		private void applyFilter()
		{
			CIVector center = CIVector.Create(Bounds.GetMidX(), Bounds.GetMidY());
			CIFilter torus = CIFilter.FromName("CITorusLensDistortion");
			
			var keys = new NSString[] { CIFilter.InputCenterKey,
											CIFilter.InputRadiusKey,
											CIFilter.InputWidthKey,
											CIFilter.InputRefractionKey };
			var values = new NSObject[] { center,
				                                                         NSNumber.FromFloat(150.0f),
				                                                         NSNumber.FromFloat(2.0f),
				                                                         NSNumber.FromFloat(1.7f)};
			torus.SetValuesForKeysWithDictionary(NSDictionary.FromObjectsAndKeys(values,keys));	                                                       
			torus.Name = "torus";
			
			controls.BackgroundFilters = new CIFilter[] { torus };								
			addAnimationToTorusFilter();
		}
		
		public override void DrawRect (RectangleF dirtyRect)
		{
			RectangleF bounds = Bounds;
			SizeF stripeSize = bounds.Size;
			stripeSize.Width = bounds.Width / 10.0f;
			RectangleF stripe = bounds;
			stripe.Size = stripeSize;
			NSColor[] colors = new NSColor[2] {NSColor.White, NSColor.Blue};
			for (int i = 0; i < 10; i++)
			{
				colors[i % 2].Set();
				NSGraphics.RectFill(stripe);
				PointF origin = stripe.Location;
				origin.X += stripe.Size.Width;
				stripe.Location = origin;
			}
		}
		
		private void addAnimationToTorusFilter()
		{
			string keyPath = string.Format("backgroundFilters.torus.{0}",CIFilter.InputWidthKey.ToString());
			CABasicAnimation animation = new CABasicAnimation();
			animation.KeyPath = keyPath;
			animation.From = NSNumber.FromFloat(50.0f);
			animation.To = NSNumber.FromFloat(80.0f);
			animation.Duration = 1.0f;
			animation.RepeatCount = float.MaxValue;
			animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
			animation.AutoReverses = true;
			controls.Layer.AddAnimation(animation, "torusAnimation");
			
		}

		private void removeBackgroundFilter()
		{
			controls.BackgroundFilters = null;
		}

		partial void removeFilter (NSButton sender)
		{
			if (controls.BackgroundFilters != null || this.BackgroundFilters.Count() > 0)
				removeBackgroundFilter();
		}
		
		partial void addFilter (NSButton sender)
		{
			if (controls.BackgroundFilters == null || this.BackgroundFilters.Count() == 0)
				applyFilter();
		}
	}
}

