using System;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideMaterialProperties : Slide
	{
		private SCNNode EarthNode { get; set; }

		private SCNNode CloudsNode { get; set; }

		private SCNVector3 CameraOriginalPosition { get; set; }

		public override int NumberOfSteps ()
		{
			return 1;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and add some code
			TextManager.SetTitle ("Materials");

			TextManager.AddBulletAtLevel ("Diffuse", 0);
			TextManager.AddBulletAtLevel ("Ambient", 0);
			TextManager.AddBulletAtLevel ("Specular", 0);
			TextManager.AddBulletAtLevel ("Normal", 0);
			TextManager.AddBulletAtLevel ("Reflective", 0);
			TextManager.AddBulletAtLevel ("Emission", 0);
			TextManager.AddBulletAtLevel ("Transparent", 0);
			TextManager.AddBulletAtLevel ("Multiply", 0);

			// Create a node for Earth and another node to display clouds
			// Use the 'pivot' property to tilt Earth because we don't want to see the north pole.
			EarthNode = SCNNode.Create ();
			EarthNode.Pivot = SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (1, 0, 0), (float)(Math.PI * 0.1f));
			EarthNode.Position = new SCNVector3 (6, 7.2f, -2);
			EarthNode.Geometry = SCNSphere.Create (7.2f);

			CloudsNode = SCNNode.Create ();
			CloudsNode.Geometry = SCNSphere.Create (7.9f);

			GroundNode.AddChildNode (EarthNode);
			EarthNode.AddChildNode (CloudsNode);

			// Initially hide everything
			EarthNode.Opacity = 1.0f;
			CloudsNode.Opacity = 0.5f;

			EarthNode.Geometry.FirstMaterial.Ambient.Intensity = 1;
			EarthNode.Geometry.FirstMaterial.Normal.Intensity = 1;
			EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.2f;
			EarthNode.Geometry.FirstMaterial.Reflective.Contents = NSColor.White;
			EarthNode.Geometry.FirstMaterial.FresnelExponent = 3;

			EarthNode.Geometry.FirstMaterial.Emission.Intensity = 1;
			EarthNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/earth/earth-diffuse", "jpg"));

			EarthNode.Geometry.FirstMaterial.Shininess = 0.1f;
			EarthNode.Geometry.FirstMaterial.Specular.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/earth/earth-specular", "jpg"));
			EarthNode.Geometry.FirstMaterial.Specular.Intensity = 0.8f;

			EarthNode.Geometry.FirstMaterial.Normal.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/earth/earth-bump", "png"));
			EarthNode.Geometry.FirstMaterial.Normal.Intensity = 1.3f;

			EarthNode.Geometry.FirstMaterial.Emission.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/earth/earth-emissive", "jpg"));
			//EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 1.0f;

			// This effect can also be achieved with an image with some transparency set as the contents of the 'diffuse' property
			CloudsNode.Geometry.FirstMaterial.Transparent.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/earth/cloudsTransparency", "png"));
			CloudsNode.Geometry.FirstMaterial.TransparencyMode = SCNTransparencyMode.RgbZero;

			// Use a shader modifier to display an environment map independently of the lighting model used
			/*EarthNode.Geometry.ShaderModifiers = new SCNShaderModifiers { 
				EntryCGPointragment = " _output.color.rgb -= _surface.reflective.rgb * _lightingContribution.diffuse;"
				+ "_output.color.rgb += _surface.reflective.rgb;"
			};*/

			// Add animations
			var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
			rotationAnimation.Duration = 40.0f;
			rotationAnimation.RepeatCount = float.MaxValue;
			rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
			EarthNode.AddAnimation (rotationAnimation, new NSString ("earthNodeAnimation"));

			rotationAnimation.Duration = 100.0f;
			CloudsNode.AddAnimation (rotationAnimation, new NSString ("cloudsNodeAnimation"));

			//animate light
			var lightHandleNode = SCNNode.Create ();
			var lightNode = SCNNode.Create ();
			lightNode.Light = SCNLight.Create ();
			lightNode.Light.LightType = SCNLightType.Directional;
			lightNode.Light.CastsShadow = true;
			lightHandleNode.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, -(float)Math.PI * 2, 0, 12)));
			lightHandleNode.AddChildNode (lightNode);

			EarthNode.AddChildNode (lightHandleNode);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.0f;

			switch (index) {
			case 0:
				break;
			}

			SCNTransaction.Commit ();
		}
	}
}