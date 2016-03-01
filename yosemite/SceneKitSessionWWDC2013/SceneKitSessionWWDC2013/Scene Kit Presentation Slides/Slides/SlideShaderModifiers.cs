using System;
using System.IO;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013 {
	public class SlideShaderModifiers : Slide {
		SCNNode PlaneNode { get; set; }

		SCNNode SphereNode { get; set; }

		SCNNode TorusNode { get; set; }

		SCNNode XRayNode { get; set; }

		SCNNode VirusNode { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Shader Modifiers");

			TextManager.AddBulletAtLevel ("Inject custom GLSL code", 0);
			TextManager.AddBulletAtLevel ("Combines with Scene Kit’s shaders", 0);
			TextManager.AddBulletAtLevel ("Inject at specific stages", 0);
		}

		public override int NumberOfSteps ()
		{
			return 15;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();

			switch (index) {
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.SetSubtitle ("API");

				TextManager.AddEmptyLine ();
				TextManager.AddCode ("#aMaterial.#ShaderModifiers# = new SCNShaderModifiers {\n"
				+ "     <Entry Point> = <GLSL Code>\n"
				+ "};#");
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				break;
			case 2:
				TextManager.FlipOutText (SlideTextManager.TextType.Code);

				TextManager.AddEmptyLine ();
				TextManager.AddCode ("#aMaterial.#ShaderModifiers# = new SCNShaderModifiers { \n"
				+ "     EntryCGPointragment = \n"
				+ "     new Vector3 (1.0f) - #output#.Color.GetRgb () \n"
				+ "};#");

				TextManager.FlipInText (SlideTextManager.TextType.Code);

				break;
			case 3:
				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				TextManager.SetSubtitle ("Entry points");

				TextManager.AddBulletAtLevel ("Geometry", 0);
				TextManager.AddBulletAtLevel ("Surface", 0);
				TextManager.AddBulletAtLevel ("Lighting", 0);
				TextManager.AddBulletAtLevel ("Fragment", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				break;
			case 4:
				SCNTransaction.AnimationDuration = 1;

				TextManager.HighlightBullet (0);

				// Create a (very) tesselated plane
				var plane = SCNPlane.Create (10, 10);
				plane.WidthSegmentCount = 200;
				plane.HeightSegmentCount = 200;

				// Setup the material (same as the floor)
				plane.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Mirror;
				plane.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Mirror;
				plane.FirstMaterial.Diffuse.Contents = new NSImage ("/Library/Desktop Pictures/Circles.jpg");
				plane.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (0, 0, 1), NMath.PI / 4);
				plane.FirstMaterial.Specular.Contents = NSColor.White;
				plane.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap", "jpg"));
				plane.FirstMaterial.Reflective.Intensity = 0.0f;

				// Create a node to hold that plane
				PlaneNode = SCNNode.Create ();
				PlaneNode.Position = new SCNVector3 (0, 0.1f, 0);
				PlaneNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
				PlaneNode.Scale = new SCNVector3 (5, 5, 1);
				PlaneNode.Geometry = plane;
				ContentNode.AddChildNode (PlaneNode);

				// Attach the "wave" shader modifier, and set an initial intensity value of 0
				var shaderFile = NSBundle.MainBundle.PathForResource ("Shaders/wave", "shader");
				var geometryModifier = File.ReadAllText (shaderFile);
				PlaneNode.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointGeometry = geometryModifier };
				PlaneNode.Geometry.SetValueForKey (new NSNumber (0.0f), new NSString ("intensity"));

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
					// Show the pseudo code for the deformation
				var textNode = TextManager.AddCode ("#float len = #geometry#.Position.Xy.Length;\n"
				               + "aMaterial.ShaderModifiers = new SCNShaderModifiers { \n"
				               + "     #EntryPointGeometry# = geometry.Position.Y \n"
				               + "};#");

				textNode.Position = new SCNVector3 (8.5f, 7, 0);
				SCNTransaction.Commit ();
				break;
			case 5:
				// Progressively increase the intensity
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 2;
				PlaneNode.Geometry.SetValueForKey (new NSNumber (1.0f), new NSString ("intensity"));
				PlaneNode.Geometry.FirstMaterial.Reflective.Intensity = 0.3f;
				SCNTransaction.Commit ();

				// Redraw forever
				((SCNView)presentationViewController.View).Playing = true;
				((SCNView)presentationViewController.View).Loops = true;
				break;
			case 6:
				SCNTransaction.AnimationDuration = 1;

				TextManager.FadeOutText (SlideTextManager.TextType.Code);

				// Hide the plane used for the previous modifier
				PlaneNode.Geometry.SetValueForKey (new NSNumber (0.0f), new NSString ("intensity"));
				PlaneNode.Geometry.FirstMaterial.Reflective.Intensity = 0.0f;
				PlaneNode.Opacity = 0.0f;

				// Create a sphere to illustrate the "car paint" modifier
				var sphere = SCNSphere.Create (6);
				sphere.SegmentCount = 100;
				sphere.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/noise", "png"));
				sphere.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
				sphere.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;
				sphere.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap3", "jpg"));
				sphere.FirstMaterial.FresnelExponent = 1.3f;

				SphereNode = SCNNode.FromGeometry (sphere);
				SphereNode.Position = new SCNVector3 (5, 6, 0);
				GroundNode.AddChildNode (SphereNode);

				// Attach the "car paint" shader modifier
				shaderFile = NSBundle.MainBundle.PathForResource ("Shaders/carPaint", "shader");
				var surfaceModifier = File.ReadAllText (shaderFile);
				sphere.FirstMaterial.ShaderModifiers = new SCNShaderModifiers { EntryPointSurface = surfaceModifier };

				var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
				rotationAnimation.Duration = 15.0f;
				rotationAnimation.RepeatCount = float.MaxValue;
				rotationAnimation.By = NSValue.FromVector (new SCNVector4 (0, 1, 0, -(float)(Math.PI * 2)));
				SphereNode.AddAnimation (rotationAnimation, new NSString ("sphereNodeAnimation"));

				TextManager.HighlightBullet (1);
				break;
			case 7:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.5f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				// Move the camera closer
				presentationViewController.CameraNode.Position = new SCNVector3 (5, -0.5f, -17);
				SCNTransaction.Commit ();
				break;
			case 8:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				// Move back
				presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
				SCNTransaction.Commit ();
				break;
			case 9:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				// Hide the sphere used for the previous modifier
				SphereNode.Opacity = 0.0f;
				SphereNode.Position = new SCNVector3 (6, 4, -8);
				SCNTransaction.Commit ();

				SCNTransaction.AnimationDuration = 0;

				TextManager.HighlightBullet (2);

				// Load the model, animate
				var intermediateNode = SCNNode.Create ();
				intermediateNode.Position = new SCNVector3 (4, 0.1f, 10);
				TorusNode = Utils.SCAddChildNode (intermediateNode, "torus", "Scenes/torus/torus", 11);

				rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
				rotationAnimation.Duration = 10.0f;
				rotationAnimation.RepeatCount = float.MaxValue;
				rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				TorusNode.AddAnimation (rotationAnimation, new NSString ("torusNodeAnimation"));

				GroundNode.AddChildNode (intermediateNode);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				intermediateNode.Position = new SCNVector3 (4, 0.1f, 0);
				SCNTransaction.Commit ();

				break;
			case 10:
				// Attach the shader modifier
				shaderFile = NSBundle.MainBundle.PathForResource ("Shaders/toon", "shader");
				var lightingModifier = File.ReadAllText (shaderFile);
				TorusNode.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointLightingModel = lightingModifier };
				break;
			case 11:
				SCNTransaction.AnimationDuration = 1.0f;

				// Hide the torus used for the previous modifier
				TorusNode.Position = new SCNVector3 (TorusNode.Position.X, TorusNode.Position.Y, TorusNode.Position.Z - 10);
				TorusNode.Opacity = 0.0f;

				// Load the model, animate
				intermediateNode = SCNNode.Create ();
				intermediateNode.Position = new SCNVector3 (4, -2.6f, 14);
				intermediateNode.Scale = new SCNVector3 (70, 70, 70);

				XRayNode = Utils.SCAddChildNode (intermediateNode, "node", "Scenes/bunny", 12);
				XRayNode.Position = new SCNVector3 (0, 0, 0);
				XRayNode.Opacity = 0.0f;

				GroundNode.AddChildNode (intermediateNode);

				rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
				rotationAnimation.Duration = 10.0f;
				rotationAnimation.RepeatCount = float.MaxValue;
				rotationAnimation.From = NSValue.FromVector (new SCNVector4 (0, 1, 0, 0));
				rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)));
				intermediateNode.AddAnimation (rotationAnimation, new NSString ("bunnyNodeAnimation"));

				TextManager.HighlightBullet (3);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				XRayNode.Opacity = 1.0f;
				intermediateNode.Position = new SCNVector3 (4, -2.6f, -2);
				SCNTransaction.Commit ();
				break;
			case 12:
				// Attach the "x ray" modifier
				shaderFile = NSBundle.MainBundle.PathForResource ("Shaders/xRay", "shader");
				var fragmentModifier = File.ReadAllText (shaderFile);
				XRayNode.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointFragment = fragmentModifier };
				XRayNode.Geometry.FirstMaterial.ReadsFromDepthBuffer = false;
				break;
			case 13:
				// Highlight everything
				TextManager.HighlightBullet (-1);

				// Hide the node used for the previous modifier
				XRayNode.Opacity = 0.0f;
				XRayNode.ParentNode.Position = new SCNVector3 (4, -2.6f, -5);

				// Create the model
				sphere = SCNSphere.Create (5);
				sphere.SegmentCount = 150; // tesselate a lot

				VirusNode = SCNNode.FromGeometry (sphere);
				VirusNode.Position = new SCNVector3 (3, 6, 0);
				VirusNode.Rotation = new SCNVector4 (1, 0, 0, Pitch * (float)(Math.PI / 180.0f));
				GroundNode.AddChildNode (VirusNode);

				// Set the shader modifiers
				var geomFile = NSBundle.MainBundle.PathForResource ("Shaders/sm_geom", "shader");
				var surfFile = NSBundle.MainBundle.PathForResource ("Shaders/sm_surf", "shader");
				var lightFile = NSBundle.MainBundle.PathForResource ("Shaders/sm_light", "shader");
				var fragFile = NSBundle.MainBundle.PathForResource ("Shaders/sm_frag", "shader");
				geometryModifier = File.ReadAllText (geomFile);
				surfaceModifier = File.ReadAllText (surfFile);
				lightingModifier = File.ReadAllText (lightFile);
				fragmentModifier = File.ReadAllText (fragFile);
				VirusNode.Geometry.FirstMaterial.ShaderModifiers = new SCNShaderModifiers { 
					EntryPointGeometry = geometryModifier,
					EntryPointSurface = surfaceModifier,
					EntryPointLightingModel = lightingModifier,
					EntryPointFragment = fragmentModifier
				};
				break;
			case 14:
				SCNTransaction.AnimationDuration = 1.0f;

				// Hide the node used for the previous modifier
				VirusNode.Opacity = 0.0f;
				VirusNode.Position = new SCNVector3 (3, 6, -10);

				// Change the text
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				TextManager.SetSubtitle ("SCNShadable");

				TextManager.AddBulletAtLevel ("Protocol adopted by SCNMaterial and SCNGeometry", 0);
				TextManager.AddBulletAtLevel ("Shaders parameters are animatable", 0);
				TextManager.AddBulletAtLevel ("Texture samplers are bound to a SCNMaterialProperty", 0);

				TextManager.AddCode ("#var aProperty = SCNMaterialProperty.#Create# (anImage);\n"
				+ "aMaterial.#SetValueForKey# (aProperty, #new NSString# (\"aSampler\"));#");

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			}
			SCNTransaction.Commit ();
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			((SCNView)presentationViewController.View).Playing = false;
			presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
		}
	}
}

