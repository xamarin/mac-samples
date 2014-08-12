using System;
using SceneKit;

namespace SceneKitSessionWWDC2014
{
	public class SlideAssetsCollection : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Slide's title and subtitle
			TextManager.SetTitle ("Assets Catalog");
			TextManager.SetSubtitle (".scnassets folders");

			TextManager.AddBulletAtLevel ("Manage your assets", 0);
			TextManager.AddBulletAtLevel ("Add DAE files and referenced textures", 0);
			TextManager.AddBulletAtLevel ("Optimized at build time", 0);
			TextManager.AddBulletAtLevel ("Compilation options", 0);
			TextManager.AddBulletAtLevel ("Geometry interleaving", 1);
			TextManager.AddBulletAtLevel ("PVRTC, Up axis", 1);

			var intermediateNode = SCNNode.Create ();
			intermediateNode.Position = new SCNVector3 (0, 0, 7);
			GroundNode.AddChildNode (intermediateNode);

			// Load the "folder" model
			var folder = Utils.SCAddChildNode (intermediateNode, "folder", "Scenes.scnassets/assetCatalog/assetCatalog", 8);
			folder.Position = new SCNVector3 (5, 0, 2);
			folder.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 4) * 0.9f);
		}
	}
}