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
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

using MonoMac.OpenGL;

namespace NeHeLesson17
{
	public class Scene
	{
		Texture2D[] texture = new Texture2D[2];
		float cnt1;
		float cnt2;

		public Scene () : base()
		{
			loadGLTextures ();
		}

		bool loadGLTextures ()
		{
			bool status = false;

			texture [0] = new Texture2DFont (NSBundle.MainBundle.PathForResource ("Font", "bmp"));
			texture [1] = new Texture2D (NSBundle.MainBundle.PathForResource ("Bumps", "bmp"));
			if (texture [0].TextureName > 0 && texture [1].TextureName > 0) {
				status = true;
			}

			return status;
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
			const
			double DEG2RAD = Math.PI / 180 ; 

			// tangent of half fovY
			double tangent = Math.Tan (fovY / 2 * DEG2RAD);

			// half height of near plane
			double height = front * tangent;

			// half width of near plane
			double width = height * aspectRatio;

			// params: left, right, bottom, top, near, far
			GL.Frustum (-width, width, -height, height, front, back);
		}

		// This method renders our scene and where all of your drawing code will go.
		// The main thing to note is that we've factored the drawing code out of the NSView subclass so that
		// the full-screen and non-fullscreen views share the same states for rendering 
		public bool DrawGLScene ()
		{
			// Clear The Screen And The Depth Buffer
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			// Reset The Current Modelview Matrix
			GL.LoadIdentity ();

			// Select second texture
			GL.BindTexture (TextureTarget.Texture2D, texture [1].TextureName);   
			//GL.BindTexture (TextureTarget.Texture2D, texture[1]);   
			// Move into screen 5 units
			GL.Translate (0.0f, 0.0f, -5.0f);   
			// Rotate around Z axis (clockwise)
			GL.Rotate (45.0f, 0.0f, 0.0f, 1.0f);   
			// Rotate on X and Y axis by cnt1 (left and right )
			GL.Rotate (cnt1 * 30.0f, 1.0f, 1.0f, 0.0f);
			// Disable blending before drawing in 3D
			GL.Disable (EnableCap.Blend);   
			// Bright white
			GL.Color3 (1.0f, 1.0f, 1.0f);   
			GL.Begin (BeginMode.Quads);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (-1.0f, 1.0f);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (1.0f, 1.0f);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (1.0f, -1.0f);
			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (-1.0f, -1.0f);
			GL.End ();

			// Rotate on the X and Y axis by 90 degrees (left to right )
			GL.Rotate (90.0f, 1.0f, 1.0f, 0.0f);
			GL.Begin (BeginMode.Quads);
			GL.TexCoord2 (0.0f, 0.0f);
			GL.Vertex2 (-1.0f, 1.0f);
			GL.TexCoord2 (1.0f, 0.0f);
			GL.Vertex2 (1.0f, 1.0f);
			GL.TexCoord2 (1.0f, 1.0f);
			GL.Vertex2 (1.0f, -1.0f);
			GL.TexCoord2 (0.0f, 1.0f);
			GL.Vertex2 (-1.0f, -1.0f);
			GL.End ();

			GL.Enable (EnableCap.Blend);
			GL.LoadIdentity ();   // Reset the view

			// Pulsing colors based on text position
			GL.Color3 (Math.Cos (cnt1), Math.Sin (cnt2), 1.0f - 0.5f * Math.Cos (cnt1 + cnt2));
			// Print GL text to the screen
			Texture2DFont font = (Texture2DFont)texture[0];
			
			font.RenderText (280 + (int)(250 * Math.Cos (cnt1)), 
				235 + (int)(200 * Math.Sin (cnt2)), "NeHe", 0);

			GL.Color3 (1.0f * (float)Math.Sin (cnt2), 1.0f - 0.5f * (float)Math.Cos (cnt1 + cnt2), 1.0f * (float)Math.Cos (cnt1));
			font.RenderText ((int)(280 + 230 * Math.Cos (cnt2)), (int)(235 + 200 * Math.Sin (cnt1)), "OpenGL", 1);
			
			// Set Color To Blue
			GL.Color3 (Color.Blue);						
			font.RenderText ((int)(240 + 200 * Math.Cos ((cnt2 + cnt1) / 5)), 3, "Giuseppe D'Agata", 0);

			GL.Color3 (Color.White);
			font.RenderText ((int)(243 + 200 * Math.Cos ((cnt2 + cnt1) / 5)), 2, "Giuseppe D'Agata", 0);

			// Increase first counter
			cnt1 += 0.01f;
			// Increase second counter
			cnt2 += 0.0081f;
			return true;
		}

	}
}

