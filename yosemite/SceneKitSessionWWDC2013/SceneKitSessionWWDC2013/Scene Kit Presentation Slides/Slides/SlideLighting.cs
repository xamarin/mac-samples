using System;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013 {
	public class SlideLighting : Slide {
		SCNNode RoomNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Performance");
			TextManager.SetSubtitle ("Lighting");

			TextManager.AddBulletAtLevel ("Minimize the number of lights", 0);
			TextManager.AddBulletAtLevel ("Prefer static than dynamic shadows", 0);
			TextManager.AddBulletAtLevel ("Use material's \"multiply\" property", 0);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 1:
				// Load the scene
				var intermediateNode = SCNNode.Create ();
				intermediateNode.Position = new SCNVector3 (0.0f, 0.1f, -24.5f);
				intermediateNode.Scale = new SCNVector3 (2.3f, 1.0f, 1.0f);
				intermediateNode.Opacity = 0.0f;
				RoomNode = Utils.SCAddChildNode (intermediateNode, "Mesh", "Scenes/cornell-box/cornell-box", 15);
				ContentNode.AddChildNode (intermediateNode);

				// Hide the light maps for now
				foreach (var material in RoomNode.Geometry.Materials) {
					material.Multiply.Intensity = 0.0f;
					material.LightingModelName = SCNLightingModel.Blinn;
				}

				// Animate the point of view with an implicit animation.
				// On completion add to move the camera from right to left and back and forth.
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;

				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 2;

					SCNTransaction.SetCompletionBlock (() => {
						var animation = CABasicAnimation.FromKeyPath ("position");
						animation.Duration = 10.0f;
						animation.Additive = true;
						animation.To = NSValue.FromVector (new SCNVector3 (-5, 0, 0));
						animation.From = NSValue.FromVector (new SCNVector3 (5, 0, 0));
						animation.TimeOffset = -animation.Duration / 2;
						animation.AutoReverses = true;
						animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
						animation.RepeatCount = float.MaxValue;

						presentationViewController.CameraNode.AddAnimation (animation, new NSString ("myAnim"));
					});
					presentationViewController.CameraHandle.Position = presentationViewController.CameraHandle.ConvertPositionToNode (new SCNVector3 (0, +5, -30), presentationViewController.CameraHandle.ParentNode);
					presentationViewController.CameraPitch.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 4) * 0.2f);
					SCNTransaction.Commit ();
				});

				intermediateNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case 2:
				// Remove the lighting by using a constant lighing model (no lighting)
				foreach (var material in RoomNode.Geometry.Materials)
					material.LightingModelName = SCNLightingModel.Constant;
				break;
			case 3:
				// Activate the light maps smoothly
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				foreach (var material in RoomNode.Geometry.Materials)
					material.Multiply.Intensity = 1.0f;
				SCNTransaction.Commit ();
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			// Remove the animation from the camera and restore (animate) its position before leaving this slide
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0;
			presentationViewController.CameraNode.RemoveAnimation (new NSString ("myAnim"));
			presentationViewController.CameraNode.Position = presentationViewController.CameraNode.PresentationNode.Position;
			SCNTransaction.Commit ();

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
			SCNTransaction.Commit ();
		}
	}
}