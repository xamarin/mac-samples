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

namespace NeHeLesson17
{
	public class Texture2D
	{
		int texId;
		byte[] data;
		int width, height;

		public Texture2D (string path)
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

			int bytesPerRow = image.BytesPerRow;
			int bitsPerPixel = image.BitsPerPixel;
			int bitsPerComponent = image.BitsPerComponent;

			data = new byte[bytesPerRow * height * 4];
			
			CGImageAlphaInfo ai = CGImageAlphaInfo.PremultipliedLast;
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			
			context = new CGBitmapContext (data, width, height, 8, 4 * width, colorSpace, ai);

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

			// Bind the texture
			GL.BindTexture (TextureTarget.Texture2D, texId);

			// Setup texture parameters
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

			GL.PixelStore (PixelStoreParameter.UnpackRowLength, 0);

			GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
			data = null;

			GL.BindTexture (TextureTarget.Texture2D, 0);
		}

		public int TextureName {
			get { return texId; }
		}
	}
}

