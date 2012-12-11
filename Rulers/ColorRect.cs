using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace Rulers
{
	
	/// <summary>
	/// ColorRect is a lightweight class for splatting a colored rectangle
	/// into a RectsView (which actually does most of the work). A ColorRect
	/// can be locked down, so that the user can't move it, in which case it
	/// draws a little X in the middle. When the selected rect is locked,
	/// the RectsView doesn't allow the user to move the ruler markers.
	/// </summary>

	public class ColorRect : NSObject
	{
		public ColorRect (RectangleF frame, NSColor color) 
		{
			Frame = frame;
			Color = color;
		}
		
		public RectangleF Frame { get; set; }
		public NSColor Color { get; set; }
		public bool IsLocked { get; set; }
		
		public void DrawRect(RectangleF aRect, bool selected)
		{
			NSGraphics.RectClip (aRect);
			
			aRect.Intersect (Frame);
			
			Color.Set ();
			NSGraphics.RectFill (aRect);
			
		    if (selected) {
		        NSColor.Black.Set ();
		        NSGraphics.FrameRectWithWidth (Frame, 4.0f);
		    }
			
			if (IsLocked){
				float xSize = (Frame.Width > 10.0f) ? 5.0f : 3.0f;
				
				NSBezierPath path = new NSBezierPath ();
				
				NSColor.Black.Set ();
				path.LineWidth = 3.0f;
				path.MoveTo (new PointF (MidX (Frame) - xSize, MidY (Frame) - xSize));
				path.LineTo (new PointF (MidX (Frame) + xSize, MidY (Frame) + xSize));
				path.MoveTo (new PointF (MidX (Frame) - xSize, MidY (Frame) + xSize));
				path.LineTo (new PointF (MidX (Frame) + xSize, MidY (Frame) - xSize));
				path.Stroke ();
				
			}
	
		}
		
		static float MidX (RectangleF rect)
		{
			return rect.X + (rect.Width / 2);	
		}
		
		static float MidY (RectangleF rect)
		{
			return rect.Y + (rect.Height / 2);	
		}		
	}
}

