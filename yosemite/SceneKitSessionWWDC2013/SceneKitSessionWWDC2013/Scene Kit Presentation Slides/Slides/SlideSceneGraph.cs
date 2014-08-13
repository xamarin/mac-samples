using System;

using AppKit;
using SceneKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideSceneGraph : Slide
	{
		private static SCNNode DiagramNode = null;

		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Scene Graph");
			TextManager.SetSubtitle ("Scene");
			TextManager.AddBulletAtLevel ("SCNScene", 0);
			TextManager.AddBulletAtLevel ("Starting point", 0);

			// Setup the diagram
			var diagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();
			GroundNode.AddChildNode (diagramNode);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			var diagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();
			SlideSceneGraph.ScenegraphDiagramGoToStep (index);

			switch (index) {
			case 0:
				diagramNode.Opacity = 0.0f;
				diagramNode.Position = new SCNVector3 (0.0f, 5.0f, 0.0f);
				diagramNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
				break;
			case 1:
				SCNTransaction.Begin ();

				SCNTransaction.AnimationDuration = 0;
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				// Change the slide's subtitle and add some text
				TextManager.SetSubtitle ("Node");
				TextManager.AddBulletAtLevel ("SCNNode", 0);
				TextManager.AddBulletAtLevel ("A location in 3D space", 0);
				TextManager.AddBulletAtLevel ("Position / Rotation / Scale", 1);

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				SCNTransaction.Commit ();
				break;
			case 2:
				TextManager.AddBulletAtLevel ("Hierarchy of nodes", 0);
				TextManager.AddBulletAtLevel ("Relative to the parent node", 1);
				break;
			case 3:
				SCNTransaction.Begin ();

				SCNTransaction.AnimationDuration = 0;
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				// Change the slide's subtitle and add some text
				TextManager.SetSubtitle ("Node attributes");
				TextManager.AddBulletAtLevel ("Geometry", 0);
				TextManager.AddBulletAtLevel ("Camera", 0);
				TextManager.AddBulletAtLevel ("Light", 0);
				TextManager.AddBulletAtLevel ("Can be shared", 0);

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				SCNTransaction.Commit ();

				SCNTransaction.Begin ();

				SCNTransaction.AnimationDuration = 1.0f;
				// move the diagram up otherwise it would intersect the floor
				diagramNode.Position = new SCNVector3 (0.0f, diagramNode.Position.Y + 1.0f, 0.0f);

				SCNTransaction.Commit ();
				break;
			}
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			var diagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();
			SCNTransaction.Begin ();

			SCNTransaction.AnimationDuration = 1.0f;
			diagramNode.Opacity = 1.0f;
			diagramNode.Rotation = new SCNVector4 (1, 0, 0, 0);
			SlideSceneGraph.ShowNodesNamed (new string[] { "scene" });

			SCNTransaction.Commit ();
		}

		public static SCNNode SharedScenegraphDiagramNode ()
		{
			if (DiagramNode == null) {
				DiagramNode = SCNNode.Create ();
				DiagramNode.Opacity = 0.0f;


				// "Scene"
				var blue = NSColor.FromDeviceRgba (49.0f / 255.0f, 80.0f / 255.0f, 201.0f / 255.0f, 1);
				var box = Utils.SCBoxNode ("Scene", new CGRect (-53.5f, -25, 107, 50), blue, 10, true);
				box.Name = "scene";
				box.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				box.Position = new SCNVector3 (5.4f, 4.8f, 0);
				DiagramNode.AddChildNode (box);


				// Arrow from "Scene" to "Root Node"
				var arrowNode = new SCNNode {
					Name = "sceneArrow",
					Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new CGSize (3, 0.2f), new CGSize (0.5f, 0.7f), 0.2f, false), 0),
					Scale = new SCNVector3 (20, 20, 1),
					Position = new SCNVector3 (-5, 0, 8),
					Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 2))
				};
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = blue;
				box.AddChildNode (arrowNode);


				// "Root Node"
				var green = NSColor.FromDeviceRgba (154.0f / 255.0f, 197.0f / 255.0f, 58.0f / 255.0f, 1);
				box = Utils.SCBoxNode ("Root Node", new CGRect (-40, -36, 80, 72), green, 10, true);
				box.Name = "rootNode";
				box.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				box.Position = new SCNVector3 (5.405f, 1.8f, 0);
				DiagramNode.AddChildNode (box);


				// Arrows from "Root Node" to child nodes
				arrowNode = arrowNode.Clone ();
				arrowNode.Name = "nodeArrow1";
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new CGSize (5.8f, 0.15f), new CGSize (0.5f, 0.7f), 0.2f, true), 0);
				arrowNode.Position = new SCNVector3 (0, -30, 8);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * 0.85f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = green;
				box.AddChildNode (arrowNode);

				arrowNode = arrowNode.Clone ();
				arrowNode.Name = "nodeArrow2";
				arrowNode.Position = new SCNVector3 (0, -43, 8);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * (1 - 0.85f)));
				box.AddChildNode (arrowNode);

				arrowNode = arrowNode.Clone ();
				arrowNode.Name = "nodeArrow3";
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new CGSize (2.6f, 0.15f), new CGSize (0.5f, 0.7f), 0.2f, true), 0);
				arrowNode.Position = new SCNVector3 (-4, -38, 8);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * 0.5f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = green;
				box.AddChildNode (arrowNode);


				// Multiple "Child Node"
				box = Utils.SCBoxNode ("Child Node", new CGRect (-40, -36, 80, 72), green, 10, true);
				box.Name = "child1";
				box.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				box.Position = new SCNVector3 (2.405f, -2, 0);
				DiagramNode.AddChildNode (box);

				box = box.Clone ();
				box.Name = "child2";
				box.Position = new SCNVector3 (5.405f, -2, 0);
				DiagramNode.AddChildNode (box);

				box = box.Clone ();
				box.Name = "child3";
				box.Position = new SCNVector3 (8.405f, -2, 0);
				DiagramNode.AddChildNode (box);


				// "Light"
				var purple = NSColor.FromDeviceRgba (190.0f / 255.0f, 56.0f / 255.0f, 243.0f / 255.0f, 1);
				box = Utils.SCBoxNode ("Light", new CGRect (-40, -20, 80, 40), purple, 10, true);
				box.Name = "light";
				box.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				box.Position = new SCNVector3 (2.405f, -4.8f, 0);
				DiagramNode.AddChildNode (box);
				

				// Arrow to "Light"
				arrowNode = new SCNNode {
					Name = "lightArrow",
					Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new CGSize (2.0f, 0.15f), new CGSize (0.5f, 0.7f), 0.2f, false), 0),
					Position = new SCNVector3 (-5, 60, 8),
					Scale = new SCNVector3 (20, 20, 1),
					Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 2))
				};
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = purple;
				box.AddChildNode (arrowNode);


				// "Camera"
				box = Utils.SCBoxNode ("Camera", new CGRect (-45, -20, 90, 40), purple, 10, true);
				box.Name = "camera";
				box.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				box.Position = new SCNVector3 (5.25f, -4.8f, 0);
				DiagramNode.AddChildNode (box);


				// Arrow to "Camera"
				arrowNode = arrowNode.Clone ();
				arrowNode.Name = "cameraArrow";
				arrowNode.Position = new SCNVector3 (0, 60, 8);
				box.AddChildNode (arrowNode);


				// "Geometry"
				box = Utils.SCBoxNode ("Geometry", new CGRect (-55, -20, 110, 40), purple, 10, true);
				box.Name = "geometry";
				box.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				box.Position = new SCNVector3 (8.6f, -4.8f, 0);
				DiagramNode.AddChildNode (box);


				// Arrows to "Geometry"
				arrowNode = arrowNode.Clone ();
				arrowNode.Name = "geometryArrow";
				arrowNode.Position = new SCNVector3 (-10, 60, 8);
				box.AddChildNode (arrowNode);

				arrowNode = arrowNode.Clone ();
				arrowNode.Name = "geometryArrow2";
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new CGSize (5.0f, 0.15f), new CGSize (0.5f, 0.7f), 0.2f, false), 0);
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = purple;
				arrowNode.Position = new SCNVector3 (-105, 53, 8);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 8));
				box.AddChildNode (arrowNode);


				// Multiple "Material"
				var redColor = NSColor.FromDeviceRgba (168.0f / 255.0f, 21.0f / 255.0f, 0.0f / 255.0f, 1);

				var materialsBox = Utils.SCBoxNode (null, new CGRect (-151, -25, 302, 50), NSColor.LightGray, 2, true);
				materialsBox.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				materialsBox.Name = "materials";
				materialsBox.Position = new SCNVector3 (8.7f, -7.1f, -0.2f);
				DiagramNode.AddChildNode (materialsBox);

				box = Utils.SCBoxNode ("Material", new CGRect (-45, -20, 90, 40), redColor, 0, true);
				box.Position = new SCNVector3 (-100, 0, 0.2f);
				materialsBox.AddChildNode (box);

				box = box.Clone ();
				box.Position = new SCNVector3 (100, 0, 0.2f);
				materialsBox.AddChildNode (box);

				box = box.Clone ();
				box.Position = new SCNVector3 (0, 0, 0.2f);
				materialsBox.AddChildNode (box);

				// Arrow from "Geometry" to the materials
				arrowNode = new SCNNode {
					Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new CGSize (2.0f, 0.15f), new CGSize (0.5f, 0.7f), 0.2f, false), 0),
					Position = new SCNVector3 (-6, 65, 8),
					Scale = new SCNVector3 (20, 20, 1),
					Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 2))
				};
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = redColor;
				box.AddChildNode (arrowNode);

				materialsBox.ParentNode.ReplaceChildNode (materialsBox, materialsBox.FlattenedClone ());
			}
			return DiagramNode;
		}

		private static void HighlightNodes (string[] names, SCNNode node)
		{
			foreach (var child in node.ChildNodes) {
				var result = Array.Find (names, delegate(string s) {
					return s == child.Name;
				});
				if (result != null && result.Length > 0) {
					child.Opacity = 1;
					HighlightNodes (names, child);
				} else {
					if (child.Opacity == 1.0f)
						child.Opacity = 0.3f;
				}
			}
		}

		private static void ShowNodesNamed (string[] names)
		{
			var diagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1.0f;
			foreach (var nodeName in names) {
				var node = diagramNode.FindChildNode (nodeName, true);
				node.Opacity = 1.0f;
				if (node.Rotation.Z == 0.0f)
					node.Rotation = new SCNVector4 (0, 1, 0, 0);
			}
			SCNTransaction.Commit ();
		}

		public static void ScenegraphDiagramGoToStep (int step)
		{
			var diagramNode = SlideSceneGraph.SharedScenegraphDiagramNode ();

			switch (step) {
			case 0:
				// Restore the initial state (hidden and rotated)
				foreach (var child in diagramNode) {
					child.Opacity = 0.0f;
					if (child.Rotation.Z == 0) // don't touch nodes that already have a rotation set
						child.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 2));
				}
				break;
			case 1:
				ShowNodesNamed (new string[] { "sceneArrow", "rootNode" });
				break;
			case 2:
				ShowNodesNamed (new string[] { "child1", "child2", "child3", "nodeArrow1", "nodeArrow2", "nodeArrow3" });
				break;
			case 3:
				ShowNodesNamed (new string[] {
					"light",
					"camera",
					"geometry",
					"lightArrow",
					"cameraArrow",
					"geometryArrow",
					"geometryArrow2"
				});
				break;
			case 4:
				ShowNodesNamed (new string[] {
					"scene",
					"sceneArrow",
					"rootNode",
					"light",
					"camera",
					"cameraArrow",
					"child1",
					"child2",
					"child3",
					"nodeArrow1",
					"nodeArrow2",
					"nodeArrow3",
					"geometry",
					"lightArrow",
					"geometryArrow",
					"geometryArrow2"
				});
				HighlightNodes (new string[] {
					"scene",
					"sceneArrow",
					"rootNode",
					"light",
					"camera",
					"cameraArrow",
					"child1",
					"child2",
					"child3",
					"nodeArrow1",
					"nodeArrow2",
					"nodeArrow3",
					"geometry",
					"lightArrow",
					"geometryArrow",
					"geometryArrow2"
				}, diagramNode);
				break;
			case 5:
				ShowNodesNamed (new string[] {
					"scene",
					"sceneArrow",
					"rootNode",
					"light",
					"camera",
					"cameraArrow",
					"child1",
					"child2",
					"child3",
					"nodeArrow1",
					"nodeArrow2",
					"nodeArrow3",
					"geometry",
					"lightArrow",
					"geometryArrow",
					"geometryArrow2",
					"materials"
				});
				HighlightNodes (new string[] {
					"scene",
					"sceneArrow",
					"rootNode",
					"child2",
					"child3",
					"nodeArrow2",
					"nodeArrow3",
					"geometry",
					"geometryArrow",
					"geometryArrow2",
					"materials"
				}, diagramNode);
				break;
			case 6:
				HighlightNodes (new string[] { "child3", "geometryArrow", "geometry" }, diagramNode);
				break;
			case 7:
				ShowNodesNamed (new string[] {
					"scene",
					"sceneArrow",
					"rootNode",
					"light",
					"camera",
					"cameraArrow",
					"child1",
					"child2",
					"child3",
					"nodeArrow1",
					"nodeArrow2",
					"nodeArrow3",
					"geometry",
					"lightArrow",
					"geometryArrow",
					"geometryArrow2",
					"materials"
				});
				break;
			}
		}
	}
}