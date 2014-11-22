
using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using AppKit;
using CoreAnimation;
using CoreGraphics;
using OpenGL;

namespace OpenGLLayer
{
        public partial class MyView : AppKit.NSView
        {

                static OpenGLLayer movingLayer;

                // Called when created from unmanaged code
                public MyView (IntPtr handle) : base(handle)
                {
                }

                // Called when created directly from a XIB file
                [Export("initWithCoder:")]
                public MyView (NSCoder coder) : base(coder)
                {
                }

                public override void AwakeFromNib ()
                {
                        Layer = new CALayer ();
                        Layer.AddSublayer (MovingLayer);
                        WantsLayer = true;
                }

                private OpenGLLayer MovingLayer {
                        get {
                                if (movingLayer == null) {
                                        movingLayer = new OpenGLLayer ();
                                        movingLayer.Frame = new CGRect (0, 0, 150, 150);
                                }
                                return movingLayer;
                        }
                }

                public override void MouseDown (NSEvent theEvent)
                {
                        CGPoint location =  ConvertPointFromView(theEvent.LocationInWindow, null);
						movingLayer.Position = new CGPoint(location.X, location.Y);
                }
		
                partial void toggle (NSButton sender)
                {
                        movingLayer.Animate = !movingLayer.Animate;
                }
        }
}

