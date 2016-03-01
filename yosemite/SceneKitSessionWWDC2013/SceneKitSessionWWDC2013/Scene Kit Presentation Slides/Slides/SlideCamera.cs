using System;
using System.Collections.Generic;
using SceneKit;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideCamera : Slide
	{
		public override int NumberOfSteps ()
		{
			return 9;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Create a node to own the "sign" model, make it to be close to the camera, rotate by 90 degree because it's oriented with z as the up axis
			var intermediateNode = SCNNode.Create ();
			intermediateNode.Position = new SCNVector3 (0, 0, 7);
			intermediateNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
			GroundNode.AddChildNode (intermediateNode);

			// Load the "sign" model
			var signNode = Utils.SCAddChildNode (intermediateNode, "sign", "Scenes/intersection/intersection", 30);
			signNode.Position = new SCNVector3 (4, -2, 0.05f);

			// Re-parent every node that holds a camera otherwise they would inherit the scale from the "sign" model.
			// This is not a problem except that the scale affects the zRange of cameras and so it would be harder to get the transition from one camera to another right
			var cameraNodes = new List<SCNNode> ();
			foreach (SCNNode child in signNode) {
				if (child.Camera != null)
					cameraNodes.Add (child);
			}

			for (var i = 0; i < cameraNodes.Count; i++) {
				var cameraNode = cameraNodes [i];
				var previousWorldTransform = cameraNode.WorldTransform;
				intermediateNode.AddChildNode (cameraNode); // re-parent
				cameraNode.Transform = intermediateNode.ConvertTransformFromNode (previousWorldTransform, null);
				cameraNode.Scale = new SCNVector3 (1, 1, 1);
			}
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Node Attributes");
				TextManager.SetSubtitle ("Camera");
				TextManager.AddBulletAtLevel ("Point of view for renderers", 0);

				// Start with the "sign" model hidden
				var group = ContentNode.FindChildNode ("group", true);
				group.Scale = new SCNVector3 (0, 0, 0);
				group.Hidden = true;
				break;
			case 1:
				// Reveal the model (unhide then scale)
				group = ContentNode.FindChildNode ("group", true);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				group.Hidden = false;
				SCNTransaction.Commit ();

				SCNTransaction.AnimationDuration = 1.0f;
				group.Scale = new SCNVector3 (1, 1, 1);
				break;
			case 2:
				TextManager.AddCode ("#aNode.#Camera# = #SCNCamera#.Create (); \naView.#PointOfView# = aNode;#");
				break;
			case 3:
				// Switch to camera1
				SCNTransaction.AnimationDuration = 2.0f;
				((SCNView)presentationViewController.View).PointOfView = ContentNode.FindChildNode ("camera1", true);
				break;
			case 4:
				// Switch to camera2
				SCNTransaction.AnimationDuration = 2.0f;
				((SCNView)presentationViewController.View).PointOfView = ContentNode.FindChildNode ("camera2", true);
				break;
			case 5:
				// On completion add some code
				SCNTransaction.SetCompletionBlock (() => {
					TextManager.FadesIn = true;
					TextManager.AddEmptyLine ();
					TextManager.AddCode ("#aNode.#Camera#.XFov = angleInDegrees;#");
				});

				// Switch back to the default camera
				SCNTransaction.AnimationDuration = 1.0f;
				((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
				break;
			case 6:
				// Switch to camera 3
				SCNTransaction.AnimationDuration = 1.0f;
				var target = ContentNode.FindChildNode ("camera3", true);

				// Don't let the default transition animate the FOV (we will animate the FOV separately)
				var wantedFOV = target.Camera.XFov;
				target.Camera.XFov = ((SCNView)presentationViewController.View).PointOfView.Camera.XFov;

				// Animate point of view with an ease-in/ease-out function
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				((SCNView)presentationViewController.View).PointOfView = target;
				SCNTransaction.Commit ();

				// Animate the FOV with the default timing function (for a better looking transition)
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				((SCNView)presentationViewController.View).PointOfView.Camera.XFov = wantedFOV;
				SCNTransaction.Commit ();
				break;
			case 7:
				// Switch to camera 4
				var cameraNode = ContentNode.FindChildNode ("camera4", true);

				// Don't let the default transition animate the FOV (we will animate the FOV separately)
				wantedFOV = cameraNode.Camera.XFov;
				cameraNode.Camera.XFov = ((SCNView)presentationViewController.View).PointOfView.Camera.XFov;

				// Animate point of view with the default timing function
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				((SCNView)presentationViewController.View).PointOfView = cameraNode;
				SCNTransaction.Commit ();

				// Animate the FOV with an ease-in/ease-out function
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				((SCNView)presentationViewController.View).PointOfView.Camera.XFov = wantedFOV;
				SCNTransaction.Commit ();
				break;
			case 8:
				// Quickly switch back to the default camera
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
				SCNTransaction.Commit ();
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

