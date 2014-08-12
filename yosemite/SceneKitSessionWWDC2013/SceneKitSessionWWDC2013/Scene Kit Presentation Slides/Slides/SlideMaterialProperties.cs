using System;
using MonoMac.AppKit;
using MonoMac.SceneKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideMaterialProperties : Slide
	{
		private SCNNode EarthNode { get; set; }

		private SCNNode CloudsNode { get; set; }

		private SCNVector3 CameraOriginalPosition { get; set; }

		public override int NumberOfSteps ()
		{
			return 18;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
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
			EarthNode.Opacity = 0.0f;
			CloudsNode.Opacity = 0.0f;

			EarthNode.Geometry.FirstMaterial.Ambient.Intensity = 0;
			EarthNode.Geometry.FirstMaterial.Normal.Intensity = 0;
			EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0;
			EarthNode.Geometry.FirstMaterial.Emission.Intensity = 0;

			// Use a shader modifier to display an environment map independently of the lighting model used
			EarthNode.Geometry.ShaderModifiers = new SCNShaderModifiers { 
				EntryPointFragment = " _output.color.rgb -= _surface.reflective.rgb * _lightingContribution.diffuse;"
				+ "_output.color.rgb += _surface.reflective.rgb;"
			};

			// Add animations
			var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
			rotationAnimation.Duration = 40.0f;
			rotationAnimation.RepeatCount = float.MaxValue;
			rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
			EarthNode.AddAnimation (rotationAnimation, new NSString ("earthNodeAnimation"));

			rotationAnimation.Duration = 100.0f;
			CloudsNode.AddAnimation (rotationAnimation, new NSString ("cloudsNodeAnimation"));
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.0f;

			switch (index) {
			case 0:
				// Set the slide's title and add some code
				TextManager.SetTitle ("Materials");

				TextManager.AddBulletAtLevel ("Diffuse", 0);
				TextManager.AddBulletAtLevel ("Ambient", 0);
				TextManager.AddBulletAtLevel ("Specular and shininess", 0);
				TextManager.AddBulletAtLevel ("Normal", 0);
				TextManager.AddBulletAtLevel ("Reflective", 0);
				TextManager.AddBulletAtLevel ("Emission", 0);
				TextManager.AddBulletAtLevel ("Transparent", 0);
				TextManager.AddBulletAtLevel ("Multiply", 0);

				break;
			case 1:
				EarthNode.Opacity = 1.0f;

				presentationViewController.UpdateLightingWithIntensities (new float[] { 1 });
				break;
			case 2:
				EarthNode.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Blue;

				TextManager.HighlightBullet (0);
				ShowCodeExample ("#material.#diffuse.contents# = [NSColor blueColor];#", null, null);
				break;
			case 3:
				EarthNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-diffuse", "jpg"));

				ShowCodeExample ("#material.#diffuse.contents# =#", "earth-diffuse-mini", "jpg");
				break;
			case 4:
				EarthNode.Geometry.FirstMaterial.Ambient.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-diffuse", "jpg"));
				EarthNode.Geometry.FirstMaterial.Ambient.Intensity = 1;

				TextManager.HighlightBullet (1);
				ShowCodeExample ("#material.#ambient#.contents =#", "earth-diffuse-mini", "jpg");
				presentationViewController.UpdateLightingWithIntensities (LightIntensities);
				break;
			case 5:
				EarthNode.Geometry.FirstMaterial.Shininess = 0.1f;
				EarthNode.Geometry.FirstMaterial.Specular.Contents = NSColor.White;

				TextManager.HighlightBullet (2);
				ShowCodeExample ("#material.#specular#.contents = [NSColor whiteColor];#", null, null);
				break;
			case 6:
				EarthNode.Geometry.FirstMaterial.Specular.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-specular", "jpg"));

				ShowCodeExample ("#material.#specular#.contents =#", "earth-specular-mini", "jpg");
				break;
			case 7:
				EarthNode.Geometry.FirstMaterial.Normal.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-bump", "png"));
				EarthNode.Geometry.FirstMaterial.Normal.Intensity = 1.3f;

				TextManager.HighlightBullet (3);
				ShowCodeExample ("#material.#normal#.contents =#", "earth-bump", "png");
				break;
			case 8:
				EarthNode.Geometry.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-reflective", "jpg"));
				EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.7f;
				EarthNode.Geometry.FirstMaterial.Specular.Intensity = 0.0f;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 2.0f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				CameraOriginalPosition = presentationViewController.CameraHandle.Position;
				presentationViewController.CameraHandle.Position = presentationViewController.CameraHandle.ConvertPositionToNode (new SCNVector3 (6, 0, -10.11f), presentationViewController.CameraHandle.ParentNode);
				SCNTransaction.Commit ();

				TextManager.HighlightBullet (4);
				ShowCodeExample ("material.#reflective#.contents =", "earth-reflective", "jpg");
				break;
			case 9:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				presentationViewController.CameraHandle.Position = CameraOriginalPosition;
				SCNTransaction.Commit ();
				break;
			case 10:
				EarthNode.Geometry.FirstMaterial.Emission.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-emissive", "jpg"));
				EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.3f;
				EarthNode.Geometry.FirstMaterial.Emission.Intensity = 0.5f;

				TextManager.HighlightBullet (5);
				ShowCodeExample ("material.#emission#.contents =", "earth-emissive-mini2", "jpg");
				break;
			case 11:
				EarthNode.Geometry.FirstMaterial.Emission.Intensity = 1.0f;
				EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.1f;

				ShowCodeExample (null, null, null);
				presentationViewController.UpdateLightingWithIntensities (new float[] { 0.01f }); // keeping the intensity non null avoids an unnecessary shader recompilation
				break;
			case 12:
				EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.3f;

				presentationViewController.UpdateLightingWithIntensities (LightIntensities);
				break;
			case 13:
				EarthNode.Geometry.FirstMaterial.Emission.Intensity = 0.0f;
				CloudsNode.Opacity = 0.9f;

				TextManager.HighlightBullet (6);
				break;
			case 14:
					// This effect can also be achieved with an image with some transparency set as the contents of the 'diffuse' property
				CloudsNode.Geometry.FirstMaterial.Transparent.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/cloudsTransparency", "png"));
				CloudsNode.Geometry.FirstMaterial.TransparencyMode = SCNTransparencyMode.RgbZero;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				CloudsNode.Geometry.FirstMaterial.Transparency = 0;
				SCNTransaction.Commit ();

				CloudsNode.Geometry.FirstMaterial.Transparency = 1;

				ShowCodeExample ("material.#transparent#.contents =", "cloudsTransparency-mini", "png");
				break;
			case 15:
				EarthNode.Geometry.FirstMaterial.Multiply.Contents = NSColor.FromDeviceRgba (1.0f, (float)(204 / 255.0), (float)(102 / 255.0), 1);

				TextManager.HighlightBullet (7);
				ShowCodeExample ("material.#mutliply#.contents = [NSColor yellowColor];", null, null);
				break;
			case 16:
				EarthNode.Geometry.FirstMaterial.Emission.Intensity = 1.0f;
				EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.1f;

				ShowCodeExample (null, null, null);
				presentationViewController.UpdateLightingWithIntensities (new float[] { 0.01f });
				break;
			case 17:
				EarthNode.Geometry.FirstMaterial.Emission.Intensity = 0.0f;
				EarthNode.Geometry.FirstMaterial.Reflective.Intensity = 0.3f;

				presentationViewController.UpdateLightingWithIntensities (LightIntensities);
				break;
			}

			SCNTransaction.Commit ();
		}

		private void ShowCodeExample (string code, string imageName, string extension)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0;
			TextManager.FadeOutText (SlideTextManager.TextType.Code);

			if (code != null) {
				var codeNode = TextManager.AddCode (code);

				SCNVector3 min, max;
				min = new SCNVector3 (0, 0, 0);
				max = new SCNVector3 (0, 0, 0);
				codeNode.GetBoundingBox (ref min, ref max);

				if (imageName != null) {
					SCNNode imageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Scenes/earth/" + imageName, extension), 4.0f, false);
					imageNode.Position = new SCNVector3 (max.X + 2.5f, min.Y + 0.2f, 0);
					codeNode.AddChildNode (imageNode);

					max.X += 4.0f;
				}

				codeNode.Position = new SCNVector3 (6 - (min.X + max.X) / 2, 10 - min.Y, 0);
			}
			SCNTransaction.Commit ();
		}
	}
}