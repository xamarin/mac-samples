//
// This code was created by Jeff Molofee '99
//
// If you've found this code useful, please let me know.
//
// Visit me at www.demonews.com/hosted/nehe
//
//=====================================================================
// Converted to C# and MonoMac by Kenneth J. Pouncey
// http://www.cocoa-mono.org
//
// Copyright (c) 2011 Kenneth J. Pouncey
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.OpenGL;

namespace NeHeLesson4
{
	public class Scene : NSObject
	{

		float rtri;
		// Angle For The Triangle ( NEW )
		float rquad;
		// Angle For The Quad     ( NEW )
		public Scene () : base()
		{
		}

		// Resize And Initialize The GL Window 
		//      - See also the method in the MyOpenGLView Constructor about the NSView.NSViewGlobalFrameDidChangeNotification
		public void ResizeGLScene (RectangleF bounds)
		{
			// Reset The Current Viewport
			GL.Viewport (0, 0, (int)bounds.Size.Width, (int)bounds.Size.Height);
			// Select The Projection Matrix
			GL.MatrixMode (MatrixMode.Projection);
			// Reset The Projection Matrix
			GL.LoadIdentity ();
			
			// Set perspective here - Calculate The Aspect Ratio Of The Window
			Perspective (45, bounds.Size.Width / bounds.Size.Height, 0.1, 100);
			
			// Select The Modelview Matrix
			GL.MatrixMode (MatrixMode.Modelview);
			// Reset The Modelview Matrix
			GL.LoadIdentity ();
		}

		// This creates a symmetric frustum.
		// It converts to 6 params (l, r, b, t, n, f) for glFrustum()
		// from given 4 params (fovy, aspect, near, far)
		public static void Perspective (double fovY, double aspectRatio, double front, double back)
		{
			const double DEG2RAD = Math.PI / 180;
			
			// tangent of half fovY
			double tangent = Math.Tan (fovY / 2 * DEG2RAD);
			
			// half height of near plane
			double height = front * tangent;
			
			// half width of near plane
			double width = height * aspectRatio;
			
			// params: left, right, bottom, top, near, far
			GL.Frustum (-width, width, -height, height, front, back);
		}

		// This method renders our scene.
		// The main thing to note is that we've factored the drawing code out of the NSView subclass so that
		// the full-screen and non-fullscreen views share the same states for rendering 
		public bool DrawGLScene ()
		{
			// Clear The Screen And The Depth Buffer
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			// Reset The Current Modelview Matrix
			GL.LoadIdentity ();
			
			GL.Translate (-1.5f, 0.0f, -6.0f);
			// Move Left 1.5 Units And Into The Screen 6.0
			GL.Rotate (rtri, 0.0f, 1.0f, 0.0f);
			// Rotate The Triangle On The Y axis ( NEW )
			GL.Begin (BeginMode.Triangles);
			// Drawing Using Triangles
			GL.Color3 (1.0, 0.0, 0.0);
			// Set The Color To Red
			GL.Vertex3 (0.0f, 1.0f, 0.0f);
			// Top
			GL.Color3 (Color.Green);
			// Set The Color To Green
			GL.Vertex3 (-1.0f, -1.0f, 0.0f);
			// Bottom Left
			GL.Color3 (NSColor.Blue.UsingColorSpace (NSColorSpace.CalibratedRGB));
			// Set The Color To Blue
			GL.Vertex3 (1.0f, -1.0f, 0.0f);
			// Bottom Right
			GL.End ();
			// Finished Drawing The Triangle
			// Reset The Current Modelview Matrix
			GL.LoadIdentity ();
			
			GL.Translate (1.5f, 0.0f, -6.0f);
			// Move Right 1.5 Units And Into The Screen 6.0
			GL.Rotate (rquad, 1.0f, 0.0f, 0.0f);
			// Rotate The Quad On The X axis ( NEW )   
			GL.Color3 (0.5f, 0.5f, 1.0f);
			// Set The Color To Blue One Time Only
			GL.Begin (BeginMode.Quads);
			// Start Drawing Quads
			GL.Vertex3 (-1.0f, 1.0f, 0.0f);
			// Left And Up 1 Unit (Top Left)
			GL.Vertex3 (1.0f, 1.0f, 0.0f);
			// Right And Up 1 Unit (Top Right)
			GL.Vertex3 (1.0f, -1.0f, 0.0f);
			// Right And Down One Unit (Bottom Right)
			GL.Vertex3 (-1.0f, -1.0f, 0.0f);
			// Left And Down One Unit (Bottom Left)
			GL.End ();
			// Done Drawing A Quad
			rtri += 0.2f;
			// Increase The Rotation Variable For The Triangle ( NEW )
			rquad -= 0.15f;
			// Decrease The Rotation Variable For The Quad     ( NEW )
			return true;
		}
		
	}
}

