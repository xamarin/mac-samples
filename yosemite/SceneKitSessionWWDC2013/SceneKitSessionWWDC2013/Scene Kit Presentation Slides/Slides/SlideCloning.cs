using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.SceneKit;

namespace SceneKitSessionWWDC2013
{
	public class SlideCloning : Slide
	{
		private NSColor RedColor, GreenColor, BlueColor, PurpleColor;
		private SCNNode DiagramNode;

		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			RedColor = NSColor.FromDeviceRgba (168.0f / 255.0f, 21.0f / 255.0f, 0.0f / 255.0f, 1);
			GreenColor = NSColor.FromDeviceRgba (154.0f / 255.0f, 197.0f / 255.0f, 58.0f / 255.0f, 1);
			BlueColor = NSColor.FromDeviceRgba (49.0f / 255.0f, 80.0f / 255.0f, 201.0f / 255.0f, 1);
			PurpleColor = NSColor.FromDeviceRgba (190.0f / 255.0f, 56.0f / 255.0f, 243.0f / 255.0f, 1);

			// Create the diagram but hide it
			DiagramNode = CloningDiagramNode ();
			DiagramNode.Opacity = 0.0f;
			ContentNode.AddChildNode (DiagramNode);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Performance");
				TextManager.SetSubtitle ("Copying");

				TextManager.AddBulletAtLevel ("Attributes are shared by default", 0);
				TextManager.AddBulletAtLevel ("Unshare if needed", 0);
				TextManager.AddBulletAtLevel ("Copying geometries is cheap", 0);

				break;
			case 1:
				// New "Node B" box
				var nodeB = Utils.SCBoxNode ("Node B", new RectangleF (-55, -36, 110, 50), GreenColor, 10, true);
				nodeB.Name = "nodeB";
				nodeB.Position = new SCNVector3 (140, 0, 0);
				nodeB.Opacity = 0;

				var nodeA = ContentNode.FindChildNode ("nodeA", true);
				nodeA.AddChildNode (nodeB);

