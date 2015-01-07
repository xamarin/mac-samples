using System;
using System.Drawing;
using AppKit;
using OpenGL;
using OpenTK;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideGeometry : Slide
	{
		private SCNNode TeapotNodeForPositionsAndNormals { get; set; }

		private SCNNode TeapotNodeForUVs { get; set; }

		private SCNNode TeapotNodeForMaterials { get; set; }

		private SCNNode PositionsVisualizationNode;
		private SCNNode NormalsVisualizationNode;

		public override int NumberOfSteps ()
		{
			return 6;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtile and add some text
			TextManager.SetTitle ("Node Attributes");
			TextManager.SetSubtitle ("Geometry");

			TextManager.AddBulletAtLevel ("Triangles", 0);
			TextManager.AddBulletAtLevel ("Vertices", 0);
			TextManager.AddBulletAtLevel ("Normals", 0);
			TextManager.AddBulletAtLevel ("UVs", 0);
			TextManager.AddBulletAtLevel ("Materials", 0);

			// We create a container for several versions of the teapot model
			// - one teapot to show positions and normals
			// - one teapot to show texture coordinates
			// - one teapot to show materials
			var allTeapotsNode = SCNNode.Create ();
			allTeapotsNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
			GroundNode.AddChildNode (allTeapotsNode);

			TeapotNodeForPositionsAndNormals = Utils.SCAddChildNode (allTeapotsNode, "TeapotLowRes", "Scenes/teapots/teapotLowRes", 17);
			TeapotNodeForUVs = Utils.SCAddChildNode (allTeapotsNode, "Teapot", "Scenes/teapots/teapot", 17);
			TeapotNodeForMaterials = Utils.SCAddChildNode (allTeapotsNode, "teapotMaterials", "Scenes/teapots/teapotMaterial", 17);

			TeapotNodeForPositionsAndNormals.Position = new SCNVector3 (4, 0, 0);
			TeapotNodeForUVs.Position = new SCNVector3 (4, 0, 0);
			TeapotNodeForMaterials.Position = new SCNVector3 (4, 0, 0);

			foreach (var child in TeapotNodeForMaterials.ChildNodes) {
				foreach (var material in child.Geometry.Materials) {
					material.Normal.Intensity = 0.3f;
					material.Reflective.Contents = NSColor.White;
					material.Reflective.Intensity = 3.0f;
					material.FresnelExponent = 3.0f;
				}
			}

			// Animate the teapots (rotate forever)
			var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
			rotationAnimation.Duration = 40.0f;
			rotationAnimation.RepeatCount = float.MaxValue;
			rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 0, 1, (float)(Math.PI * 2)));

			TeapotNodeForPositionsAndNormals.AddAnimation (rotationAnimation, new NSString ("teapotNodeForPositionsAndNormalsAnimation"));
			TeapotNodeForUVs.AddAnimation (rotationAnimation, new NSString ("teapotNodeForUVsAnimation"));
			TeapotNodeForMaterials.AddAnimation (rotationAnimation, new NSString ("teapotNodeForMaterialsAnimation"));

			// Load the "explode" shader modifier and add it to the geometry
			var explodeShaderPath = NSBundle.MainBundle.PathForResource ("Shaders/explode", "shader");
			var explodeShaderSource = System.IO.File.ReadAllText (explodeShaderPath);
			TeapotNodeForPositionsAndNormals.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointGeometry = explodeShaderSource };

			PositionsVisualizationNode = SCNNode.Create ();
			NormalsVisualizationNode = SCNNode.Create ();

			// Build nodes that will help visualize the vertices (position and normal)
			BuildVisualizationsOfNode (TeapotNodeForPositionsAndNormals, ref PositionsVisualizationNode, ref NormalsVisualizationNode);

			TeapotNodeForPositionsAndNormals.AddChildNode (PositionsVisualizationNode);
			TeapotNodeForPositionsAndNormals.AddChildNode (NormalsVisualizationNode);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Adjust the near clipping plane of the spotlight to maximize the precision of the dynamic drop shadows
				presentationViewController.SpotLight.Light.SetAttribute (new NSNumber (30), SCNLightAttribute.ShadowNearClippingKey);

				// Show what needs to be shown, hide what needs to be hidden
				PositionsVisualizationNode.Opacity = 0.0f;
				NormalsVisualizationNode.Opacity = 0.0f;
				TeapotNodeForUVs.Opacity = 0.0f;
				TeapotNodeForMaterials.Opacity = 0.0f;

				TeapotNodeForPositionsAndNormals.Opacity = 1.0f;

				// Don't highlight bullets (this is useful when we go back from the next slide)
				TextManager.HighlightBullet (-1);
				break;
			case 1:
				TextManager.HighlightBullet (0);

				// Animate the "explodeValue" parameter (uniform) of the shader modifier
				var explodeAnimation = CABasicAnimation.FromKeyPath ("explodeValue");
				explodeAnimation.Duration = 2.0f;
				explodeAnimation.RepeatCount = float.MaxValue;
				explodeAnimation.AutoReverses = true;
				explodeAnimation.To = new NSNumber (20);
				explodeAnimation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				TeapotNodeForPositionsAndNormals.Geometry.AddAnimation (explodeAnimation, new NSString ("explode"));
				break;
			case 2:
				TextManager.HighlightBullet (1);

				// Remove the "explode" animation and freeze the "explodeValue" parameter to the current value
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				var explodeValue = TeapotNodeForPositionsAndNormals.PresentationNode.Geometry.ValueForKey (new NSString ("explodeValue"));
				TeapotNodeForPositionsAndNormals.Geometry.SetValueForKey (explodeValue, new NSString ("explodeValue"));
				TeapotNodeForPositionsAndNormals.Geometry.RemoveAllAnimations ();
				SCNTransaction.Commit ();

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.SetCompletionBlock (() => {
					// Animate to a "no explosion" state and show the positions on completion
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 1.0f;
					PositionsVisualizationNode.Opacity = 1.0f;
					SCNTransaction.Commit ();
				});
				TeapotNodeForPositionsAndNormals.Geometry.SetValueForKey (new NSNumber (0.0f), new NSString ("explodeValue"));
				SCNTransaction.Commit ();
				break;
			case 3:
				TextManager.HighlightBullet (2);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				PositionsVisualizationNode.Opacity = 0.0f;
				NormalsVisualizationNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case 4:
				TextManager.HighlightBullet (3);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				NormalsVisualizationNode.Hidden = true;
				TeapotNodeForUVs.Opacity = 1.0f;
				TeapotNodeForPositionsAndNormals.Opacity = 0.0f;
				SCNTransaction.Commit ();
				break;
			case 5:
				TextManager.HighlightBullet (4);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				TeapotNodeForUVs.Hidden = true;
				TeapotNodeForMaterials.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			}
		}

		private void BuildVisualizationsOfNode (SCNNode node, ref SCNNode verticesNode, ref SCNNode normalsNode)
		{
			// A material that will prevent the nodes from being lit
			var noLightingMaterial = SCNMaterial.Create ();
			noLightingMaterial.LightingModelName = SCNLightingModel.Constant;

			// Create nodes to represent the vertex and normals
			var positionVisualizationNode = SCNNode.Create ();
			var normalsVisualizationNode = SCNNode.Create ();

			// Retrieve the vertices and normals from the model
			var positionSource = node.Geometry.GetGeometrySourcesForSemantic (SCNGeometrySourceSemantic.Vertex) [0];
			var normalSource = node.Geometry.GetGeometrySourcesForSemantic (SCNGeometrySourceSemantic.Normal) [0];

			// Get vertex and normal bytes
			var vertexBufferByte = (byte[])positionSource.Data.ToArray ();
			var vertexBuffer = new float[vertexBufferByte.Length / 4];
			Buffer.BlockCopy (vertexBufferByte, 0, vertexBuffer, 0, vertexBufferByte.Length);

			var normalBufferByte = (byte[])normalSource.Data.ToArray ();
			var normalBuffer = new float[normalBufferByte.Length / 4];
			Buffer.BlockCopy (normalBufferByte, 0, normalBuffer, 0, normalBufferByte.Length);

			// Iterate and create geometries to represent the positions and normals
			for (var i = 0; i < positionSource.VectorCount; i++) {
				// One new node per normal/vertex
				var vertexNode = SCNNode.Create ();
				var normalNode = SCNNode.Create ();

				// Attach one sphere per vertex
				var sphere = SCNSphere.Create (0.5f);
				sphere.SegmentCount = 4; // use a small segment count for better performances
				sphere.FirstMaterial = noLightingMaterial;
				vertexNode.Geometry = sphere;

				// And one pyramid per normal
				var pyramid = SCNPyramid.Create (0.1f, 0.1f, 8);
				pyramid.FirstMaterial = noLightingMaterial;
				normalNode.Geometry = pyramid;

				// Place the position node
				var componentsPerVector = positionSource.ComponentsPerVector;
				vertexNode.Position = new SCNVector3 (vertexBuffer [i * componentsPerVector], vertexBuffer [i * componentsPerVector + 1], vertexBuffer [i * componentsPerVector + 2]);

				// Place the normal node
				normalNode.Position = vertexNode.Position;

				// Orientate the normal
				var up = new Vector3 (0, 0, 1);
				var normalVec = new Vector3 (normalBuffer [i * 3], normalBuffer [i * 3 + 1], normalBuffer [i * 3 + 2]);
				var axis = Vector3.Normalize (Vector3.Cross (up, normalVec));
				var dotProduct = Vector3.Dot (up, normalVec);
				normalNode.Rotation = new SCNVector4 (axis.X, axis.Y, axis.Z, NMath.Acos (dotProduct));

				// Add the nodes to their parent
				positionVisualizationNode.AddChildNode (vertexNode);
				normalsVisualizationNode.AddChildNode (normalNode);
			}

			// We must flush the transaction in order to make sure that the parametric geometries (sphere and pyramid)
			// are up-to-date before flattening the nodes
			SCNTransaction.Flush ();

			// Flatten the visualization nodes so that they can be rendered with 1 draw call
			verticesNode = positionVisualizationNode.FlattenedClone ();
			normalsNode = normalsVisualizationNode.FlattenedClone ();
		}
	}
}