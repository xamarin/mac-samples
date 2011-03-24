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

namespace NeHeLesson7
{
	public class Scene : NSObject
	{

		Texture texture;		// Texture
		int textureName;		// Texture Id

		float	xrot;			// X Rotation
		float	yrot;			// Y Rotation
		float	z = -5;			// Depth into the screen
		float	xspeed;			// X Rotation Speed
		float	yspeed;			// Y Rotation Speed		

		bool	isLight;		// Lighting ON / OFF

		int	filter;			// Which Filter To Use

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

		// This method renders our scene.
		// The main thing to note is that we've factored the drawing code out of the NSView subclass so that
		// the full-screen and non-fullscreen views share the same states for rendering 
		public bool DrawGLScene ()
		{
			// Clear The Screen And The Depth Buffer
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			// Reset The Current Modelview Matrix
			GL.LoadIdentity ();

			// Move Into The Screen 5 Units
			GL.Translate (0.0f, 0.0f, z);

			// Rotate On The X Axis
			GL.Rotate (xrot, 1, 0, 0);
			// Rotate On The Y Axis
			GL.Rotate (yrot, 0, 1, 0);

			// Upload the texture
			// Since we are sharing OpenGL objects between the full-screen and non-fullscreen contexts, we only need to do this once
			if (textureName == 0) {
				var path = NSBundle.MainBundle.PathForResource ("crate", "tga");
				texture = new Texture (path);

			}

			textureName = texture.TextureName [filter];			

			// Set up texturing parameters
			GL.BindTexture (TextureTarget.Texture2D, textureName);

			GL.Begin (BeginMode.Quads);				// Start Drawing Cube
				// Front Face
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 (-1.0f, -1.0f, 1.0f);	// Bottom Left Of The Texture and Quad
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 (1.0f, -1.0f, 1.0f);		// Bottom Right Of The Texture and Quad
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 (1.0f, 1.0f, 1.0f);		// Top Right Of The Texture and Quad
				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 (-1.0f, 1.0f, 1.0f);		// Top Left Of The Texture and Quad
				// Back Face
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 (-1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 (-1.0f, 1.0f, -1.0f);	// Top Right Of The Texture and Quad
				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 (1.0f, 1.0f, -1.0f);		// Top Left Of The Texture and Quad
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 (1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
				// Top Face
				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 (-1.0f, 1.0f, -1.0f);	// Top Left Of The Texture and Quad
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 (-1.0f, 1.0f, 1.0f);		// Bottom Left Of The Texture and Quad
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 (1.0f, 1.0f, 1.0f);		// Bottom Right Of The Texture and Quad
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 (1.0f, 1.0f, -1.0f);		// Top Right Of The Texture and Quad
				// Bottom Face
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 (-1.0f, -1.0f, -1.0f);	// Top Right Of The Texture and Quad
				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 (1.0f, -1.0f, -1.0f);	// Top Left Of The Texture and Quad
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 (1.0f, -1.0f, 1.0f);		// Bottom Left Of The Texture and Quad
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 (-1.0f, -1.0f, 1.0f);	// Bottom Right Of The Texture and Quad
				// Right face
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 (1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 (1.0f, 1.0f, -1.0f);		// Top Right Of The Texture and Quad
				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 (1.0f, 1.0f, 1.0f);		// Top Left Of The Texture and Quad
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 (1.0f, -1.0f, 1.0f);		// Bottom Left Of The Texture and Quad
				// Left Face
				GL.TexCoord2 (0.0f, 0.0f);
				GL.Vertex3 (-1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
				GL.TexCoord2 (1.0f, 0.0f);
				GL.Vertex3 (-1.0f, -1.0f, 1.0f);	// Bottom Right Of The Texture and Quad
				GL.TexCoord2 (1.0f, 1.0f);
				GL.Vertex3 (-1.0f, 1.0f, 1.0f);		// Top Right Of The Texture and Quad
				GL.TexCoord2 (0.0f, 1.0f);
				GL.Vertex3 (-1.0f, 1.0f, -1.0f);	// Top Left Of The Texture and Quad			

			GL.End ();						// Done Drawing the Cube

			// increase our rotation values
			xrot += xspeed;				// X Axis Rotation
			yrot += yspeed;				// Y Axis Rotation

			return true;
		}

		public bool IsLightOn {

			get { return isLight; }
			set { 
				isLight = value; 
				if (!isLight)
					GL.Disable (EnableCap.Lighting);
				else 
					GL.Enable (EnableCap.Lighting);
			}
		}

		public int Filter {
			get { return filter; }
			set { 
				filter = value;
				if (filter > 2)
					filter = 0;
			}
		}

		public float Distance {
			get { return z; }
			set { z = value; }
		}

		public float XSpeed {
			get { return xspeed; }
			set { xspeed = value; }
		}

		public float YSpeed {
			get { return yspeed; }
			set { yspeed = value; }
		}
	}
}

