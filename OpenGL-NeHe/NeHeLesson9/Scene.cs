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

namespace NeHeLesson9
{
	public class Scene
	{
		// Texture
		Texture texture;
		// Texture Id
		int textureName;

		// Twinkling Stars
		bool twinkle;

		// Number of Stars to Draw
		const int num = 50;

		// Create a structure for star
		struct star
		{
			// Star Color
			public Color color;
			// Stars distance from Center
			public float distance;
			// Stars current angle
			public float angle;

			public star (Color color, float distance, float angle)
			{
				this.color = color;
				this.distance = distance;
				this.angle = angle;
			}
		}

		// an array of stars
		star[] stars = new star[num];

		// Viewing distance away from starts
		float zoom = -15;

		// tilt the view
		float tilt = 90;

		// spin twinkling stars
		float spin;
		Random rng = new Random ((int)DateTime.Now.Ticks);

		public Scene () : base()
		{
			InitStars ();
		}

		void InitStars ()
		{

			for (int loop = 0; loop < num; loop++) {
				var nc = Color.FromArgb (rng.Next () % 256, rng.Next () % 256, rng.Next () % 256);
				stars [loop] = new star (nc, ((float)loop / num) * 5, 0);
			}
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

			// Upload the texture
			// Since we are sharing OpenGL objects between the full-screen and non-fullscreen contexts, we only need to do this once
			if (textureName == 0) {
				var path = NSBundle.MainBundle.PathForResource ("Star", "png");
				texture = new Texture (path);
				textureName = texture.TextureName;
			}

			// Set up texturing parameters
			GL.BindTexture (TextureTarget.Texture2D, textureName);

			// Loop through all the stars
			for (int loop = 0; loop < num; loop++) {

				// Reset The Current Modelview Matrix
				GL.LoadIdentity ();
				// Zoom into the screen (using the value in 'zoom')
				GL.Translate (0, 0, zoom);
				// Tilt the view (using the value int 'tilt')
				GL.Rotate (tilt, 1, 0, 0);

				// Rotate to the current stars angle
				GL.Rotate (stars [loop].angle, 0, 1, 0);

				// Move forward on the X Plane
				GL.Translate (stars [loop].distance, 0, 0);

				// Cancel the current stars angle
				GL.Rotate (-stars [loop].angle, 0, 1, 0);
				// Cancel the screen tilt
				GL.Rotate (-tilt, 1, 0, 0);

				if (twinkle) {
					// Assign a color 
					GL.Color3 (stars [(num - loop) - 1].color);
					GL.Begin (BeginMode.Quads);
					GL.TexCoord2 (0.0f, 0.0f);
					GL.Vertex3 (-1.0f, -1.0f, 0.0f);
					GL.TexCoord2 (1.0f, 0.0f);
					GL.Vertex3 (1.0f, -1.0f, 0.0f);
					GL.TexCoord2 (1.0f, 1.0f);
					GL.Vertex3 (1.0f, 1.0f, 0.0f);
					GL.TexCoord2 (0.0f, 1.0f);
					GL.Vertex3 (-1.0f, 1.0f, 0.0f);

					GL.End ();
				}

				GL.Rotate (spin, 0, 0, 1);
				// Assign a color 
				GL.Color3 (stars [loop].color);

				GL.Begin (BeginMode.Quads);

					GL.TexCoord2 (0.0f, 0.0f);
					GL.Vertex3 (-1.0f, -1.0f, 0.0f);
					GL.TexCoord2 (1.0f, 0.0f);
					GL.Vertex3 (1.0f, -1.0f, 0.0f);
					GL.TexCoord2 (1.0f, 1.0f);
					GL.Vertex3 (1.0f, 1.0f, 0.0f);
					GL.TexCoord2 (0.0f, 1.0f);
					GL.Vertex3 (-1.0f, 1.0f, 0.0f);

				GL.End ();

				spin += 0.01f;
				stars [loop].angle += (float)loop / num;
				stars [loop].distance -= 0.01f;

				if (stars [loop].distance < 0) {

					stars [loop].distance += 5;
					stars [loop].color = Color.FromArgb (rng.Next () % 256, rng.Next () % 256, rng.Next () % 256);
				}

			}

			return true;
		}

		public bool IsTwinkling {
			get { return twinkle; }
			set { twinkle = value; }
		}

		public float Tilt {
			get { return tilt; }
			set { tilt = value; }
		}

		public float Zoom {
			get { return zoom; }
			set { zoom = value; }
		}

	}
}

