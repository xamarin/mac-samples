using System;
using SceneKit;

namespace SceneKitSessionWWDC2014
{
	public class SlideDOF : Slide
	{
		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Depth of Field");
			TextManager.SetSubtitle ("SCNCamera");

			// Create a node that will contain the chess board
			var intermediateNode = SCNNode.Create ();
			intermediateNode.Scale = new SCNVector3 (35.0f, 35.0f, 35.0f);
			intermediateNode.Position = new SCNVector3 (0, 2.1f, 20);
			intermediateNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
			ContentNode.AddChildNode (intermediateNode);

			// Load the chess model and add to "intermediateNode"
			Utils.SCAddChildNode (intermediateNode, "Line01", "Scenes.scnassets/chess/chess", 1);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.5f;

			var cameraNode = presentationViewController.CameraNode;

			switch (index) {
			case 0:
				break;
			case 1:
				// Add a code snippet
				TextManager.AddCode ("#aCamera.#FocalDistance# = 16.0f; \n"
				+ "aCamera.#FocalBlurRadius# = 8.0f;#");
				break;
			case 2:
				// Turn on DOF to illustrate the code snippet
				cameraNode.Camera.FocalDistance = 16;
				cameraNode.Camera.FocalSize = 1.5f;
				cameraNode.Camera.Aperture = 0.3f;
				cameraNode.Camera.FocalBlurRadius = 8;
				break;
			case 3:
				// Focus far away
				cameraNode.Camera.FocalDistance = 35;
				cameraNode.Camera.FocalSize = 4;
				cameraNode.Camera.Aperture = 0.1f;

				// and update the code snippet
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.AddEmptyLine ();
				TextManager.AddCode ("#aCamera.#FocalDistance# = #35.0f#; \n"
				+ "aCamera.#FocalBlurRadius# = 8.0f;#");
				break;
			}

			SCNTransaction.Commit ();
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			// Restore camera settings before leaving this slide
			((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
			((SCNView)presentationViewController.View).PointOfView.Camera.FocalBlurRadius = 0;
		}
	}
}

