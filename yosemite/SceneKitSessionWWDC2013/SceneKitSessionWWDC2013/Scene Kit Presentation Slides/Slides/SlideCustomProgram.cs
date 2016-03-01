using System;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013 {
	public class SlideCustomProgram : Slide {
		SCNNode TorusNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 3;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Extending Scene Kit with OpenGL");
			TextManager.SetSubtitle ("Material custom program");

			TextManager.AddBulletAtLevel ("Custom GLSL code per material", 0);
			TextManager.AddBulletAtLevel ("Overrides Scene Kit’s rendering", 0);
			TextManager.AddBulletAtLevel ("Geometry attributes are provided", 0);
			TextManager.AddBulletAtLevel ("Transform uniforms are also provided", 0);

			// Add a torus and animate it
			TorusNode = Utils.SCAddChildNode (GroundNode, "torus", "Scenes/torus/torus", 10);
			TorusNode.Position = new SCNVector3 (8, 8, 4);
			TorusNode.Name = "object";

			var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
			rotationAnimation.Duration = 10.0f;
			rotationAnimation.RepeatCount = float.MaxValue;
			rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
			TorusNode.AddAnimation (rotationAnimation, new NSString ("torusRotation"));
		}
	}
}

