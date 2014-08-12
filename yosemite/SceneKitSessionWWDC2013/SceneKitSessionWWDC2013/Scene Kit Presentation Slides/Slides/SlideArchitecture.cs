using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.SceneKit;

namespace SceneKitSessionWWDC2013
{
	public class SlideArchitecture : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Kit");
			TextManager.AddBulletAtLevel ("High level Objective-C API", 0);
			TextManager.AddBulletAtLevel ("Scene graph", 0);

			var redColor = NSColor.FromDeviceRgba (168.0f / 255.0f, 21.0f / 255.0f, 1.0f / 255.0f, 1);
			var grayColor = NSColor.Gray;
			var greenColor = NSColor.FromDeviceRgba (105.0f / 255.0f, 145.0f / 255.0f, 14.0f / 255.0f, 1);
			var orangeColor = NSColor.Orange;
			var purpleColor = NSColor.FromDeviceRgba (152.0f / 255.0f, 57.0f / 255.0f, 189.0f / 255.0f, 1);

			AddBox ("Cocoa", new RectangleF (0, 0, 500, 70), 3, grayColor);
			AddBox ("Core Image", new RectangleF (0, 0, 100, 70), 2, greenColor);
			AddBox ("Core Animation", new RectangleF (260, 0, 130, 70), 2, greenColor);
			AddBox ("GL Kit", new RectangleF (395, 0, 105, 70), 2, greenColor);
			AddBox ("Scene Kit", new RectangleF (105, 0, 150, 70), 2, orangeColor);
			AddBox ("OpenGL", new RectangleF (0, 0, 500, 70), 1, purpleColor);
			AddBox ("Graphics Hardware", new RectangleF (0, 0, 500, 70), 0, redColor);
		}

		private void AddBox (string title, RectangleF frame, int level, NSColor color)
		{
			var node = Utils.SCBoxNode (title, frame, color, 2.0f, true);
			node.Scale = new SCNVector3 (0.02f, 0.02f, 0.02f);
			node.Position = new SCNVector3 (-5, 1.5f * level, 10);
			ContentNode.AddChildNode (node);
		}
	}
}

