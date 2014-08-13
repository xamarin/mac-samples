using System;
using System.Drawing;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideSceneGraphSummary : Slide
	{
		private SCNNode SunNode { get; set; }

		private SCNNode SunHaloNode { get; set; }

		private SCNNode EarthNode { get; set; }

		private SCNNode EarthGroupNode { get; set; }

		private SCNNode MoonNode { get; set; }

		private SCNNode WireframeBoxNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 6;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtitle
				TextManager.SetTitle ("Scene Graph");
				TextManager.SetSubtitle ("Summary");
				break;
			case 1:
				// A node that will help visualize the position of the stars
				WireframeBoxNode = SCNNode.Create ();
				WireframeBoxNode.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 4));
				WireframeBoxNode.Geometry = SCNBox.Create (1, 1, 1, 0);
				WireframeBoxNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/box_wireframe", "png"));
				WireframeBoxNode.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant; // no lighting
				WireframeBoxNode.Geometry.FirstMaterial.DoubleSided = true; // double sided

				// Sun
				SunNode = SCNNode.Create ();
				SunNode.Position = new SCNVector3 (0, 30, 0);
				ContentNode.AddChildNode (SunNode);
				SunNode.AddChildNode ((SCNNode)WireframeBoxNode.Copy ());

				// Earth-rotation (center of rotation of the Earth around the Sun)
				var earthRotationNode = SCNNode.Create ();
				SunNode.AddChildNode (earthRotationNode);

				// Earth-group (will contain the Earth, and the Moon)
				EarthGroupNode = SCNNode.Create ();
				EarthGroupNode.Position = new SCNVector3 (15, 0, 0);
				earthRotationNode.AddChildNode (EarthGroupNode);

				// Earth
				EarthNode = (SCNNode)WireframeBoxNode.Copy ();
				EarthNode.Position = new SCNVector3 (0, 0, 0);
				EarthGroupNode.AddChildNode (EarthNode);

				// Rotate the Earth around the Sun
				var animation = CABasicAnimation.FromKeyPath ("rotation");
				animation.Duration = 10.0f;
				animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				animation.RepeatCount = float.MaxValue;
				earthRotationNode.AddAnimation (animation, new NSString ("earth rotation around sun"));

				// Rotate the Earth
				animation = CABasicAnimation.FromKeyPath ("rotation");
				animation.Duration = 1.0f;
				animation.From = NSValue.FromVector (new SCNVector4 (0, 1, 0, 0));
				animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				animation.RepeatCount = float.MaxValue;
				EarthNode.AddAnimation (animation, new NSString ("earth rotation"));
				break;
			case 2:
				// Moon-rotation (center of rotation of the Moon around the Earth)
				var moonRotationNode = SCNNode.Create ();
				EarthGroupNode.AddChildNode (moonRotationNode);

				// Moon
				MoonNode = (SCNNode)WireframeBoxNode.Copy ();
				MoonNode.Position = new SCNVector3 (5, 0, 0);
				moonRotationNode.AddChildNode (MoonNode);

				// Rotate the moon around the Earth
				animation = CABasicAnimation.FromKeyPath ("rotation");
				animation.Duration = 1.5f;
				animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				animation.RepeatCount = float.MaxValue;
				moonRotationNode.AddAnimation (animation, new NSString ("moon rotation around earth"));

				// Rotate the moon
				animation = CABasicAnimation.FromKeyPath ("rotation");
				animation.Duration = 1.5f;
				animation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				animation.RepeatCount = float.MaxValue;
				MoonNode.AddAnimation (animation, new NSString ("moon rotation"));
				break;
			case 3:
				// Add geometries (spheres) to represent the stars
				SunNode.Geometry = SCNSphere.Create (2.5f);
				EarthNode.Geometry = SCNSphere.Create (1.5f);
				MoonNode.Geometry = SCNSphere.Create (0.75f);

				// Add a textured plane to represent Earth's orbit
				var earthOrbit = SCNNode.Create ();
				earthOrbit.Opacity = 0.4f;
				earthOrbit.Geometry = SCNPlane.Create (31, 31);
				earthOrbit.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/orbit", "png"));
				earthOrbit.Geometry.FirstMaterial.Diffuse.MipFilter = SCNFilterMode.Linear;
				earthOrbit.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
				earthOrbit.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant; // no lighting
				SunNode.AddChildNode (earthOrbit);
				break;
			case 4:
				// Add a halo to the Sun (a simple textured plane that does not write to depth)
				SunHaloNode = SCNNode.Create ();
				SunHaloNode.Geometry = SCNPlane.Create (30, 30);
				SunHaloNode.Rotation = new SCNVector4 (1, 0, 0, Pitch * (float)(Math.PI / 180.0f));
				SunHaloNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/sun-halo", "png"));
				SunHaloNode.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant; // no lighting
				SunHaloNode.Geometry.FirstMaterial.WritesToDepthBuffer = false; // do not write to depth
				SunHaloNode.Opacity = 0.2f;
				SunNode.AddChildNode (SunHaloNode);

				// Add materials to the stars
				EarthNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-diffuse-mini", "jpg"));
				EarthNode.Geometry.FirstMaterial.Emission.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-emissive-mini", "jpg"));
				EarthNode.Geometry.FirstMaterial.Specular.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/earth-specular-mini", "jpg"));
				MoonNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/moon", "jpg"));
				SunNode.Geometry.FirstMaterial.Multiply.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/sun", "jpg"));
				SunNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/earth/sun", "jpg"));
				SunNode.Geometry.FirstMaterial.Multiply.Intensity = 0.5f;
				SunNode.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;

				SunNode.Geometry.FirstMaterial.Multiply.WrapS =
						SunNode.Geometry.FirstMaterial.Diffuse.WrapS =
							SunNode.Geometry.FirstMaterial.Multiply.WrapT =
								SunNode.Geometry.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;

				EarthNode.Geometry.FirstMaterial.LocksAmbientWithDiffuse =
						MoonNode.Geometry.FirstMaterial.LocksAmbientWithDiffuse =
							SunNode.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;

				EarthNode.Geometry.FirstMaterial.Shininess = 0.1f;
				EarthNode.Geometry.FirstMaterial.Specular.Intensity = 0.5f;
				MoonNode.Geometry.FirstMaterial.Specular.Contents = NSColor.Gray;

				// Achieve a lava effect by animating textures
				animation = CABasicAnimation.FromKeyPath ("contentsTransform");
				animation.Duration = 10.0f;

				var animationTransform1 = CATransform3D.MakeTranslation (0, 0, 0);
				animationTransform1 = animationTransform1.Concat (CATransform3D.MakeScale (3, 3, 3));
				var animationTransform2 = CATransform3D.MakeTranslation (1, 0, 0);
				animationTransform2 = animationTransform1.Concat (CATransform3D.MakeScale (3, 3, 3));

				animation.From = NSValue.FromCATransform3D (animationTransform1);
				animation.To = NSValue.FromCATransform3D (animationTransform2);
				animation.RepeatCount = float.MaxValue;
				SunNode.Geometry.FirstMaterial.Diffuse.AddAnimation (animation, new NSString ("sun-texture"));

				animation = CABasicAnimation.FromKeyPath ("contentsTransform");
				animation.Duration = 30.0f;

				animationTransform1 = CATransform3D.MakeTranslation (0, 0, 0);
				animationTransform1 = animationTransform1.Concat (CATransform3D.MakeScale (5, 5, 5));
				animationTransform2 = CATransform3D.MakeTranslation (1, 0, 0);
				animationTransform2 = animationTransform1.Concat (CATransform3D.MakeScale (5, 5, 5));

				animation.From = NSValue.FromCATransform3D (animationTransform1);
				animation.To = NSValue.FromCATransform3D (animationTransform2);
				animation.RepeatCount = float.MaxValue;
				SunNode.Geometry.FirstMaterial.Multiply.AddAnimation (animation, new NSString ("sun-texture2"));
				break;
			case 5:
				// We will turn off all the lights in the scene and add a new light
				// to give the impression that the Sun lights the scene
				var lightNode = SCNNode.Create ();
				lightNode.Light = SCNLight.Create ();
				lightNode.Light.Color = NSColor.Black; // initially switched off
				lightNode.Light.LightType = SCNLightType.Omni;
				SunNode.AddChildNode (lightNode);

				// Configure attenuation distances because we don't want to light the floor
				lightNode.Light.SetAttribute (new NSNumber (20), SCNLightAttribute.AttenuationEndKey);
				lightNode.Light.SetAttribute (new NSNumber (19.5), SCNLightAttribute.AttenuationStartKey);

				// Animation
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				lightNode.Light.Color = NSColor.White; // switch on
				presentationViewController.UpdateLightingWithIntensities (new float[] { 0.0f }); //switch off all the other lights
				SunHaloNode.Opacity = 0.5f; // make the halo stronger
				SCNTransaction.Commit ();
				break;
			}
		}
	}
}