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

namespace NeHeLesson13
{
	public class BitmapFont
	{

		NSFont font;
		string fontName;
		float fontSize;
		
		// Base display list for the font set
		int baseDL;

		public BitmapFont (string fontName,float fontSize)
		{
			this.fontName = fontName;
			this.fontSize = fontSize;
			
			BuildFont();
		}

		void BuildFont ()
		{
			// 95 since if we do 96, we get the delete character...
			baseDL = GL.GenLists (95);
			font = NSFont.FromFontName (fontName, fontSize);
			if (font == null)
				Console.WriteLine ("Unable to create font: " + fontName);
			else
				MakeGLDisplayListFirst (' ', 95, baseDL);
		}
		
		// Create the set of display lists for the bitmaps
		bool MakeGLDisplayListFirst (char first, int count, int baseDL)
		{

			int curListIndex;
			NSColor blackColor;
			NSMutableDictionary attribDict;
			int dListNum;
			NSString currentChar;
			char currentUnichar;
			SizeF charSize;
			RectangleF charRect;
			NSImage theImage;
			bool retval;

			// Make sure the list isn't already under construction
			GL.GetInteger (GetPName.ListIndex, out curListIndex);
			if (curListIndex != 0) {
				Console.WriteLine ("Display list already under construction");
				return false;
			}

			// Save pixel unpacking state
			GL.PushClientAttrib (ClientAttribMask.ClientPixelStoreBit);

			GL.PixelStore (PixelStoreParameter.UnpackSwapBytes, 0);
			GL.PixelStore (PixelStoreParameter.UnpackLsbFirst, 0);
			GL.PixelStore (PixelStoreParameter.UnpackSkipPixels, 0);
			GL.PixelStore (PixelStoreParameter.UnpackSkipRows, 0);
			GL.PixelStore (PixelStoreParameter.UnpackRowLength, 0);
			GL.PixelStore (PixelStoreParameter.UnpackAlignment, 0);

			blackColor = NSColor.Black;

			attribDict = new NSMutableDictionary ();
			attribDict.SetValueForKey (font, NSAttributedString.FontAttributeName);
			attribDict.SetValueForKey (NSColor.White, NSAttributedString.ForegroundColorAttributeName);
			attribDict.SetValueForKey (blackColor, NSAttributedString.BackgroundColorAttributeName);

			charRect.Location.X = charRect.Location.Y = 0;

			theImage = new NSImage (new SizeF (0,0));
			retval = true;

			for (dListNum = baseDL, currentUnichar = first; currentUnichar < first + count; 
				dListNum++, currentUnichar++) {

				currentChar = new NSString (Char.ToString (currentUnichar));
				charSize = currentChar.StringSize (attribDict);
				charRect.Size = charSize;
				charRect = charRect.Integral ();
				if (charRect.Size.Width > 0 && charRect.Size.Height > 0) {

					theImage.Size = charRect.Size;
					theImage.LockFocus ();
					NSGraphicsContext.CurrentContext.ShouldAntialias = false;
					blackColor.Set ();
					NSBezierPath.FillRect (charRect);
					currentChar.DrawString (charRect, attribDict);
					theImage.UnlockFocus ();

					if (!MakeDisplayList(dListNum, theImage)) {
						retval = false;
						break;
					}
				}
			}
			return retval;
		}
		
		// Create one display list based on the given image.  This assumes the image
		// uses 8-bit chunks to represent a sample
		bool MakeDisplayList (int listNum, NSImage theImage)
		{

			NSBitmapImageRep bitmap;
			int bytesPerRow, pixelsHigh, pixelsWide, samplesPerPixel;
			byte currentBit, byteValue;
			byte[] newBuffer;
			int rowIndex, colIndex;

			bitmap = new NSBitmapImageRep ( theImage.AsTiff (NSTiffCompression.None, 0) );

			pixelsHigh = bitmap.PixelsHigh;
			pixelsWide = bitmap.PixelsWide;

			bytesPerRow = bitmap.BytesPerRow;
			samplesPerPixel = bitmap.SamplesPerPixel;

			newBuffer = new byte[(int)Math.Ceiling ((float)bytesPerRow / 8.0) * pixelsHigh];

			byte[] bitmapBytesArray = new byte[(pixelsWide * pixelsHigh) * samplesPerPixel];
			System.Runtime.InteropServices.Marshal.Copy (bitmap.BitmapData, bitmapBytesArray, 0, (pixelsWide * pixelsHigh) * samplesPerPixel);
			
			int curIdx = 0;
			
			/*
			* Convert the color bitmap into a true bitmap, ie, one bit per pixel.  We
			* read at last row, write to first row as Cocoa and OpenGL have opposite
			* y origins
			*/
			for (rowIndex = pixelsHigh - 1; rowIndex >= 0; rowIndex--) {

				currentBit = 0x80;
				byteValue = 0;
				for (colIndex = 0; colIndex < pixelsWide; colIndex++) {
					
					if (bitmapBytesArray [rowIndex * bytesPerRow + colIndex * samplesPerPixel] > 0)
						byteValue |= currentBit;
					currentBit >>= 1;
					if (currentBit == 0) {
						newBuffer [curIdx++] = byteValue;
						currentBit = 0x80;
						byteValue = 0;
					}
				}

				/*
				* Fill out the last byte; extra is ignored by OpenGL, but each row
				* must start on a new byte
				*/
				if (currentBit != 0x80)
					newBuffer[curIdx++] = byteValue;				
			}
			
			GL.NewList( listNum, ListMode.Compile);
			GL.Bitmap(pixelsWide, pixelsHigh, 0, 0, pixelsWide, 0, newBuffer);
			GL.EndList();
			return true;
		}	
		
		// Writes a text string out based on this objects font settings.
		public void RenderText (string text)
		{
			// Pushes the display list bits
			GL.PushAttrib (AttribMask.ListBit);
			// Sets the base character to space ' '
			GL.ListBase (baseDL -' ');
			
			// Convert our string into a byte array for CallLists
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding ();
			byte[] textBytes = encoding.GetBytes (text);
			
			// Draws the display list text
			GL.CallLists (text.Length, ListNameType.UnsignedByte, textBytes);
			
			// Pops the display list bits
			GL.PopAttrib ();

		}		
	}
}