				// Arrow from "Root Node" to "Node B"
				var arrowNode = SCNNode.Create ();
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (140, 3), new SizeF (10, 14), 4, true), 0);
				arrowNode.Position = new SCNVector3 (-130, 60, 0);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * 0.12f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = GreenColor;
				nodeB.AddChildNode (arrowNode);

				// Arrow from "Node B" to the shared geometry
				arrowNode = SCNNode.Create ();
				arrowNode.Name = "arrow-shared-geometry";
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (140, 3), new SizeF (10, 14), 4, false), 0);
				arrowNode.Position = new SCNVector3 (0, -28, 0);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI * 1.12f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = PurpleColor;
				nodeB.AddChildNode (arrowNode);

				// Reveal
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				nodeB.Opacity = 1.0f;

				// Show the related code
				TextManager.AddCode ("#// Copy a node \n"
				+ "var nodeB = nodeA.#Copy# ();#");
				SCNTransaction.Commit ();
				break;
			case 2:
				var geometryNodeA = ContentNode.FindChildNode ("geometry", true);
				var oldArrowNode = ContentNode.FindChildNode ("arrow-shared-geometry", true);

				// New "Geometry" box
				var geometryNodeB = Utils.SCBoxNode ("Geometry", new RectangleF (-55, -20, 110, 40), PurpleColor, 10, true);
				geometryNodeB.Position = new SCNVector3 (140, 0, 0);
				geometryNodeB.Opacity = 0;
				geometryNodeA.AddChildNode (geometryNodeB);

				// Arrow from "Node B" to the new geometry
				arrowNode = SCNNode.Create ();
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (55, 3), new SizeF (10, 14), 4, false), 0);
				arrowNode.Position = new SCNVector3 (0, 75, 0);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * 0.5f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = PurpleColor;
				geometryNodeB.AddChildNode (arrowNode);

				// Arrow from the new geometry to "Material"
				arrowNode = SCNNode.Create ();
				arrowNode.Name = "arrow-shared-material";
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (140, 3), new SizeF (10, 14), 4, true), 0);
				arrowNode.Position = new SCNVector3 (-130, -80, 0);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI * 0.12f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = RedColor;
				geometryNodeB.AddChildNode (arrowNode);

				// Reveal
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				geometryNodeB.Opacity = 1.0f;
				oldArrowNode.Opacity = 0.0f;

				// Show the related code
				TextManager.AddEmptyLine ();
				TextManager.AddCode ("#// Unshare geometry \n"
				+ "nodeB.Geometry = nodeB.Geometry.#Copy# ();#");
				SCNTransaction.Commit ();
				break;
			case 3:
				var materialANode = ContentNode.FindChildNode ("material", true);
				oldArrowNode = ContentNode.FindChildNode ("arrow-shared-material", true);

				// New "Material" box
				var materialBNode = Utils.SCBoxNode ("Material", new RectangleF (-55, -20, 110, 40), NSColor.Orange, 10, true);
				materialBNode.Position = new SCNVector3 (140, 0, 0);
				materialBNode.Opacity = 0;
				materialANode.AddChildNode (materialBNode);

				// Arrow from the unshared geometry to the new material
				arrowNode = SCNNode.Create ();
				arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (55, 3), new SizeF (10, 14), 4, false), 0);
				arrowNode.Position = new SCNVector3 (0, 75, 0);
				arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * 0.5f));
				arrowNode.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Orange;
				materialBNode.AddChildNode (arrowNode);

				// Reveal
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				materialBNode.Opacity = 1.0f;
				oldArrowNode.Opacity = 0.0f;
				SCNTransaction.Commit ();
				break;
			}
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			// Once the slide ordered in, reveal the diagram
			foreach (var node in DiagramNode.ChildNodes)
				node.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 2)); // initially viewed from the side

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			DiagramNode.Opacity = 1.0f;

			foreach (var node in DiagramNode.ChildNodes)
				node.Rotation = new SCNVector4 (0, 1, 0, 0);

			SCNTransaction.Commit ();
		}

		// A node hierarchy that illustrates the cloning mechanism and how to unshare attributes
		private SCNNode CloningDiagramNode ()
		{
			var diagramNode = SCNNode.Create ();
			diagramNode.Position = new SCNVector3 (7, 9, 3);

			// "Scene" box
			var sceneNode = Utils.SCBoxNode ("Scene", new RectangleF (-53.5f, -25, 107, 50), BlueColor, 10, true);
			sceneNode.Name = "scene";
			sceneNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
			sceneNode.Position = new SCNVector3 (0, 4.8f, 0);
			diagramNode.AddChildNode (sceneNode);

			// "Root node" box
			var rootNode = Utils.SCBoxNode ("Root Node", new RectangleF (-40, -36, 80, 72), GreenColor, 10, true);
			rootNode.Name = "rootNode";
			rootNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
			rootNode.Position = new SCNVector3 (0.05f, 1.8f, 0);
			diagramNode.AddChildNode (rootNode);

			// "Node A" box
			var nodeA = Utils.SCBoxNode ("Node A", new RectangleF (-55, -36, 110, 50), GreenColor, 10, true);
			nodeA.Name = "nodeA";
			nodeA.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
			nodeA.Position = new SCNVector3 (0, -1.4f, 0);
			diagramNode.AddChildNode (nodeA);

			// "Geometry" box
			var geometryNode = Utils.SCBoxNode ("Geometry", new RectangleF (-55, -20, 110, 40), PurpleColor, 10, true);
			geometryNode.Name = "geometry";
			geometryNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
			geometryNode.Position = new SCNVector3 (0, -4.7f, 0);
			diagramNode.AddChildNode (geometryNode);

			// "Material" box
			var materialNode = Utils.SCBoxNode ("Material", new RectangleF (-55, -20, 110, 40), RedColor, 10, true);
			materialNode.Name = "material";
			materialNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
			materialNode.Position = new SCNVector3 (0, -7.5f, 0);
			diagramNode.AddChildNode (materialNode);

			// Arrow from "Scene" to "Root Node"
			var arrowNode = SCNNode.Create ();
			arrowNode.Name = "sceneArrow";
			arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (3, 0.2f), new SizeF (0.5f, 0.7f), 0.2f, false), 0);
			arrowNode.Scale = new SCNVector3 (20, 20, 1);
			arrowNode.Position = new SCNVector3 (-5, 0, 8);
			arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 2));
			arrowNode.Geometry.FirstMaterial.Diffuse.Contents = BlueColor;
			sceneNode.AddChildNode (arrowNode);

			// Arrow from "Root Node" to "Node A"
			arrowNode = arrowNode.Clone ();
			arrowNode.Name = "arrow";
			arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (2.6f, 0.15f), new SizeF (0.5f, 0.7f), 0.2f, true), 0);
			arrowNode.Position = new SCNVector3 (-6, -38, 8);
			arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI * 0.5f));
			arrowNode.Geometry.FirstMaterial.Diffuse.Contents = GreenColor;
			rootNode.AddChildNode (arrowNode);

			// Arrow from "Node A" to "Geometry"
			arrowNode = arrowNode.Clone ();
			arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (2.6f, 0.15f), new SizeF (0.5f, 0.7f), 0.2f, false), 0);
			arrowNode.Position = new SCNVector3 (-5, 74, 8);
			arrowNode.Scale = new SCNVector3 (20, 20, 1);
			arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 2));
			arrowNode.Geometry.FirstMaterial.Diffuse.Contents = PurpleColor;
			geometryNode.AddChildNode (arrowNode);

			// Arrow from "Geometry" to "Material"
			arrowNode = arrowNode.Clone ();
			arrowNode.Geometry = SCNShape.Create (Utils.SCArrowBezierPath (new SizeF (2.7f, 0.15f), new SizeF (0.5f, 0.7f), 0.2f, false), 0);
			arrowNode.Position = new SCNVector3 (-6, 74, 8);
			arrowNode.Scale = new SCNVector3 (20, 20, 1);
			arrowNode.Rotation = new SCNVector4 (0, 0, 1, -(float)(Math.PI / 2));
			arrowNode.Geometry.FirstMaterial.Diffuse.Contents = RedColor;
			materialNode.AddChildNode (arrowNode);

			return diagramNode;
		}
	}
}

