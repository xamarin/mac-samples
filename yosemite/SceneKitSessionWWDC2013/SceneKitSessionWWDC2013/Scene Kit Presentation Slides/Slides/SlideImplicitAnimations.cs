using System;
using AppKit;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideImplicitAnimations : Slide
	{
		private SCNNode AnimatedNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Animations");
			TextManager.SetSubtitle ("Implicit animations");

			TextManager.AddCode ("#// Begin a transaction \n"
			+ "#SCNTransaction#.Begin (); \n"
			+ "#SCNTransaction#.AnimationDuration = 2.0f; \n\n"
			+ "// Change properties \n"
			+ "aNode.#Opacity# = 1.0f; \n"
			+ "aNode.#Rotation# = \n"
			+ " new SCNVector4 (0, 1, 0, NMath.PI * 4); \n\n"
			+ "// Commit the transaction \n"
			+ "SCNTransaction.#Commit ()#;#");

			// A simple torus that we will animate to illustrate the code
			AnimatedNode = SCNNode.Create ();
			AnimatedNode.Position = new SCNVector3 (10, 7, 0);

			// Use an extra node that we can tilt it and cumulate that with the animation
			var torusNode = SCNNode.Create ();
			torusNode.Geometry = SCNTorus.Create (4.0f, 1.5f);
			torusNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI * 0.7f));
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
				AnimatedNode.Opacity = 0.25f;

				TextManager.HighlightCodeChunks (null);
				break;
			case 1:
				TextManager.HighlightCodeChunks (new int[] { 0, 1 });
				break;
			case 2:
				TextManager.HighlightCodeChunks (new int[] { 2, 3 });
				break;
			case 3:
				TextManager.HighlightCodeChunks (new int[] { 4 });

				// Animate implicitly
				SCNTransaction.AnimationDuration = 2.0f;
				AnimatedNode.Opacity = 1.0f;
				AnimatedNode.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI * 4));
				break;
			}

			SCNTransaction.Commit ();
		}
	}
}

