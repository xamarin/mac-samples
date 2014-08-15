//
// CustomCatTextAttachmentCell.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using AppKit;
using Foundation;
using CoreGraphics;

namespace NSTextViewSample
{
	public class CustomCatTextAttachmentCell : NSTextAttachmentCell
	{
		const int padding = 10;
		static readonly Random random = new Random ();

		NSColor borderColor;

		public CustomCatTextAttachmentCell (NSImage image) : base (image)
		{
			borderColor = NSColor.FromDeviceHsba ((float)random.NextDouble (), 1f, 1f, 1f);
		}

		public override CGRect CellFrameForTextContainer (NSTextContainer textContainer, CGRect lineFrag, CGPoint position, nuint charIndex)
		{
			var rect = base.CellFrameForTextContainer (textContainer, lineFrag, position, charIndex);

			return new CGRect (
				rect.Location,
				new CGSize (
					rect.Width + padding * 2,
					rect.Height + padding * 2
				)
			);
		}

		public override void DrawWithFrame (CGRect cellFrame, NSView inView)
		{
			borderColor.SetFill ();

			NSGraphics.RectFill (
				new CGRect (
					cellFrame.X + padding / 2,
					cellFrame.Y + padding / 2,
					cellFrame.Width - padding,
					cellFrame.Height - padding
				)
			);

			base.DrawWithFrame (cellFrame, inView);
		}
	}
}