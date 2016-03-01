using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013 {
	public class SlideEditor : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Kit Editor");
			TextManager.AddBulletAtLevel ("Built into Xcode", 0);
			TextManager.AddBulletAtLevel ("Scene information", 0);
			TextManager.AddBulletAtLevel ("Preview final rendering", 0);
			TextManager.AddBulletAtLevel ("Scene graph inspection", 0);
			TextManager.AddBulletAtLevel ("Adjust lightning and materials", 0);
			TextManager.AddBulletAtLevel ("Preview animations and performance", 0);
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			// Bring up a screenshot of the editor
			var editorScreenshotNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/editor", "png"), 14, true);
			editorScreenshotNode.Position = new SCNVector3 (17, 4.1f, 5);
			editorScreenshotNode.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 1.5f));
			GroundNode.AddChildNode (editorScreenshotNode);

			// Animate it (rotate and move)
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			editorScreenshotNode.Position = new SCNVector3 (7.5f, 4.1f, 5);
			editorScreenshotNode.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 6.0f));
			SCNTransaction.Commit ();
		}
	}
}

