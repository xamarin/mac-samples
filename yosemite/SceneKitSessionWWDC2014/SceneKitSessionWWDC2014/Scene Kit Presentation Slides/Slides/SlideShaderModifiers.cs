using System;
using System.IO;
using AppKit;
using OpenGL;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideShaderModifiers : Slide
	{
		private SCNNode PlaneNode { get; set; }

		private SCNNode SphereNode { get; set; }

		private SCNNode TorusNode { get; set; }

		private SCNNode XRayNode { get; set; }

		private SCNNode VirusNode { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Shader Modifiers");

			TextManager.AddBulletAtLevel ("Inject custom GLSL code", 0);
			TextManager.AddBulletAtLevel ("Combines with Scene Kit’s shaders", 0);
			TextManager.AddBulletAtLevel ("Inject at specific stages", 0);

			TextManager.AddEmptyLine ();
			TextManager.AddCode ("#aMaterial.#shaderModifiers# = @{ <Entry Point> : <GLSL Code> };#");
		}

		public override int NumberOfSteps ()
		{
			return 2;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();

			switch (index) {
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.SetSubtitle ("Entry points");

				var textNode = Utils.SCLabelNode ("Geometry", Utils.LabelSize.Normal, false);
				textNode.Position = new SCNVector3 (-13.5f, 9, 0);
				ContentNode.AddChildNode (textNode);
				textNode = Utils.SCLabelNode ("Surface", Utils.LabelSize.Normal, false);
				textNode.Position = new SCNVector3 (-5.3f, 9, 0);
				ContentNode.AddChildNode (textNode);
				textNode = Utils.SCLabelNode ("Lighting", Utils.LabelSize.Normal, false);
				textNode.Position = new SCNVector3 (2, 9, 0);
				ContentNode.AddChildNode (textNode);
				textNode = Utils.SCLabelNode ("Fragment", Utils.LabelSize.Normal, false);
				textNode.Position = new SCNVector3 (9.5f, 9, 0);
				ContentNode.AddChildNode (textNode);

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				//add spheres
				var sphere = SCNSphere.Create (3);
				sphere.FirstMaterial.Diffuse.Contents = NSColor.Red;
				sphere.FirstMaterial.Specular.Contents = NSColor.White;
				sphere.FirstMaterial.Specular.Intensity = 1.0f;

				sphere.FirstMaterial.Shininess = 0.1f;
				sphere.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap", "jpg"));
				sphere.FirstMaterial.FresnelExponent = 2;

				//GEOMETRY
				var node = SCNNode.Create ();
				node.Geometry = (SCNGeometry)sphere.Copy ();
				node.Position = new SCNVector3 (-12, 3, 0);
				node.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointGeometry = "// Waves Modifier\n"
					+ "uniform float Amplitude = 0.2;\n"
					+ "uniform float Frequency = 5.0;\n"
					+ "vec2 nrm = _geometry.position.xz;\n"
					+ "float len = length(nrm)+0.0001; // for robustness\n"
					+ "nrm /= len;\n"
					+ "float a = len + Amplitude*sin(Frequency * _geometry.position.y + u_time * 10.0);\n"
					+ "_geometry.position.xz = nrm * a;\n"
				};

				GroundNode.AddChildNode (node);

				// SURFACE
				node = SCNNode.Create ();
				node.Geometry = (SCNGeometry)sphere.Copy ();
				node.Position = new SCNVector3 (-4, 3, 0);

				var surfaceModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("Shaders/sm_surf", "shader"));

				node.Rotation = new SCNVector4 (1, 0, 0, -(float)Math.PI / 4);
				node.Geometry.FirstMaterial = (SCNMaterial)node.Geometry.FirstMaterial.Copy ();
				node.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Lambert;
				node.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointSurface = surfaceModifier };
				GroundNode.AddChildNode (node);

				// LIGHTING
				node = SCNNode.Create ();
				node.Geometry = (SCNGeometry)sphere.Copy ();
				node.Position = new SCNVector3 (4, 3, 0);

				var lightingModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("Shaders/sm_light", "shader"));
				node.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointLightingModel = lightingModifier };

				GroundNode.AddChildNode (node);

				// FRAGMENT
				node = SCNNode.Create ();
				node.Geometry = (SCNGeometry)sphere.Copy ();
				node.Position = new SCNVector3 (12, 3, 0);

				node.Geometry.FirstMaterial = (SCNMaterial)node.Geometry.FirstMaterial.Copy ();
				node.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Green;

				var fragmentModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("Shaders/sm_frag", "shader"));
				node.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointFragment = fragmentModifier };

				GroundNode.AddChildNode (node);


				//redraw forever
				((SCNView)presentationViewController.View).Playing = true;
				((SCNView)presentationViewController.View).Loops = true;

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

