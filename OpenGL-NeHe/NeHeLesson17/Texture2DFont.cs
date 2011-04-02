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

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;

namespace NeHeLesson17
{
	public class Texture2DFont : Texture2D
	{
		int baseDL;

		public Texture2DFont (string path) : base(path)
		{
			buildFont ();
		}

		void buildFont ()
		{
			float cx;
			float cy;
			int loop;

			baseDL = GL.GenLists (256);
			for (loop = 0; loop < 256; loop++) {
				cx = ((float)(loop % 16)) / 16.0f;
				cy = ((float)(loop / 16)) / 16.0f;

				GL.NewList (baseDL + loop, ListMode.Compile);
				GL.Begin (BeginMode.Quads);
				GL.TexCoord2 (cx, 1 - cy - 0.0625f);
				GL.Vertex2 (0, 0);

				GL.TexCoord2 (cx + 0.0625f, 1 - cy - 0.0625f);
				GL.Vertex2 (16, 0);

				GL.TexCoord2 (cx + 0.0625f, 1 - cy);
				GL.Vertex2 (16, 16);

				GL.TexCoord2 (cx, 1 - cy);
				GL.Vertex2 (0, 16);
				GL.End ();
				GL.Translate (10, 0, 0);
				GL.EndList ();

			}
		}

		public void RenderText (int x, int y, string text, int fontSet)
		{
			if (fontSet > 1)
				fontSet = 1;

			int[] viewport = new int[4];
			GL.GetInteger (GetPName.Viewport, viewport);
			GL.BindTexture (TextureTarget.Texture2D, TextureName);
			GL.Disable (EnableCap.DepthTest);
			GL.MatrixMode (MatrixMode.Projection);
			GL.PushMatrix ();     // Save the projection matrix
			GL.LoadIdentity ();   // Reset the projection matrix
			// Set up an ortho screen
			GL.Ortho (0, viewport [2], 0, viewport [3], -1, 1);
			GL.MatrixMode (MatrixMode.Modelview);
			GL.PushMatrix ();     // Save the modelview matrix
			GL.LoadIdentity ();   // Reset the modelview matrix
			GL.Translate (x, y, 0);   // Position the text
			GL.ListBase (baseDL - 32 + (128 * fontSet));   // Choose the font set

			// Convert our string into a byte array for CallLists
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding ();
			byte[] textBytes = encoding.GetBytes (text);

			// Write the text to the screen

			// Draws the display list text
			GL.CallLists (text.Length, ListNameType.UnsignedByte, textBytes);

			GL.MatrixMode (MatrixMode.Projection);
			GL.PopMatrix ();   // Restore the previous projection matrix
			GL.MatrixMode (MatrixMode.Modelview);
			GL.PopMatrix ();   // Restore the previous modelview matrix
			GL.Enable (EnableCap.DepthTest);			

		}		
	}

}