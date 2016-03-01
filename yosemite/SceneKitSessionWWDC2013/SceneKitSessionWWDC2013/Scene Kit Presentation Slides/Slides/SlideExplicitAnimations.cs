using System;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013 {
	public class SlideExplicitAnimations : Slide {
		SCNNode AnimatedNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 5;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Animations");
			TextManager.SetSubtitle ("Explicit animations");

			TextManager.AddCode ("#// Create an animation \n"
			+ "animation = #CABasicAnimation#.FromKeyPath (\"rotation\"); \n\n"
			+ "// Configure the animation \n"
			+ "animation.#Duration# = 2.0f; \n"
			+ "animation.#To# = NSValue.FromVector (new SCNVector4 (0, 1, 0, NMath.PI * 2)); \n"
			+ "animation.#RepeatCount# = float.MaxValue; \n\n"
			+ "// Play the animation \n"
			+ "aNode.#AddAnimation #(animation, \"myAnimation\");#");

			// A simple torus that we will animate to illustrate the code
			AnimatedNode = SCNNode.Create ();
			AnimatedNode.Position = new SCNVector3 (9, 5.7f, 16);

			// Use an extra node that we can tilt it and cumulate that with the animation
			var torusNode = SCNNode.Create ();
			torusNode.Geometry = SCNTorus.Create (4.0f, 1.5f);
			torusNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI * 0.5f));
			torusNode.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Cyan;
			torusNode.Geometry.FirstMaterial.Specular.Contents = NSColor.White;
			torusNode.Geometry.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap", "jpg"));
			torusNode.Geometry.FirstMaterial.FresnelExponent = 0.7f;

			AnimatedNode.AddChildNode (torusNode);
			ContentNode.AddChildNode (AnimatedNode);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			// Animate by default
			SCNTransaction.Begin ();

			switch (index) {
			case 0:
				// Disable animations for first step
				SCNTransaction.AnimationDuration = 0;

				// Initially dim the torus
				AnimatedNode.Opacity = 0.0f;

				TextManager.HighlightCodeChunks (null);
				break;
			case 1:
				TextManager.HighlightCodeChunks (new int[] { 0 });
				break;
			case 2:
				TextManager.HighlightCodeChunks (new int[] { 1, 2, 3 });
				break;
			case 3:
				TextManager.HighlightCodeChunks (new int[] { 4, 5 });
				break;
			case 4:
				SCNTransaction.AnimationDuration = 1.0f;

				// Show the torus
				AnimatedNode.Opacity = 1.0f;

				// Animate explicitly
				var animation = CABasicAnimation.FromKeyPath ("rotation");
				animation.Duration = 2.0f;
				animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				animation.RepeatCount = float.MaxValue;
				AnimatedNode.AddAnimation (animation, new NSString ("myAnimation"));

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				// Dim the text
				TextManager.TextNode.Opacity = 0.75f;

				presentationViewController.CameraHandle.Position = presentationViewController.CameraHandle.ConvertPositionToNode (new SCNVector3 (9, 8, 15), presentationViewController.CameraHandle.ParentNode);
				presentationViewController.CameraPitch.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 10));

				SCNTransaction.Commit ();
				break;
			}

			SCNTransaction.Commit ();
		}
	}
}

