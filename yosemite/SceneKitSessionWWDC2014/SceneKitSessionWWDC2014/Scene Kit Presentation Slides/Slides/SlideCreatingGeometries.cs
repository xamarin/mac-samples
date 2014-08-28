using System;
using System.Runtime.InteropServices;
using AppKit;
using SceneKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideCreatingGeometries : Slide
	{
		private SCNNode CarouselNode { get; set; }

		private SCNNode TextNode { get; set; }

		private SCNNode StarOutline { get; set; }

		private SCNNode StarNode { get; set; }

		private int PrimitiveIndex { get; set; }

		public override int NumberOfSteps ()
		{
			return 6;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title
			TextManager.SetTitle ("Creating Geometries");
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				TextManager.SetSubtitle ("Built-in parametric primitives");
				break;
			case 1:
				PresentPrimitives ();
				break;
			case 2:
				// Hide the carousel and illustrate SCNText
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.SetCompletionBlock (() => {
					if (CarouselNode != null)
						CarouselNode.RemoveFromParentNode ();
				});

				PresentTextNode ();

				TextNode.Opacity = 1.0f;

				if (CarouselNode != null) {
					CarouselNode.Position = new SCNVector3 (0, CarouselNode.Position.Y, -50);
					CarouselNode.Opacity = 0.0f;
				}

				SCNTransaction.Commit ();

				TextManager.SetSubtitle ("Built-in 3D text");
				TextManager.AddBulletAtLevel ("SCNText", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			case 3:
				//Show bezier path
				var star = StarPath (3, 6);

				var shape = SCNShape.Create (star, 1);
				shape.ChamferRadius = 0.2f;
				shape.ChamferProfile = OutlineChamferProfilePath ();
				shape.ChamferMode = SCNChamferMode.Front;

				// that way only the outline of the model will be visible
				var outlineMaterial = SCNMaterial.Create ();
				outlineMaterial.Ambient.Contents = outlineMaterial.Diffuse.Contents = outlineMaterial.Specular.Contents = NSColor.Black;
				outlineMaterial.Emission.Contents = NSColor.White;
				outlineMaterial.DoubleSided = true;

				var tranparentMaterial = SCNMaterial.Create ();
				tranparentMaterial.Transparency = 0.0f;

				shape.Materials = new SCNMaterial[] {
					tranparentMaterial,
					tranparentMaterial,
					tranparentMaterial,
					outlineMaterial,
					outlineMaterial
				};

				StarOutline = SCNNode.Create ();
				StarOutline.Geometry = shape;
				StarOutline.Position = new SCNVector3 (0, 5, 30);
				StarOutline.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, NMath.PI * 2, 0, 10.0)));

				GroundNode.AddChildNode (StarOutline);

				// Hide the 3D text and introduce SCNShape
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				SCNTransaction.SetCompletionBlock (() => {
					TextNode.RemoveFromParentNode ();
				});

				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.SetSubtitle ("3D Shapes");

				TextManager.AddBulletAtLevel ("SCNShape", 0);

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);

				StarOutline.Position = new SCNVector3 (0, 5, 0);
				TextNode.Position = new SCNVector3 (TextNode.Position.X, TextNode.Position.Y, -30);


				SCNTransaction.Commit ();
				break;
			case 4:
				star = StarPath (3, 6);

				shape = SCNShape.Create (star, 0);
				shape.ChamferRadius = 0.1f;

				StarNode = SCNNode.Create ();
				StarNode.Geometry = shape;
				var material = SCNMaterial.Create ();
				material.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/color_envmap", "png"));
				material.Diffuse.Contents = NSColor.Black;
				StarNode.Geometry.Materials = new SCNMaterial[] { material };
				StarNode.Position = new SCNVector3 (0, 5, 0);
				StarNode.Pivot = SCNMatrix4.CreateTranslation (0, 0, -0.5f);
				StarOutline.ParentNode.AddChildNode (StarNode);

				StarNode.EulerAngles = StarOutline.EulerAngles;
				StarNode.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, NMath.PI * 2, 0, 10.0)));

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				SCNTransaction.SetCompletionBlock (() => {
					StarOutline.RemoveFromParentNode ();
				});

				shape.ExtrusionDepth = 1;
				StarOutline.Opacity = 0.0f;

				SCNTransaction.Commit ();
				break;
			case 5:
				//OpenSubdiv
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.SetSubtitle ("Subdivisions");

				TextManager.AddBulletAtLevel ("OpenSubdiv", 0);
				TextManager.AddCode ("#aGeometry.#SubdivisionLevel# = anInteger;#");

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);

				//add boxes
				var boxesNode = SCNNode.Create ();

				var level0 = Utils.SCAddChildNode (boxesNode, "rccarBody_LP", "Scenes.scnassets/car/car_lowpoly", 10);
				level0.Position = new SCNVector3 (-6, level0.Position.Y, 0);

				var label = Utils.SCBoxNode ("0", new CGRect (0, 0, 40, 40), NSColor.Orange, 20.0f, true);
				label.Position = new SCNVector3 (0, -35, 10);
				label.Scale = new SCNVector3 (0.3f, 0.3f, 0.001f);
				level0.AddChildNode (label);

				boxesNode.Position = new SCNVector3 (0, 0, 30);

				var level1 = level0.Clone ();
				/*foreach (var child in level1.ChildNodes) {
					if (child.Name != "engine_LP") {
						child.Geometry = (SCNGeometry)child.Geometry.Copy ();
						child.Geometry.SubdivisionLevel = 3;
					}
				}*/

				level1.Position = new SCNVector3 (6, level1.Position.Y, 0);
				boxesNode.AddChildNode (level1);

				label = Utils.SCBoxNode ("2", new CGRect (0, 0, 40, 40), NSColor.Orange, 20.0f, true);
				label.Position = new SCNVector3 (0, -35, 10);
				label.Scale = new SCNVector3 (0.3f, 0.3f, 0.001f);
				level1.AddChildNode (label);

				level0.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (NMath.PI * 2, new SCNVector3 (0, 1, 0), 45.0)));
				level1.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (NMath.PI * 2, new SCNVector3 (0, 1, 0), 45.0)));

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				SCNTransaction.SetCompletionBlock (() => {
					StarNode.RemoveFromParentNode ();
				});

				// move the camera back to its previous position
				presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
				presentationViewController.CameraPitch.Rotation = new SCNVector4 (1, 0, 0, Pitch * NMath.PI / 180.0f);

				StarNode.Position = new SCNVector3 (StarNode.Position.X, StarNode.Position.Y, StarNode.Position.Z - 30);
				StarOutline.Position = new SCNVector3 (StarOutline.Position.X, StarOutline.Position.Y, StarOutline.Position.Z - 30);

				GroundNode.AddChildNode (boxesNode);

				//move boxes in
				boxesNode.Position = new SCNVector3 (0, 0, 3.5f);

				SCNTransaction.Commit ();
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			// Make sure the camera is back to its default location before leaving the slide
			presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
		}

		// Create a carousel of 3D primitives
		private void PresentPrimitives ()
		{
			// Create the carousel node. It will host all the primitives as child nodes.
			CarouselNode = SCNNode.Create ();
			CarouselNode.Position = new SCNVector3 (0, 0.1f, -5);
			CarouselNode.Scale = new SCNVector3 (0, 0, 0); // start infinitely small
			ContentNode.AddChildNode (CarouselNode);

			// Animate the scale to achieve a "grow" effect
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			CarouselNode.Scale = new SCNVector3 (1, 1, 1);
			SCNTransaction.Commit ();

			// Rotate the carousel forever
			var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
			rotationAnimation.Duration = 40.0f;
			rotationAnimation.RepeatCount = float.MaxValue;
			rotationAnimation.To = NSValue.FromVector (new SCNVector4 (0, 1, 0, NMath.PI * 2));
			CarouselNode.AddAnimation (rotationAnimation, new NSString ("rotationAnimation"));

			// A material shared by all the primitives
			var sharedMaterial = SCNMaterial.Create ();
			sharedMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap", "jpg"));
			sharedMaterial.Reflective.Intensity = 0.2f;
			sharedMaterial.DoubleSided = true;

			PrimitiveIndex = 0;

			// SCNBox
			var box = SCNBox.Create (5.0f, 5.0f, 5.0f, 5.0f * 0.05f);
			box.WidthSegmentCount = 4;
			box.HeightSegmentCount = 4;
			box.LengthSegmentCount = 4;
			box.ChamferSegmentCount = 4;
			AddPrimitive (box, 5.0f / 2, rotationAnimation, sharedMaterial);

			// SCNPyramid
			var pyramid = SCNPyramid.Create (5.0f * 0.8f, 5.0f, 5.0f * 0.8f);
			pyramid.WidthSegmentCount = 4;
			pyramid.HeightSegmentCount = 10;
			pyramid.LengthSegmentCount = 4;
			AddPrimitive (pyramid, 0, rotationAnimation, sharedMaterial);

			// SCNCone
			var cone = SCNCone.Create (0, 5.0f / 2, 5.0f);
			cone.RadialSegmentCount = 20;
			cone.HeightSegmentCount = 4;
			AddPrimitive (cone, 5.0f / 2, rotationAnimation, sharedMaterial);

			// SCNTube
			var tube = SCNTube.Create (5.0f * 0.25f, 5.0f * 0.5f, 5.0f);
			tube.HeightSegmentCount = 5;
			tube.RadialSegmentCount = 40;
			AddPrimitive (tube, 5.0f / 2, rotationAnimation, sharedMaterial);

			// SCNCapsule
			var capsule = SCNCapsule.Create (5.0f * 0.4f, 5.0f * 1.4f);
			capsule.HeightSegmentCount = 5;
			capsule.RadialSegmentCount = 20;
			AddPrimitive (capsule, 5.0f * 0.7f, rotationAnimation, sharedMaterial);

			// SCNCylinder
			var cylinder = SCNCylinder.Create (5.0f * 0.5f, 5.0f);
			cylinder.HeightSegmentCount = 5;
			cylinder.RadialSegmentCount = 40;
			AddPrimitive (cylinder, 5.0f / 2, rotationAnimation, sharedMaterial);

			// SCNSphere
			var sphere = SCNSphere.Create (5.0f * 0.5f);
			sphere.SegmentCount = 20;
			AddPrimitive (sphere, 5.0f / 2, rotationAnimation, sharedMaterial);

			// SCNTorus
			var torus = SCNTorus.Create (5.0f * 0.5f, 5.0f * 0.25f);
			torus.RingSegmentCount = 40;
			torus.PipeSegmentCount = 20;
			AddPrimitive (torus, 5.0f / 4, rotationAnimation, sharedMaterial);

			// SCNPlane
			var plane = SCNPlane.Create (5.0f, 5.0f);
			plane.WidthSegmentCount = 5;
			plane.HeightSegmentCount = 5;
			plane.CornerRadius = 5.0f * 0.1f;
			AddPrimitive (plane, 5.0f / 2, rotationAnimation, sharedMaterial);
		}

		private void AddPrimitive (SCNGeometry geometry, float yPos, CABasicAnimation rotationAnimation, SCNMaterial sharedMaterial)
		{
			var xPos = 13.0f * NMath.Sin (NMath.PI * 2 * PrimitiveIndex / 9.0f);
			var zPos = 13.0f * NMath.Cos (NMath.PI * 2 * PrimitiveIndex / 9.0f);

			var node = SCNNode.Create ();
			node.Position = new SCNVector3 (xPos, yPos, zPos);
			node.Geometry = geometry;
			node.Geometry.FirstMaterial = sharedMaterial;
			CarouselNode.AddChildNode (node);

			PrimitiveIndex++;
			rotationAnimation.TimeOffset = -PrimitiveIndex;
			node.AddAnimation (rotationAnimation, new NSString ("rotationAnimation"));
		}

		private SCNMaterial TextFrontMaterial ()
		{
			var material = SCNMaterial.Create ();
			material.Diffuse.Contents = NSColor.Black;
			material.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap", "jpg"));
			material.Reflective.Intensity = 0.5f;
			material.Multiply.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/gradient2", "tiff"));
			return material;
		}

		private SCNMaterial TextSideAndChamferMaterial ()
		{
			var material = SCNMaterial.Create ();
			material.Diffuse.Contents = NSColor.White;
			material.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/envmap", "jpg"));
			material.Reflective.Intensity = 0.4f;
			return material;
		}

		private NSBezierPath TextChamferProfile ()
		{
			var profile = new NSBezierPath ();
			profile.MoveTo (new CGPoint (0, 1));
			profile.LineTo (new CGPoint (1.5f, 1));
			profile.LineTo (new CGPoint (1.5f, 0));
			profile.LineTo (new CGPoint (1, 0));
			return profile;
		}

		// Takes a string an creates a node hierarchy where each letter is an independent geometry that is animated
		private SCNNode SplittedStylizedText (string message)
		{
			var textNode = SCNNode.Create ();
			var frontMaterial = TextFrontMaterial ();
			var border = TextSideAndChamferMaterial ();

			// Current x position of the next letter to add
			var positionX = (nfloat)0.0;

			// For each letter
			for (var i = 0; i < message.Length; i++) {
				var letterNode = SCNNode.Create ();
				var letterString = message.Substring (i, 1);
				var text = SCNText.Create (letterString, 50.0f);
				text.Font = NSFont.FromFontName ("Avenir Next Heavy", 288);

				text.ChamferRadius = 3.0f;
				text.ChamferProfile = TextChamferProfile ();

				// use a different material for the "heart" character
				var finalFrontMaterial = frontMaterial;
				if (i == 1) {
					finalFrontMaterial = (SCNMaterial)finalFrontMaterial.Copy ();
					finalFrontMaterial.Diffuse.Contents = NSColor.Red;
					finalFrontMaterial.Reflective.Contents = NSColor.Black;
					letterNode.Scale = new SCNVector3 (1.1f, 1.1f, 1.0f);
				}

				text.Materials = new SCNMaterial[] { finalFrontMaterial, finalFrontMaterial, border, border, border };

				letterNode.Geometry = text;
				textNode.AddChildNode (letterNode);

				// measure the letter we just added to update the position
				SCNVector3 min, max;
				max = new SCNVector3 (0, 0, 0);
				min = new SCNVector3 (0, 0, 0);
				if (letterNode.GetBoundingBox (ref min, ref max)) {
					letterNode.Position = new SCNVector3 (positionX - min.X + (max.X + min.X) * 0.5f, -min.Y, 0);
					positionX += max.X;
				} else {
					// if we have no bounding box, it is probably because of the "space" character. In that case, move to the right a little bit.
					positionX += 50.0f;
				}

				// Place the pivot at the center of the letter so that the rotation animation looks good
				letterNode.Pivot = SCNMatrix4.CreateTranslation ((max.X + min.X) * 0.5f, 0, 0);

				// Animate the letter
				var animation = CAKeyFrameAnimation.FromKeyPath ("rotation");
				animation.Duration = 4.0f;
				animation.KeyTimes = new NSNumber[] { 0.0f, 0.3f, 1.0f };
				animation.Values = new NSObject[] {
					NSValue.FromVector (new SCNVector4 (0, 1, 0, 0)),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2)))
				};

				var timingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				animation.TimingFunctions = new CAMediaTimingFunction[] { timingFunction, timingFunction, timingFunction };
				animation.RepeatCount = float.MaxValue;
				animation.BeginTime = CAAnimation.CurrentMediaTime () + 1.0 + i * 0.2; // desynchronize animations
				letterNode.AddAnimation (animation, new NSString ("letterNodeAnimation"));
			}

			return textNode;
		}

		private void PresentTextNode ()
		{
			TextNode = SplittedStylizedText ("I❤︎SceneKit");
			TextNode.Scale = new SCNVector3 (0.017f, 0.0187f, 0.017f);
			TextNode.Opacity = 0.0f;
			TextNode.Position = new SCNVector3 (-14, 0, 0);
			GroundNode.AddChildNode (TextNode);
		}

		private NSBezierPath OutlineChamferProfilePath ()
		{
			var path = new NSBezierPath ();
			path.MoveTo (new CGPoint (1, 1));
			path.LineTo (new CGPoint (1, 0));
			return path;
		}

		private NSBezierPath StarPath (float innerRadius, float outerRadius)
		{
			var raysCount = 5;
			var delta = 2.0f * NMath.PI / raysCount;

			var path = new NSBezierPath ();

			for (var i = 0; i < raysCount; ++i) {
				var alpha = i * delta + NMath.PI / 2;

				if (i == 0)
					path.MoveTo (new CGPoint (outerRadius * NMath.Cos (alpha), outerRadius * NMath.Sin (alpha)));
				else
					path.LineTo (new CGPoint (outerRadius * NMath.Cos (alpha), outerRadius * NMath.Sin (alpha)));

				alpha += 0.5f * delta;
				path.LineTo (new CGPoint (innerRadius * NMath.Cos (alpha), innerRadius * NMath.Sin (alpha)));
			}

			return path;
		}
	}
}