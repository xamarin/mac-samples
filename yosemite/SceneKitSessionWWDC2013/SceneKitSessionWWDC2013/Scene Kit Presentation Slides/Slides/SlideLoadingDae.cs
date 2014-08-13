using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideLoadingDae : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Add some text
			TextManager.SetTitle ("Loading a 3D Scene");
			TextManager.SetSubtitle ("DAE Documents");

			TextManager.AddBulletAtLevel ("Geometries", 0);
			TextManager.AddBulletAtLevel ("Animations", 0);
			TextManager.AddBulletAtLevel ("Textures", 0);
			TextManager.AddBulletAtLevel ("Lighting", 0);
			TextManager.AddBulletAtLevel ("Cameras", 0);
			TextManager.AddBulletAtLevel ("Skinning", 0);
			TextManager.AddBulletAtLevel ("Morphing", 0);

			// And an image resting on the ground
			var imageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/dae file icon", "png"), 10, false);
			imageNode.Position = new SCNVector3 (6, 4.5f, 1);
			GroundNode.AddChildNode (imageNode);
		}
	}
}

