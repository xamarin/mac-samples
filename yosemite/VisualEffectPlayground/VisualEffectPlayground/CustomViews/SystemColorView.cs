using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace VisualEffectPlayground
{
	[Register ("SystemColorView")]
	public class SystemColorView : NSView
	{
		public bool DrawTitles {
			[Export("drawTitles")]
			get;
			[Export("setDrawTitles:")]
			set;
		}

		public bool DrawColors {
			[Export("drawColors")]
			get;
			[Export("setDrawColors:")]
			set;
		}

		public SystemColorView (IntPtr handle) : base (handle)
		{
			DrawColors = true;
			// You can easily tweak this option to always show titles
			DrawTitles = false;
		}

		public override void DrawRect (CGRect dirtyRect)
		{
			NSColorList colors = NSColorList.ColorListNamed ("System");
			CGRect rect = Bounds;
			rect.Height = 12;
			var style = (NSMutableParagraphStyle)NSParagraphStyle.DefaultParagraphStyle.MutableCopy ();
			style.Alignment = NSTextAlignment.Right;
			NSDictionary attrs = NSDictionary.FromObjectsAndKeys (
				                     new NSObject[] { NSFont.SystemFontOfSize (8), NSColor.LabelColor, style },
				                     new NSObject[] { NSAttributedString.FontAttributeName, NSAttributedString.ForegroundColorAttributeName, NSAttributedString.ParagraphStyleAttributeName }
			                     );

			foreach (NSString key in colors.AllKeys ()) {
				if (DrawColors) {
					NSColor color = colors.ColorWithKey (key);
					color.Set ();
					NSGraphics.RectFill (rect);
				}

				if (DrawTitles)
					key.DrawString (rect, attrs);

				rect.Y += 12;
			}
		}
	}
}

