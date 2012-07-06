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
	class ClockLayer : CALayer {
		public ClockLayer ()
		{
		}
		
		[Export ("initWithLayer:")]
		public ClockLayer (CALayer other)
			: base (other)
		{
		}
		
		public override void Clone (CALayer other)
		{
			ClockColor = ((ClockLayer) other).ClockColor;
			base.Clone (other);
		}
		
		[Export ("clockColor")]
		public CGColor ClockColor { get; set; }
		
		[Export ("needsDisplayForKey:")]
		static bool NeedsDisplayForKey (NSString key)
		{
			switch (key.ToString ()) {
			case "clockColor":
				return true;
			default:
				return CALayer.NeedsDisplayForKey (key);
			}
		}

		public override void DrawInContext (CGContext context)
		{
			base.DrawInContext (context);
			
			context.AddEllipseInRect (Bounds);
			context.SetFillColor (ClockColor);
			context.FillPath ();
		}
	}
}
