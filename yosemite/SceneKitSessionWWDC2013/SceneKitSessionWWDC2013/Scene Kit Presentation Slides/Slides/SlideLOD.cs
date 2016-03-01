using System;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013 {
	public class SlideLOD : Slide {
		public override int NumberOfSteps ()
		{
			return 7;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Levels of Detail");

			((SCNView)presentationViewController.View).AllowsCameraControl = true;

			var intermediateNode = SCNNode.Create ();
			intermediateNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
			GroundNode.AddChildNode (intermediateNode);

			// Load two resolutions
			AddTeapot (0, -5, intermediateNode); // high res
			AddTeapot (4, +5, intermediateNode); // low res

			// Load the other resolutions but hide them
			for (var i = 1; i < 4; i++) {
				var teapotNode = AddTeapot (i, 5, intermediateNode);
				teapotNode.Opacity = 0.0f;
			}
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Hide everything (in case the user went backward)
				for (var i = 1; i < 4; i++) {
					var teapot = GroundNode.FindChildNode ("Teapot" + i, true);
					teapot.Opacity = 0.0f;
				}
				break;
			case 1:
				// Move the camera and adjust the clipping plane
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 3;
				presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 200);
				presentationViewController.CameraNode.Camera.ZFar = 500.0f;
				SCNTransaction.Commit ();
				break;
			case 2:
				// Revert to original position
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
				presentationViewController.CameraNode.Camera.ZFar = 100.0f;
				SCNTransaction.Commit ();
				break;
			case 3:
				var numberNodes = new SCNNode[] { AddNumberNode ("64k", -17),
					AddNumberNode ("6k", -9),
					AddNumberNode ("3k", -1),
					AddNumberNode ("1k", 6.5f),
					AddNumberNode ("256", 14)
				};
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;

				// Move the camera and the text
				presentationViewController.CameraHandle.Position = new SCNVector3 (presentationViewController.CameraHandle.Position.X, presentationViewController.CameraHandle.Position.Y + 6, presentationViewController.CameraHandle.Position.Z);
				TextManager.TextNode.Position = new SCNVector3 (TextManager.TextNode.Position.X, TextManager.TextNode.Position.Y + 6, TextManager.TextNode.Position.Z);

				// Show the remaining resolutions
				for (var i = 0; i < 5; i++) {
					var numberNode = numberNodes [i];
					numberNode.Position = new SCNVector3 (numberNode.Position.X, 7, -5);

					var teapot = GroundNode.FindChildNode ("Teapot" + i, true);
					teapot.Opacity = 1.0f;
					teapot.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI / 4));
					teapot.Position = new SCNVector3 ((i - 2) * 8, 5, teapot.Position.Z);
				}

				SCNTransaction.Commit ();
				break;
			case 4:
				presentationViewController.ShowsNewInSceneKitBadge (true);

				// Remove the numbers
				RemoveNumberNodes ();

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
					// Add some text and code
				TextManager.SetSubtitle ("SCNLevelOfDetail");

				TextManager.AddCode ("#var lod1 = SCNLevelOfDetail.#CreateWithWorldSpaceDistance# (aGeometry, aDistance); \n"
				+ "geometry.#LevelsOfDetail# = new SCNLevelOfDetail { lod1, lod2, ..., lodn };#");

				// Animation the merge
				for (int i = 0; i < 5; i++) {
					var teapot = GroundNode.FindChildNode ("Teapot" + i, true);

					teapot.Opacity = i == 0 ? 1.0f : 0.0f;
					teapot.Rotation = new SCNVector4 (0, 0, 1, 0);
					teapot.Position = new SCNVector3 (0, -5, teapot.Position.Z);
				}

				// Move the camera and the text
				presentationViewController.CameraHandle.Position = new SCNVector3 (presentationViewController.CameraHandle.Position.X, presentationViewController.CameraHandle.Position.Y - 3, presentationViewController.CameraHandle.Position.Z);
				TextManager.TextNode.Position = new SCNVector3 (TextManager.TextNode.Position.X, TextManager.TextNode.Position.Y - 3, TextManager.TextNode.Position.Z);

				SCNTransaction.Commit ();
				break;
			case 5:
				presentationViewController.ShowsNewInSceneKitBadge (false);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 3;
				// Change the lighting to remove the front light and rise the main light
				presentationViewController.UpdateLightingWithIntensities (new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.3f });
				presentationViewController.RiseMainLight (true);

				// Remove some text
				TextManager.FadeOutText (SlideTextManager.TextType.Title);
				TextManager.FadeOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				SCNTransaction.Commit ();

				// Retrieve the main teapot
				var maintTeapot = GroundNode.FindChildNode ("Teapot0", true);

				// The distances to use for each LOD
				var distances = new float[] { 30, 50, 90, 150 };

				// An array of SCNLevelOfDetail instances that we will build
				var levelsOfDetail = new SCNLevelOfDetail[4];
				for (var i = 1; i < 5; i++) {
					var teapotNode = GroundNode.FindChildNode ("Teapot" + i, true);
					var teapot = teapotNode.Geometry;

					// Unshare the material because we will highlight the different levels of detail with different colors in the next step
					teapot.FirstMaterial = (SCNMaterial)teapot.FirstMaterial.Copy ();

					// Build the SCNLevelOfDetail instance
					var levelOfDetail = SCNLevelOfDetail.CreateWithWorldSpaceDistance (teapot, distances [i - 1]);
					levelsOfDetail [i - 1] = levelOfDetail;
				}

				maintTeapot.Geometry.LevelsOfDetail = levelsOfDetail;

				// Duplicate and move the teapots
				var startTime = CAAnimation.CurrentMediaTime ();
				var delay = 0.2;

				var rowCount = 9;
				var columnCount = 12;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				// Change the far clipping plane to be able to see far away
				presentationViewController.CameraNode.Camera.ZFar = 1000.0;

				for (var j = 0; j < columnCount; j++) {
					for (var i = 0; i < rowCount; i++) {
						// Clone
						var clone = maintTeapot.Clone ();
						maintTeapot.ParentNode.AddChildNode (clone);

						// Animate
						var animation = CABasicAnimation.FromKeyPath ("position");
						animation.Additive = true;
						animation.Duration = 1.0;
						animation.To = NSValue.FromVector (new SCNVector3 ((i - rowCount / 2.0f) * 12.0f, 5 + (columnCount - j) * 15.0f, 0));
						animation.From = NSValue.FromVector (new SCNVector3 (0, 0, 0));
						animation.BeginTime = startTime + delay; // desynchronize

						// Freeze at the end of the animation
						animation.RemovedOnCompletion = false;
						animation.FillMode = CAFillMode.Forwards;

						clone.AddAnimation (animation, new NSString ("cloneAnimation"));

						// Animate the hidden property to automatically show the node when the position animation starts
						animation = CABasicAnimation.FromKeyPath ("hidden");
						animation.Duration = delay + 0.01;
						animation.FillMode = CAFillMode.Both;
						animation.From = new NSNumber (1);
						animation.To = new NSNumber (0);
						clone.AddAnimation (animation, new NSString ("cloneAnimation2"));

						delay += 0.05;
					}
				}
				SCNTransaction.Commit ();

				// Animate the camera while we duplicate the nodes
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0 + rowCount * columnCount * 0.05;

				var position = presentationViewController.CameraHandle.Position;
				presentationViewController.CameraHandle.Position = new SCNVector3 (position.X, position.Y + 5, position.Z);
				presentationViewController.CameraPitch.Rotation = new SCNVector4 (1, 0, 0, presentationViewController.CameraPitch.Rotation.W - ((float)(Math.PI / 4) * 0.1f));
				SCNTransaction.Commit ();
				break;
			case 6:
				// Highlight the levels of detail with colors
				var teapotChild = GroundNode.FindChildNode ("Teapot0", true);
				var colors = new NSColor[] { NSColor.Red, NSColor.Orange, NSColor.Yellow, NSColor.Green };

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				for (var i = 0; i < 4; i++) {
					var levelOfDetail = teapotChild.Geometry.LevelsOfDetail [i];
					levelOfDetail.Geometry.FirstMaterial.Multiply.Contents = colors [i];
				}
				SCNTransaction.Commit ();
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			// Reset the camera and lights before leaving this slide
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 2;
			presentationViewController.CameraNode.Camera.ZFar = 100.0f;
			SCNTransaction.Commit ();

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.5f;
			presentationViewController.RiseMainLight (false);
			SCNTransaction.Commit ();
		}

		private SCNNode AddTeapot (int index, float x, SCNNode parent)
		{
			var teapotNode = Utils.SCAddChildNode (parent, "Teapot" + index, "Scenes/lod/lod", 11);
			teapotNode.Geometry.FirstMaterial.Reflective.Intensity = 0.8f;
			teapotNode.Geometry.FirstMaterial.FresnelExponent = 1.0f;

			var yOffset = index == 4 ? 0.0f : index * 20.0f;
			teapotNode.Position = new SCNVector3 (x, -10 - yOffset, 0.1f);

			return teapotNode;
		}

		private SCNNode AddNumberNode (string numberString, float x)
		{
			var numberNode = Utils.SCLabelNode (numberString, Utils.LabelSize.Large, true);
			numberNode.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Orange;
			numberNode.Geometry.FirstMaterial.Ambient.Contents = NSColor.Orange;
			numberNode.Position = new SCNVector3 (x, 50, 0);
			numberNode.Name = "number";

			var text = (SCNText)numberNode.Geometry;
			text.ExtrusionDepth = 5;

			GroundNode.AddChildNode (numberNode);

			return numberNode;
		}

		void RemoveNumberNodes ()
		{
			// Move, fade and remove on completion
			foreach (var node in GroundNode.ChildNodes) {
				if (node.Name == "number") {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 1;
					SCNTransaction.SetCompletionBlock (node.RemoveFromParentNode);
					node.Opacity = 0.0f;
					node.Position = new SCNVector3 (node.Position.X, node.Position.Y, node.Position.Z - 20);
					SCNTransaction.Commit ();
				}
			}
		}
	}
}