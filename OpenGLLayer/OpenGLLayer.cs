
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using MonoMac.CoreGraphics;
using MonoMac.CoreVideo;
using MonoMac.OpenGL;
using System.Runtime.InteropServices;
namespace OpenGLLayer
{
        public partial class OpenGLLayer : MonoMac.CoreAnimation.CAOpenGLLayer
        {

                double previousTime;
                double rotation;
                bool animate;
                //IntPtr localContext;

				float[,] cube_vertices = new float[8,3] {
					{-1, -1, 1},  // 2    0
					{1, -1, 1},   // 1    1
					{1, 1, 1},    // 0    2
					{-1, 1, 1},   // 3    3
					{-1, 1, -1},  // 7    4
					{1, 1, -1},   // 4    5
					{-1, -1, -1}, // 6    6
					{1, -1, -1}   // 5    7
					
				};
				
				float[,] cube_face_colors = new float[6,3] {
					{0.4f, 1.0f, 0.4f}, // flora
					{0.0f, 0.0f, 1.0f}, // blueberry
					{0.4f, 0.8f, 1.0f}, // sky
					{1.0f, 0.8f, 0.4f}, // cantelopue
					{1.0f, 1.0f, 0.4f}, // blubble gum
					{0.5f, 0.0f, 0.25f}  // marron
				};
				
				int num_faces = 6;
				
				short[,] cube_faces = new short[6,4] {
					{3, 0, 1, 2}, // +Z
					{0, 3, 4, 6}, // -X
					{2, 1, 7, 5}, // +X
					{3, 2, 5, 4}, // +Y
					{1, 0, 6, 7}, // -Y
					{5, 7, 6, 4}  // -Z
				};
		
                #region Constructors

                public OpenGLLayer () : base()
                {
                        Initialize ();
                }

                // Called when created from unmanaged code
                public OpenGLLayer (IntPtr handle) : base(handle)
                {
                        Initialize ();
                }

                // Called when created directly from a XIB file
                [Export("initWithCoder:")]
                public OpenGLLayer (NSCoder coder) : base(coder)
                {
                        Initialize ();
                }

                // Shared initialization code
                void Initialize ()
                {
                        Animate = true;
                        this.Asynchronous = true;
                }

                #endregion

                public bool Animate {
                        get { return animate; }
                        set { animate = value; }
                }

                public override bool CanDrawInCGLContext (CGLContext glContext, CGLPixelFormat pixelFormat, double timeInterval, CVTimeStamp timeStamp)
                {
                        if (!animate)
                                previousTime = 0.0;
                        return animate;
                }


                public override void DrawInCGLContext (MonoMac.OpenGL.CGLContext glContext, CGLPixelFormat pixelFormat, double timeInterval, CVTimeStamp timeStamp)
                {
                        GL.ClearColor (NSColor.Clear.UsingColorSpace (NSColorSpace.CalibratedRGB));
                        GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                        GL.Enable (EnableCap.DepthTest);
                        GL.Hint (HintTarget.LineSmoothHint, HintMode.Nicest);
                        GL.Hint (HintTarget.PolygonSmoothHint, HintMode.Nicest);
                        if (previousTime == 0)
                                previousTime = timeInterval;
                        rotation += 15.0 * (timeInterval - previousTime);
                        GL.LoadIdentity ();
                        double comp = 1 / Math.Sqrt (3.0);
                        GL.Rotate (rotation, comp, comp, comp);
                        
                        drawCube ();
                        
                        GL.Flush ();
                        previousTime = timeInterval;
                        GL.Disable (EnableCap.DepthTest);
                        GL.Hint (HintTarget.LineSmoothHint, HintMode.DontCare);
                        GL.Hint (HintTarget.PolygonSmoothHint, HintMode.DontCare);
                        
                        
                }

                public override CGLPixelFormat CopyCGLPixelFormatForDisplayMask (uint mask)
                {
                        
                        // make sure to add a null value
                        CGLPixelFormatAttribute[] attribs = new CGLPixelFormatAttribute[] { 
							CGLPixelFormatAttribute.Accelerated, 
							CGLPixelFormatAttribute.DoubleBuffer, CGLPixelFormatAttribute.ColorSize, (CGLPixelFormatAttribute)24, CGLPixelFormatAttribute.DepthSize, (CGLPixelFormatAttribute)16, (CGLPixelFormatAttribute)0 };
                        
                        int numPixs = -1;
                        CGLPixelFormat pixelFormat = new CGLPixelFormat (attribs, out numPixs);
                        return pixelFormat;
                }



                private void drawCube ()
                {
                        long f, i;
                        double fSize = 0.5;
                        GL.Begin (BeginMode.Quads);
                        for (f = 0; f < num_faces; f++) {
                                GL.Color3 (cube_face_colors[f, 0], cube_face_colors[f, 1], cube_face_colors[f, 2]);
                                for (i = 0; i < 4; i++) {
                                        GL.Vertex3 (cube_vertices[cube_faces[f, i], 0] * fSize, cube_vertices[cube_faces[f, i], 1] * fSize, cube_vertices[cube_faces[f, i], 2] * fSize);
                                }
                                
                        }
                        
                        GL.End ();
                        GL.Color3 (Color.Black);
                        
                        for (f = 0; f < num_faces; f++) {
                                GL.Begin (BeginMode.LineLoop);
                                for (i = 0; i < 4; i++)
                                        GL.Vertex3 (cube_vertices[cube_faces[f, i], 0] * fSize, cube_vertices[cube_faces[f, i], 1] * fSize, cube_vertices[cube_faces[f, i], 2] * fSize);
                                GL.End ();
                        }
                        
                        
                }
                
        }
}

