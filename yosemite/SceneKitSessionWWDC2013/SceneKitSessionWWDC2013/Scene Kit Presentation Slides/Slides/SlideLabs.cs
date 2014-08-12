using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.SceneKit;
using MonoMac.CoreFoundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideLabs : Slide
	{
		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			// Set the slide's title
			TextManager.SetTitle ("Labs");

			// Add two labs
			var lab1TitleNode = Utils.SCBoxNode ("Scene Kit Lab", new RectangleF (-375, -35, 750, 70), NSColor.FromCalibratedWhite (0.15f, 1.0f), 0.0f, false);
			lab1TitleNode.Scale = new SCNVector3 (0.02f, 0.02f, 0.02f);
			lab1TitleNode.Position = new SCNVector3 (-2.8f, 30.7f, 10.0f);
			lab1TitleNode.Rotation = new SCNVector4 (1, 0, 0, (float)(Math.PI));
			lab1TitleNode.Opacity = 0.0f;

			var lab2TitleNode = (SCNNode)lab1TitleNode.Copy ();
			lab2TitleNode.Position = new SCNVector3 (-2.8f, 29.2f, 10.0f);

			ContentNode.AddChildNode (lab1TitleNode);
			ContentNode.AddChildNode (lab2TitleNode);

			var lab1InfoNode = AddLabInfoNode ("\nGraphics and Games Lab A\nTuesday 4:00PM", 30.7f);
			var lab2InfoNode = AddLabInfoNode ("\nGraphics and Games Lab A\nWednesday 9:00AM", 29.2f);

			var delayInSeconds = 0.75;
			var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * Utils.NSEC_PER_SEC));
			DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				lab1TitleNode.Opacity = lab2TitleNode.Opacity = 1.0f;
				lab1TitleNode.Rotation = lab2TitleNode.Rotation = new SCNVector4 (1, 0, 0, 0);
				lab1InfoNode.Opacity = lab2InfoNode.Opacity = 1.0f;
				lab1InfoNode.Rotation = lab2InfoNode.Rotation = new SCNVector4 (0, 1, 0, 0);
				SCNTransaction.Commit ();
			});
		}

		private SCNNode AddLabInfoNode (string title, float yPosition)
		{
			var labInfoNode = Utils.SCBoxNode (title, new RectangleF (0, 0, 293.33f, 93.33f), NSColor.FromDeviceRgba (31 / 255, 31 / 255, 31 / 255, 1), 0.0f, false);
			labInfoNode.Scale = new SCNVector3 (0.015f, 0.015f, 0.015f);
			labInfoNode.Pivot = SCNMatrix4.CreateTranslation (new SCNVector3 (145.33f, 46.66f, 5));
			labInfoNode.Position = new SCNVector3 (6.9f, yPosition, 10.0f);
			labInfoNode.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI));
			labInfoNode.Opacity = 0.0f;

			var colorBox = Utils.SCBoxNode (null, new RectangleF (293.33f, 0, 40, 93.33f), NSColor.FromDeviceRgba (1, 214 / 255, 37 / 255, 1), 0.0f, false);
			colorBox.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;

			ContentNode.AddChildNode (labInfoNode);
			labInfoNode.AddChildNode (colorBox);

			return labInfoNode;
		}
	}
}

