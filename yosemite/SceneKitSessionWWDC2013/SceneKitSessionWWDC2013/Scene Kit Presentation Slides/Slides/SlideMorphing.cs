using System;
using System.IO;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013 {
	public class SlideMorphing : Slide {
		SCNNode MapNode { get; set; }

		SCNNode GaugeANode, GaugeAProgressNode;
		SCNNode GaugeBNode, GaugeBProgressNode;

		public override int NumberOfSteps ()
		{
			return 8;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Load the scene
			var intermediateNode = SCNNode.Create ();
			intermediateNode.Position = new SCNVector3 (6, 9, 0);
			intermediateNode.Scale = new SCNVector3 (1.4f, 1, 1);
			GroundNode.AddChildNode (intermediateNode);

			MapNode = Utils.SCAddChildNode (intermediateNode, "Map", "Scenes/map/foldingMap", 25);
			MapNode.Position = new SCNVector3 (0, 0, 0);
			MapNode.Opacity = 0.0f;

			// Use a bunch of shader modifiers to simulate ambient occlusion when the map is folded
			var geomFile = NSBundle.MainBundle.PathForResource ("Shaders/mapGeometry", "shader");
			var fragFile = NSBundle.MainBundle.PathForResource ("Shaders/mapFragment", "shader");
			var lightFile = NSBundle.MainBundle.PathForResource ("Shaders/mapLighting", "shader");
			var geometryModifier = File.ReadAllText (geomFile);
			var fragmentModifier = File.ReadAllText (fragFile);
			var lightingModifier = File.ReadAllText (lightFile);

			MapNode.Geometry.ShaderModifiers = new SCNShaderModifiers { 
				EntryPointGeometry = geometryModifier,
				EntryPointFragment = fragmentModifier,
				EntryPointLightingModel = lightingModifier
			};
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			//animate by default
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;

			switch (index) {
			case 0:
				SCNTransaction.AnimationDuration = 0;

				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Morphing");
				TextManager.AddBulletAtLevel ("Linear morph between multiple targets", 0);

				// Initial state, no ambient occlusion
				// This also shows how uniforms from shader modifiers can be set using KVC
				MapNode.Geometry.SetValueForKey (new NSNumber (0), new NSString ("ambientOcclusionYFactor"));
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				// Reveal the map and show the gauges
				MapNode.Opacity = 1.0f;

				GaugeAProgressNode = SCNNode.Create ();
				GaugeBProgressNode = SCNNode.Create ();

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				GaugeANode = Utils.SCGaugeNode ("Target A", ref GaugeAProgressNode);
				GaugeANode.Position = new SCNVector3 (-10.5f, 15, -5);
				ContentNode.AddChildNode (GaugeANode);

				GaugeBNode = Utils.SCGaugeNode ("Target B", ref GaugeBProgressNode);
				GaugeBNode.Position = new SCNVector3 (-10.5f, 13, -5);
				ContentNode.AddChildNode (GaugeBNode);
				SCNTransaction.Commit ();
				break;
			case 2:
				// Morph and update the gauges
				GaugeAProgressNode.Scale = new SCNVector3 (1, 1, 1);
				MapNode.Morpher.SetWeight (0.65f, 0);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				GaugeAProgressNode.Opacity = 1.0f;
				SCNTransaction.Commit ();

				var shadowPlane = MapNode.ChildNodes [0];
				shadowPlane.Scale = new SCNVector3 (0.35f, 1, 1);

				MapNode.ParentNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 4) * 0.75f);
				break;
			case 3:
				// Morph and update the gauges
				GaugeAProgressNode.Scale = new SCNVector3 (1, 0.01f, 1);
				MapNode.Morpher.SetWeight (0, 0);

				shadowPlane = MapNode.ChildNodes [0];
				shadowPlane.Scale = new SCNVector3 (1, 1, 1);

				MapNode.ParentNode.Rotation = new SCNVector4 (1, 0, 0, 0);

				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 0.5f;
					GaugeAProgressNode.Opacity = 0.0f;
					SCNTransaction.Commit ();
				});
				break;
			case 4:
				// Morph and update the gauges
				GaugeBProgressNode.Scale = new SCNVector3 (1, 1, 1);
				MapNode.Morpher.SetWeight (0.4f, 1);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.1f;
				GaugeBProgressNode.Opacity = 1.0f;
				SCNTransaction.Commit ();

				shadowPlane = MapNode.ChildNodes [0];
				shadowPlane.Scale = new SCNVector3 (1, 0.6f, 1);

				MapNode.ParentNode.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 4) * 0.5f);
				break;
			case 5:
				// Morph and update the gauges
				GaugeBProgressNode.Scale = new SCNVector3 (1, 0.01f, 1);
				MapNode.Morpher.SetWeight (0, 1);

				shadowPlane = MapNode.ChildNodes [0];
				shadowPlane.Scale = new SCNVector3 (1, 1, 1);

				MapNode.ParentNode.Rotation = new SCNVector4 (0, 1, 0, 0);

				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 0.5f;
					GaugeBProgressNode.Opacity = 0.0f;
					SCNTransaction.Commit ();
				});
				break;
			case 6:
				// Morph and update the gauges
				GaugeAProgressNode.Scale = new SCNVector3 (1, 1, 1);
				GaugeBProgressNode.Scale = new SCNVector3 (1, 1, 1);

				MapNode.Morpher.SetWeight (0.65f, 0);
				MapNode.Morpher.SetWeight (0.30f, 1);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.1f;
				GaugeAProgressNode.Opacity = 1.0f;
				GaugeBProgressNode.Opacity = 1.0f;
				SCNTransaction.Commit ();

				shadowPlane = MapNode.ChildNodes [0];
				shadowPlane.Scale = new SCNVector3 (0.4f, 0.7f, 1);
				shadowPlane.Opacity = 0.2f;

				MapNode.Geometry.SetValueForKey (new NSNumber (0.35f), new NSString ("ambientOcclusionYFactor"));
				MapNode.Position = new SCNVector3 (0, 0, 5);
				MapNode.ParentNode.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 4) * 0.5f);
				MapNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 4) * 0.75f);
				break;
			case 7:
				SCNTransaction.AnimationDuration = 0.5f;

				// Hide everything and update the text
				MapNode.Opacity = 0;
				GaugeANode.Opacity = 0.0f;
				GaugeBNode.Opacity = 0.0f;

				TextManager.SetSubtitle ("SCNMorpher");
				TextManager.AddBulletAtLevel ("Topology must match", 0);
				TextManager.AddBulletAtLevel ("Can be loaded from DAEs", 0);
				TextManager.AddBulletAtLevel ("Can be created programmatically", 0);

				break;
			}
			SCNTransaction.Commit ();
		}
	}
}