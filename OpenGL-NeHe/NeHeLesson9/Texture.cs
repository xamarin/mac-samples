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
using MonoMac.CoreGraphics;
using MonoMac.OpenGL;

namespace NeHeLesson9
{
	public class Texture
	{
		int texId;
		int pboId;
		byte[] data;
		int width, height;

		private Texture () : base()
		{
		}

		public Texture (string path) : base()
		{
			GetImagaDataFromPath (path);
			LoadTexture ();
		}

		void GetImagaDataFromPath (string path)
		{
			NSImage src;
			CGImage image;
			CGContext context = null;

			src = new NSImage (path);

			image = src.AsCGImage (RectangleF.Empty, null, null);
			width = image.Width;
			height = image.Height;

			data = new byte[width * height * 4];

			CGImageAlphaInfo ai = CGImageAlphaInfo.PremultipliedLast;

			context = new CGBitmapContext (data, width, height, 8, 4 * width, image.ColorSpace, ai);

			// Core Graphics referential is upside-down compared to OpenGL referential
			// Flip the Core Graphics context here
			// An alternative is to use flipped OpenGL texture coordinates when drawing textures
			context.TranslateCTM (0, height);
			context.ScaleCTM (1, -1);

			// Set the blend mode to copy before drawing since the previous contents of memory aren't used. 
			// This avoids unnecessary blending.
			context.SetBlendMode (CGBlendMode.Copy);

			context.DrawImage (new RectangleF (0, 0, width, height), image);
		}

		void LoadTexture ()
		{
			GL.GenTextures (1, out texId);
			GL.GenBuffers (1, out pboId);

			// Bind the texture
			GL.BindTexture (TextureTarget.Texture2D, texId);

			// Bind the PBO
			GL.BindBuffer (BufferTarget.PixelUnpackBuffer, pboId);


			// Upload the texture data to the PBO
			GL.BufferData (BufferTarget.PixelUnpackBuffer, new IntPtr (width * height * 4 * sizeof(byte)), data, BufferUsageHint.StaticDraw);

			// Setup texture parameters
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

			GL.PixelStore (PixelStoreParameter.UnpackRowLength, 0);

			// OpenGL likes the GL_BGRA + GL_UNSIGNED_INT_8_8_8_8_REV combination
			// Use offset instead of pointer to indictate that we want to use data copied from a PBO 
			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
			data = null;

			GL.BindTexture (TextureTarget.Texture2D, 0);
			GL.BindBuffer (BufferTarget.PixelUnpackBuffer, 0);
		}

		public int TextureName {
			get { return texId; }
		}
	}
}

