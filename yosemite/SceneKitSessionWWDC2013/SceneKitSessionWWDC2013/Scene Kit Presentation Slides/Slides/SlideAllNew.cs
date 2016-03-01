using AppKit;
using SceneKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;

namespace SceneKitSessionWWDC2013 {
	public class SlideAllNew : Slide {
		SCNMaterial[] Materials { get; set; }

		NSFont Font { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Create the font and the materials that will be shared among the features in the word cloud
			Font = NSFont.FromFontName ("Myriad Set BoldItalic", 50) != null ? NSFont.FromFontName ("Myriad Set BoldItalic", 50) : NSFont.FromFontName ("Avenir Heavy Oblique", 50);

			var frontAndBackMaterial = SCNMaterial.Create ();
			var sideMaterial = SCNMaterial.Create ();
			sideMaterial.Diffuse.Contents = NSColor.DarkGray;

			Materials = new SCNMaterial[] { frontAndBackMaterial, sideMaterial, frontAndBackMaterial };

			// Add different features to the word cloud
			PlaceFeature ("Export to DAE", new CGPoint (10, -8), 0);
			PlaceFeature ("OpenGL Core Profile", new CGPoint (-16, -7), 0.05f);
			PlaceFeature ("Warmup", new CGPoint (-12, -10), 0.1f);
			PlaceFeature ("Constraints", new CGPoint (-10, 6), 0.15f);
			PlaceFeature ("Custom projection", new CGPoint (4, 9), 0.2f);
			PlaceFeature ("Skinning", new CGPoint (-4, 8), 0.25f);
			PlaceFeature ("Morphing", new CGPoint (-3, -8), 0.3f);
			PlaceFeature ("Performance Statistics", new CGPoint (-1, 6), 0.35f);
			PlaceFeature ("CIFilters", new CGPoint (1, 5), 0.85f);
			PlaceFeature ("GLKit Math", new CGPoint (3, -10), 0.45f);
			PlaceFeature ("Depth of Field", new CGPoint (-0.5f, 0), 0.47f);
			PlaceFeature ("Animation Events", new CGPoint (5, 3), 0.50f);
			PlaceFeature ("Shader Modifiers", new CGPoint (7, 2), 0.95f);
			PlaceFeature ("GOBO", new CGPoint (-10, 1), 0.60f);
			PlaceFeature ("Ray testing", new CGPoint (-8, 0), 0.65f);
			PlaceFeature ("Skybox", new CGPoint (8, -1), 0.7f);
			PlaceFeature ("Fresnel", new CGPoint (6, -2), 0.75f);
			PlaceFeature ("SCNShape", new CGPoint (-6, -3), 0.8f);
			PlaceFeature ("Levels of detail", new CGPoint (-11, 3), 0.9f);
			PlaceFeature ("Animation blending", new CGPoint (-2, -5), 1);
		}

		void PlaceFeature (string message, CGPoint p, float offset)
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

