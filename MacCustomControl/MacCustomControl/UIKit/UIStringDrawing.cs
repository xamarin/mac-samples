using System;
using System.Drawing;
using Foundation;
using AppKit;
using CoreGraphics;

namespace UIKit
{
	public static class UIStringDrawing
	{
		public static NSColor FillColor { get; set; } = NSColor.Black; 

		public static CGSize DrawString (this NSString item, CGRect rect, UIFont font, UILineBreakMode mode, UITextAlignment alignment) {

			// Get paragraph style
			var labelStyle = new NSMutableParagraphStyle(NSParagraphStyle.DefaultParagraphStyle.MutableCopy() as AppKit.NSMutableParagraphStyle);

			// Adjust alignment
			labelStyle.Alignment = alignment;

			// Adjust line break mode
			labelStyle.LineBreakMode = mode;

			// Define attributes
			var attributes = new NSStringAttributes () {
				Font = font.NSFont,
				ForegroundColor = UIStringDrawing.FillColor,
				ParagraphStyle = labelStyle
			};

			// Preform drawing
			item.DrawInRect(rect, attributes);

			// Return new bounding size
			return new CGSize (rect.Width, rect.Height);
		}

		public static CGRect GetBoundingRect (this NSString This, CGSize size, NSStringDrawingOptions options, UIStringAttributes attributes)
		{
			// Define attributes
			var attr = new NSMutableDictionary ();
			attr.Add (NSFont.NameAttribute, attributes.Font.NSFont);

			var rect = This.BoundingRectWithSize (size, options, attr);

			// HACK: Cheating on the height
			return new CGRect(rect.Left, rect.Top , rect.Width, rect.Height * 1.5f);
		}

		public static CGRect GetBoundingRect (this NSString This, CGSize size, NSStringDrawingOptions options, UIStringAttributes attributes, NSStringDrawingContext context)
		{
			// Define attributes
			var attr = new NSMutableDictionary ();
			attr.Add (NSFont.NameAttribute, attributes.Font.NSFont);

			var rect = This.BoundingRectWithSize (size, options, attr);

			// HACK: Cheating on the height
			return new CGRect(rect.Left, rect.Top , rect.Width, rect.Height * 1.5f);
		}
	}
}

