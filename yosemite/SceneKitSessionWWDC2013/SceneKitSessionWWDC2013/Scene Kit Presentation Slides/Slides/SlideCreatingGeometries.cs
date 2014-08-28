using System;
using System.Runtime.InteropServices;

using AppKit;
using SceneKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideCreatingGeometries : Slide
	{
		private SCNNode CarouselNode { get; set; }

		private SCNNode TextNode { get; set; }

		private SCNNode Level2OutlineNode, Level2Node;

		private int PrimitiveIndex { get; set; }

		public override int NumberOfSteps ()
		{
			return 7;
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
				break;
			case 1:
				// Set the slide's subtitle and display the primitves
				TextManager.SetSubtitle ("Built-in parametric primitives");
				PresentPrimitives ();
				break;
			case 2:
				// Hide the carousel and illustrate SCNText
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				SCNTransaction.SetCompletionBlock (() => {
					CarouselNode.RemoveFromParentNode ();

					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 1;
					PresentTextNode ();
					TextNode.Opacity = 1.0f;
					SCNTransaction.Commit ();
				});
				CarouselNode.Opacity = 0.0f;
				SCNTransaction.Commit ();

				TextManager.SetSubtitle ("Built-in 3D text");
				TextManager.AddBulletAtLevel ("SCNText", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			case 3:
				// Hide the 3D text and introduce SCNShape
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				SCNTransaction.SetCompletionBlock (() => {
					if (TextNode != null)
						TextNode.RemoveFromParentNode ();

					presentationViewController.ShowsNewInSceneKitBadge (true);

					TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
					TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

					TextManager.SetSubtitle ("3D Shapes");

					TextManager.AddBulletAtLevel ("SCNShape", 0);
					TextManager.AddBulletAtLevel ("Initializes with a NSBezierPath", 0);
					TextManager.AddBulletAtLevel ("Extrusion and chamfer", 0);

					TextManager.AddCode ("#aNode.Geometry = SCNShape.#Create# (aBezierPath, 10);#");

					TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
					TextManager.FlipInText (SlideTextManager.TextType.Bullet);
					TextManager.FlipInText (SlideTextManager.TextType.Code);
				});
				if (TextNode != null)
					TextNode.Opacity = 0.0f;
				SCNTransaction.Commit ();
				break;
			case 4:
				TextManager.FadeOutText (SlideTextManager.TextType.Bullet);
				TextManager.FadeOutText (SlideTextManager.TextType.Code);

				// Illustrate SCNShape, show the floor ouline
				Level2Node = Level2 ();
				Level2OutlineNode = Level2Outline ();

				Level2Node.Position = Level2OutlineNode.Position = new SCNVector3 (-11, 0, -5);
				Level2Node.Rotation = Level2OutlineNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
				Level2Node.Opacity = Level2OutlineNode.Opacity = 0.0f;
				Level2Node.Scale = new SCNVector3 (0.03f, 0.03f, 0);
				Level2OutlineNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.05f);

				GroundNode.AddChildNode (Level2OutlineNode);
				GroundNode.AddChildNode (Level2Node);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				Level2OutlineNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case 5:
				presentationViewController.ShowsNewInSceneKitBadge (false);

				// Show the extruded floor
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				Level2Node.Opacity = 1.0f;
				Level2Node.Scale = new SCNVector3 (0.03f, 0.03f, 0.05f);

				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 1.5f;
					// move the camera a little higher
					presentationViewController.CameraNode.Position = new SCNVector3 (0, 7, -3);
					presentationViewController.CameraPitch.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 4) * 0.7f);
					SCNTransaction.Commit ();
				});
				SCNTransaction.Commit ();
				break;
			case 6:
				TextManager.FadeOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				// Example of a custom geometry (Möbius strip)
				TextManager.SetSubtitle ("Custom geometry");

				TextManager.AddBulletAtLevel ("Custom vertices, normals and texture coordinates", 0);
				TextManager.AddBulletAtLevel ("SCNGeometry", 0);

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				// move the camera back to its previous position
				presentationViewController.CameraNode.Position = new SCNVector3 (0, 0, 0);
				presentationViewController.CameraPitch.Rotation = new SCNVector4 (1, 0, 0, Pitch * (float)(Math.PI / 180.0));

				Level2Node.Opacity = 0.0f;
				Level2OutlineNode.Opacity = 0.0f;

				SCNTransaction.Commit ();

				break;
			}
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
			var positionX = 0.0f;

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
					positionX += (float)max.X;
				} else {
					// if we have no bounding box, it is probably because of the "space" character. In that case, move to the right a little bit.
					positionX += 50.0f;
				}

				// Place the pivot at the center of the letter so that the rotation animation looks good
				letterNode.Pivot = SCNMatrix4.CreateTranslation ((max.X + min.X) * 0.5f, 0, 0);

				// Animate the letter
				var animation = CAKeyFrameAnimation.GetFromKeyPath ("rotation");
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

		private SCNMaterial[] FloorMaterials ()
		{
			var lightGrayMaterial = SCNMaterial.Create ();
			lightGrayMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/shapeMap", "png"));
			lightGrayMaterial.LocksAmbientWithDiffuse = true;

			var darkGrayMaterial = SCNMaterial.Create ();
			darkGrayMaterial.Diffuse.Contents = NSColor.FromDeviceWhite (0.8f, 1.0f);
			darkGrayMaterial.LocksAmbientWithDiffuse = true;

			return new SCNMaterial[] { lightGrayMaterial, lightGrayMaterial, darkGrayMaterial };
		}

		private SCNMaterial WallMaterial ()
		{
			var material = SCNMaterial.Create ();
			material.Diffuse.Contents = NSColor.FromDeviceRgba (0.11f, 0.70f, 0.88f, 1.0f);
			return material;
		}

		private SCNNode Level2 ()
		{
			var node = SCNNode.Create ();
			var roomsNode = SCNNode.Create ();

			var floor = SCNShape.Create (MosconeFloor (), 10.0f);
			var walls = SCNShape.Create (MosconeRooms (), 20.0f);

			node.Geometry = floor;
			node.Geometry.Materials = FloorMaterials ();

			roomsNode.Geometry = walls;
			roomsNode.Geometry.FirstMaterial = WallMaterial ();
			roomsNode.Pivot = SCNMatrix4.CreateTranslation (0, 0, -0.5f * 20.0f);
			roomsNode.Opacity = 1.0f;

			node.AddChildNode (roomsNode);

			return node;
		}

		private SCNNode Level2Outline ()
		{
			var floor = SCNShape.Create (MosconeFloor (), 10.0f * 1.01f);
			floor.ChamferRadius = 10.0f;
			floor.ChamferProfile = OutlineChamferProfilePath ();
			floor.ChamferMode = SCNChamferMode.Front;

			// Use a transparent material for everything except the chamfer. That way only the outline of the model will be visible.
			var outlineMaterial = SCNMaterial.Create ();
			outlineMaterial.Ambient.Contents = outlineMaterial.Diffuse.Contents = outlineMaterial.Specular.Contents = NSColor.Black;
			outlineMaterial.Emission.Contents = NSColor.White;

			var tranparentMaterial = SCNMaterial.Create ();
			tranparentMaterial.Transparency = 0.0f;

			var node = SCNNode.Create ();
			node.Geometry = floor;
			node.Geometry.Materials = new SCNMaterial[] {
				tranparentMaterial,
				tranparentMaterial,
				tranparentMaterial,
				outlineMaterial,
				outlineMaterial
			};

			return node;
		}

		private NSBezierPath MosconeFloor ()
		{
			var path = new NSBezierPath ();

			path.MoveTo (new CGPoint (69, 0));
			path.LineTo (new CGPoint (69, -107));
			path.LineTo (new CGPoint (0, -107));
			path.LineTo (new CGPoint (0, -480));
			path.LineTo (new CGPoint (104, -480));
			path.LineTo (new CGPoint (104, -500));
			path.LineTo (new CGPoint (184, -480));
			path.LineTo (new CGPoint (226, -480));
			path.LineTo (new CGPoint (226, -500));
			path.LineTo (new CGPoint (306, -480));
			path.LineTo (new CGPoint (348, -480));
			path.LineTo (new CGPoint (348, -500));
			path.LineTo (new CGPoint (428, -480));
			path.LineTo (new CGPoint (470, -480));
			path.LineTo (new CGPoint (470, -500));
			path.LineTo (new CGPoint (550, -480));
			path.LineTo (new CGPoint (592, -480));
			path.LineTo (new CGPoint (592, -505));
			path.LineTo (new CGPoint (752.548776f, -460.046343f));
			path.CurveTo (new CGPoint (767.32333f, -440.999893f), new CGPoint (760.529967f, -457.811609f), new CGPoint (767.218912f, -449.292876f));
			path.CurveTo (new CGPoint (700, 0), new CGPoint (767.32333f, -440.999893f), new CGPoint (776, -291));
			path.LineTo (new CGPoint (69, 0));

			path.MoveTo (new CGPoint (676, -238));
			path.LineTo (new CGPoint (676, -348));
			path.LineTo (new CGPoint (710, -348));
			path.LineTo (new CGPoint (710, -238));
			path.LineTo (new CGPoint (676, -238));
			path.LineTo (new CGPoint (676, -238));

			return path;
		}

		private NSBezierPath MosconeRooms ()
		{
			var path = new NSBezierPath ();

			path.MoveTo (new CGPoint (553, -387));
			path.LineTo (new CGPoint (426, -387));
			path.LineTo (new CGPoint (426, -383));
			path.LineTo (new CGPoint (549, -383));
			path.LineTo (new CGPoint (549, -194));
			path.LineTo (new CGPoint (357, -194));
			path.LineTo (new CGPoint (357, -383));
			path.LineTo (new CGPoint (411, -383));
			path.LineTo (new CGPoint (411, -387));
			path.LineTo (new CGPoint (255, -387));
			path.LineTo (new CGPoint (255, -383));
			path.LineTo (new CGPoint (353, -383));
			path.LineTo (new CGPoint (353, -194));
			path.LineTo (new CGPoint (175, -194));
			path.LineTo (new CGPoint (175, -383));
			path.LineTo (new CGPoint (240, -383));
			path.LineTo (new CGPoint (240, -387));
			path.LineTo (new CGPoint (171, -387));
			path.LineTo (new CGPoint (171, -190));
			path.LineTo (new CGPoint (553, -190));
			path.LineTo (new CGPoint (553, -387));

			path.MoveTo (new CGPoint (474, -141));
			path.LineTo (new CGPoint (474, -14));
			path.LineTo (new CGPoint (294, -14));
			path.LineTo (new CGPoint (294, -141));
			path.LineTo (new CGPoint (407, -141));
			path.LineTo (new CGPoint (407, -145));
			path.LineTo (new CGPoint (172, -145));
			path.LineTo (new CGPoint (172, -141));
			path.LineTo (new CGPoint (290, -141));
			path.LineTo (new CGPoint (290, -14));
			path.LineTo (new CGPoint (124, -14));
			path.LineTo (new CGPoint (124, -141));
			path.LineTo (new CGPoint (157, -141));
			path.LineTo (new CGPoint (157, -145));
			path.LineTo (new CGPoint (120, -145));
			path.LineTo (new CGPoint (120, -10));
			path.LineTo (new CGPoint (478, -10));
			path.LineTo (new CGPoint (478, -145));
			path.LineTo (new CGPoint (422, -145));
			path.LineTo (new CGPoint (422, -141));
			path.LineTo (new CGPoint (474, -141));

			return path;
		}

		private NSBezierPath OutlineChamferProfilePath ()
		{
			var path = new NSBezierPath ();
			path.MoveTo (new CGPoint (1, 1));
			path.LineTo (new CGPoint (1, 0));
			return path;
		}
	}
}