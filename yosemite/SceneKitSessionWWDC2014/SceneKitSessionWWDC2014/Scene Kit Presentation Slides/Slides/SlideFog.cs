using System;
using AppKit;
using SceneKit;

namespace SceneKitSessionWWDC2014
{
	public class SlideFog : Slide
	{
		public override int NumberOfSteps ()
		{
			return 3;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Fog");
			TextManager.SetSubtitle ("SCNScene");

			TextManager.AddEmptyLine ();
			TextManager.AddCode ("// set some fog\n\naScene.#fogColor# = aColor;\n\naScene.#fogStartDistance# = 50;\n\naScene.#fogEndDistance# = 100;#");

			//add palm trees
			var palmTree = Utils.SCAddChildNode (GroundNode, "PalmTree", "Scenes.scnassets/palmTree/palm_tree", 15);
			palmTree.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
			palmTree.Position = new SCNVector3 (4, -1, 0);

			palmTree = palmTree.Clone ();
			GroundNode.AddChildNode (palmTree);
			palmTree.Position = new SCNVector3 (0, -1, 7);

			palmTree = palmTree.Clone ();
			GroundNode.AddChildNode (palmTree);
			palmTree.Position = new SCNVector3 (8, -1, 13);

			palmTree = palmTree.Clone ();
			GroundNode.AddChildNode (palmTree);
			palmTree.Position = new SCNVector3 (13, -1, -7);

			palmTree = palmTree.Clone ();
			GroundNode.AddChildNode (palmTree);
			palmTree.Position = new SCNVector3 (-13, -1, -14);

			palmTree = palmTree.Clone ();
			GroundNode.AddChildNode (palmTree);
			palmTree.Position = new SCNVector3 (3, -1, -14);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;

			switch(index){
			case 0:
				break;
			case 1:
				//add a plan in the background
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.FadeOutText (SlideTextManager.TextType.Subtitle);

				var bg = SCNNode.Create ();
				var plane = SCNPlane.Create (100, 100);
				bg.Geometry = plane;
				bg.Position = new SCNVector3 (0, 0, -60);
				presentationViewController.CameraNode.AddChildNode (bg);

				((SCNView)presentationViewController.View).Scene.FogColor = NSColor.White;
				((SCNView)presentationViewController.View).Scene.FogStartDistance = 10;
				((SCNView)presentationViewController.View).Scene.FogEndDistance = 50;
				break;
			case 2:
				((SCNView)presentationViewController.View).Scene.FogDensityExponent = 0.3f;
				break;
			}

			SCNTransaction.Commit ();
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.5f;
			((SCNView)presentationViewController.View).Scene.FogColor = NSColor.Black;
			((SCNView)presentationViewController.View).Scene.FogEndDistance = 45.0f;
			((SCNView)presentationViewController.View).Scene.FogDensityExponent = 1.0f;
			((SCNView)presentationViewController.View).Scene.FogStartDistance = 40.0f;
			SCNTransaction.Commit ();
		}
	}
}

