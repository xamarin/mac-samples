using System;
using SceneKit;

namespace SceneKitSessionWWDC2013
{
	public class SlideMaterials : Slide
	{
		private SCNNode SceneGraphDiagramNode { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Materials");

			TextManager.AddBulletAtLevel ("Determines the appearance of the geometry", 0);
			TextManager.AddBulletAtLevel ("SCNMaterial", 0);
			TextManager.AddBulletAtLevel ("Material properties", 0);
			TextManager.AddBulletAtLevel ("SCNMaterialProperty", 1);
			TextManager.AddBulletAtLevel ("Contents is a color or an image", 1);

			// Prepare the diagram but hide it for now
			SceneGraphDiagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();
			SlideSceneGraph.ScenegraphDiagramGoToStep (0);

			SceneGraphDiagramNode.Position = new SCNVector3 (3.0f, 8.0f, 0);
			SceneGraphDiagramNode.Opacity = 0.0f;

			ContentNode.AddChildNode (SceneGraphDiagramNode);
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.0f;
			SlideSceneGraph.ScenegraphDiagramGoToStep (5);
			SceneGraphDiagramNode.Opacity = 1.0f;
			SCNTransaction.Commit ();
		}
	}
}

