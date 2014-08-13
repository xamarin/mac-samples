using System;
using SceneKit;

namespace SceneKitSessionWWDC2013
{
	public class SlideManipulation2 : Slide
	{
		SCNNode sceneGraphDiagramNode;

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Manipulation");
			TextManager.SetSubtitle ("Retrieving a node");

			TextManager.AddCode ("#// Get by name \n"
			+ "var aNode = scene.RootNode \n"
			+ "            .#FindChildNode #(\"aName\", true);#");

			sceneGraphDiagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();
			SlideSceneGraph.ScenegraphDiagramGoToStep (7);

			sceneGraphDiagramNode.Opacity = 0.0f;
			sceneGraphDiagramNode.Position = new SCNVector3 (1.5f, 8.0f, 0);

			ContentNode.AddChildNode (sceneGraphDiagramNode);
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.0f;
			sceneGraphDiagramNode.Opacity = 1.0f;
			SCNTransaction.Commit ();
		}
	}
}

