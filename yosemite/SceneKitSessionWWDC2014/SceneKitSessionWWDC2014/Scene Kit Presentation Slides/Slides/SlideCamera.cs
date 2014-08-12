using System;
using System.Collections.Generic;

using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideCamera : Slide
	{
		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Create a node to own the "sign" model, make it to be close to the camera, rotate by 90 degree because it's oriented with z as the up axis
			var intermediateNode = SCNNode.Create ();
			intermediateNode.Position = new SCNVector3 (0, 0, 7);
			intermediateNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
			GroundNode.AddChildNode (intermediateNode);

			// Load the "sign" model
			var signNode = Utils.SCAddChildNode (intermediateNode, "sign", "Scenes.scnassets/intersection/intersection", 10);
			signNode.Position = new SCNVector3 (4, 0, 0.05f);

			// Re-parent every node that holds a camera otherwise they would inherit the scale from the "sign" model.
			// This is not a problem except that the scale affects the zRange of cameras and so it would be harder to get the transition from one camera to another right
			var cameraNodes = new List <SCNNode> ();
			foreach (SCNNode child in signNode) {
				if (child.Camera != null)
					cameraNodes.Add (child);
			}

			foreach (var cameraNode in cameraNodes) {
				var previousWorldTransform = cameraNode.WorldTransform;
				intermediateNode.AddChildNode (cameraNode); // re-parent
				cameraNode.Transform = intermediateNode.ConvertTransformFromNode (previousWorldTransform, null);
				cameraNode.Scale = new SCNVector3 (1, 1, 1);
			}

			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Node Attributes");
			TextManager.SetSubtitle ("SCNCamera");
			TextManager.AddBulletAtLevel ("Point of view for renderers", 0);

			TextManager.AddCode ("#aNode.#Camera# = #SCNCamera#.Create (); \naView.#PointOfView# = aNode;#");
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			switch (index) {
			case 0:
				break;
			case 1:
				// Switch to camera1
				SCNTransaction.AnimationDuration = 2.0f;
				((SCNView)presentationViewController.View).PointOfView = ContentNode.FindChildNode ("camera1", true);
				break;
			case 2:
				// Switch to camera2
				SCNTransaction.AnimationDuration = 2.0f;
				((SCNView)presentationViewController.View).PointOfView = ContentNode.FindChildNode ("camera2", true);
				break;
			case 3:
				// Switch back to the default camera
				SCNTransaction.AnimationDuration = 1.0f;
				((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
				break;
			}
			SCNTransaction.Commit ();
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			// Restore the default point of view before leaving this slide
			((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
			SCNTransaction.Commit ();
		}
	}
}

