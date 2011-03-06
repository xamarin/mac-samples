
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;

namespace OpenGLLayer
{
        public partial class MyView : MonoMac.AppKit.NSView
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
                                        movingLayer.Frame = new RectangleF (0, 0, 150, 150);
                                }
                                return movingLayer;
                        }
                }

                public override void MouseDown (NSEvent theEvent)
                {
                        PointF location =  ConvertPointFromView(theEvent.LocationInWindow, null);
						movingLayer.Position = new PointF(location.X, location.Y);
                }
		
                partial void toggle (NSButton sender)
                {
                        movingLayer.Animate = !movingLayer.Animate;
                }
        }
}

