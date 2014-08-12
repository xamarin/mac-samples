using System;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideLight : Slide
	{
		private SCNNode LightNode { get; set; }

		private SCNNode LightOffImageNode { get; set; }

		private SCNNode LightOnImageNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 2;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0;

			switch (index) {
			case 0:
				// Set the slide's title and subtile and add some text
				TextManager.SetTitle ("Node Attributes");
				TextManager.SetSubtitle ("SCNLights");

				TextManager.AddBulletAtLevel ("Four light types", 0);
				TextManager.AddBulletAtLevel ("Omni", 1);
				TextManager.AddBulletAtLevel ("Directional", 1);
				TextManager.AddBulletAtLevel ("Spot", 1);
				TextManager.AddBulletAtLevel ("Ambient", 1);

				// Add some code
				var codeExampleNode = TextManager.AddCode ("#aNode.#Light# = SCNLight.Create (); \naNode.Light.LightType = SCNLightType.Omni;#");
				codeExampleNode.Position = new SCNVector3 (12, 7, 1);

				// Add a light to the scene
				LightNode = SCNNode.Create ();
				LightNode.Light = SCNLight.Create ();
				LightNode.Light.LightType = SCNLightType.Omni;
				LightNode.Light.Color = NSColor.Black; // initially off
				LightNode.Light.SetAttribute (new NSNumber (30), SCNLightAttribute.AttenuationStartKey);
				LightNode.Light.SetAttribute (new NSNumber (40), SCNLightAttribute.AttenuationEndKey);
				LightNode.Position = new SCNVector3 (5, 3.5f, 0);
				ContentNode.AddChildNode (LightNode);

				// Load two images to help visualize the light (on and off)
				LightOffImageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/light-off", "tiff"), 7, false);
				LightOnImageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/light-on", "tiff"), 7, false);
				LightOnImageNode.Opacity = 0;

				LightNode.AddChildNode (LightOnImageNode);
				LightNode.AddChildNode (LightOffImageNode);
				break;
			case 1:
				// Switch the light on
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				LightNode.Light.Color = NSColor.FromCalibratedRgba (1, 1, 0.8f, 1);
				LightOnImageNode.Opacity = 1.0f;
				LightOffImageNode.Opacity = 0.0f;
				SCNTransaction.Commit ();
				break;
			}
			SCNTransaction.Commit ();
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			// Switch the light off
			LightNode.Light = null;
			SCNTransaction.Commit ();
		}
	}
}

