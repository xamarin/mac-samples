using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideParticleEditor : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("3D Particle Editor");

			TextManager.AddBulletAtLevel ("Integrated into Xcode", 0);
			TextManager.AddBulletAtLevel ("Edit .scnp files", 0);
			TextManager.AddBulletAtLevel ("Particle templates available", 0);
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			// Bring up a screenshot of the editor
			var editorScreenshotNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/particleEditor", "png"), 14, true);
			editorScreenshotNode.Geometry.FirstMaterial.Diffuse.MipFilter = SCNFilterMode.Linear;
			editorScreenshotNode.Position = new SCNVector3 (17, 3.8f, 5);
			editorScreenshotNode.Rotation = new SCNVector4 (0, 1, 0, -(float)Math.PI / 1.5f);
			GroundNode.AddChildNode (editorScreenshotNode);

			// Animate it (rotate and move)
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			editorScreenshotNode.Position = new SCNVector3 (7, 3.8f, 5);
			editorScreenshotNode.Rotation = new SCNVector4 (0, 1, 0, -(float)Math.PI / 7);
			SCNTransaction.Commit ();
		}
	}
}

