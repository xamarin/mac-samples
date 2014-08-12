using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.SceneKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideAllNew : Slide
	{
		private SCNMaterial[] Materials { get; set; }

		private NSFont Font { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Create the font and the materials that will be shared among the features in the word cloud
			Font = NSFont.FromFontName ("Myriad Set BoldItalic", 50) != null ? NSFont.FromFontName ("Myriad Set BoldItalic", 50) : NSFont.FromFontName ("Avenir Heavy Oblique", 50);

			var frontAndBackMaterial = SCNMaterial.Create ();
			var sideMaterial = SCNMaterial.Create ();
			sideMaterial.Diffuse.Contents = NSColor.DarkGray;

			Materials = new SCNMaterial[] { frontAndBackMaterial, sideMaterial, frontAndBackMaterial };

			// Add different features to the word cloud
			PlaceFeature ("Export to DAE", new PointF (10, -8), 0);
			PlaceFeature ("OpenGL Core Profile", new PointF (-16, -7), 0.05f);
			PlaceFeature ("Warmup", new PointF (-12, -10), 0.1f);
			PlaceFeature ("Constraints", new PointF (-10, 6), 0.15f);
			PlaceFeature ("Custom projection", new PointF (4, 9), 0.2f);
			PlaceFeature ("Skinning", new PointF (-4, 8), 0.25f);
			PlaceFeature ("Morphing", new PointF (-3, -8), 0.3f);
			PlaceFeature ("Performance Statistics", new PointF (-1, 6), 0.35f);
			PlaceFeature ("CIFilters", new PointF (1, 5), 0.85f);
			PlaceFeature ("GLKit Math", new PointF (3, -10), 0.45f);
			PlaceFeature ("Depth of Field", new PointF (-0.5f, 0), 0.47f);
			PlaceFeature ("Animation Events", new PointF (5, 3), 0.50f);
			PlaceFeature ("Shader Modifiers", new PointF (7, 2), 0.95f);
			PlaceFeature ("GOBO", new PointF (-10, 1), 0.60f);
			PlaceFeature ("Ray testing", new PointF (-8, 0), 0.65f);
			PlaceFeature ("Skybox", new PointF (8, -1), 0.7f);
			PlaceFeature ("Fresnel", new PointF (6, -2), 0.75f);
			PlaceFeature ("SCNShape", new PointF (-6, -3), 0.8f);
			PlaceFeature ("Levels of detail", new PointF (-11, 3), 0.9f);
			PlaceFeature ("Animation blending", new PointF (-2, -5), 1);
		}

		private void PlaceFeature (string message, PointF p, float offset)
		{
			// Create and configure a node with a text geometry, and add it to the scene
			var text = SCNText.Create (message, 5);
			text.Font = Font;
			text.Flatness = 0.4f;
			text.Materials = Materials;

			var textNode = SCNNode.Create ();
			textNode.Geometry = text;
			textNode.Position = new SCNVector3 (p.X, p.Y + Altitude, 0);
			textNode.Scale = new SCNVector3 (0.02f, 0.02f, 0.02f);

			ContentNode.AddChildNode (textNode);

			// Animation the node's position and opacity
			var positionAnimation = CABasicAnimation.FromKeyPath ("position.z");
			positionAnimation.From = NSNumber.FromInt16 (-10);
			positionAnimation.To = NSNumber.FromInt16 (10);
			positionAnimation.Duration = 5.0f;
			positionAnimation.TimeOffset = -offset * positionAnimation.Duration;
			positionAnimation.RepeatCount = float.MaxValue;
			textNode.AddAnimation (positionAnimation, new NSString ("positionAnimation"));

			var opacityAnimation = CAKeyFrameAnimation.GetFromKeyPath ("opacity");
			opacityAnimation.KeyTimes = new NSNumber[] { 0.0f, 0.2f, 0.9f, 1.0f };
			opacityAnimation.Values = new NSNumber[] { 0.0f, 1.0f, 1.0f, 0.0f };
			opacityAnimation.Duration = positionAnimation.Duration;
			opacityAnimation.TimeOffset = positionAnimation.TimeOffset;
			opacityAnimation.RepeatCount = float.MaxValue;
			textNode.AddAnimation (opacityAnimation, new NSString ("opacityAnimation"));
		}
	}
}

