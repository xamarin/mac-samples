using System;
using System.Globalization;
using System.Collections.Generic;

using AppKit;
using OpenGL;
using SceneKit;
using Foundation;
using CoreGraphics;

namespace SceneKitSessionWWDC2013
{
	public class Utils : SCNNode
	{
		public const long NSEC_PER_SEC = 1000000000;

		public enum LabelSize
		{
			Small = 1,
			Normal = 2,
			Large = 4
		}

		public static SCNNode SCAddChildNode (SCNNode container, string name, string path, nfloat scale)
		{
			// Load the scene from the specified file
			var scene = SCNScene.FromFile (path);

			// Retrieve the root node
			var node = scene.RootNode;

			// Search for the node named "name"
			if (name.Length > 0)
				node = node.FindChildNode (name, true);
			else
				node = node.ChildNodes [0]; // Take the first child if no name is passed

			if (scale != 0) {
				// Rescale based on the current bounding box and the desired scale
				// Align the node to 0 on the Y axis
				var min = new SCNVector3 ();
				var max = new SCNVector3 ();
				node.GetBoundingBox (ref min, ref max);

				var mid = SCNVector3.Add (min, max);
				mid = SCNVector3.Multiply (mid, 0.5f);
				mid.Y = min.Y; // Align on bottom

				var size = SCNVector3.Subtract (max, min);
				var maxSize = NMath.Max (NMath.Max (size.X, size.Y), size.Z);

				scale = scale / maxSize;
				mid = SCNVector3.Multiply (mid, scale);
				mid = -mid;

				node.Scale = new SCNVector3 (scale, scale, scale);
				node.Position = new SCNVector3 (mid);
			}

			// Add to the container passed in argument
			container.AddChildNode (node);

			return node;
		}

		public static SCNNode SCBoxNode (string title, CGRect frame, NSColor color, float cornerRadius, bool centered)
		{
			NSMutableDictionary titleAttributes = null;
			NSMutableDictionary centeredTitleAttributes = null;

			// create and extrude a bezier path to build the box
			var path = NSBezierPath.FromRoundedRect (frame, cornerRadius, cornerRadius);
			path.Flatness = 0.05f;

			var shape = SCNShape.Create (path, 20);
			shape.ChamferRadius = 0.0f;

			var node = SCNNode.Create ();
			node.Geometry = shape;

			// create an image and fill with the color and text
			var textureSize = new CGSize ();
			textureSize.Width = NMath.Ceiling (frame.Size.Width * 1.5f);
			textureSize.Height = NMath.Ceiling (frame.Size.Height * 1.5f);

			var texture = new NSImage (textureSize);
			texture.LockFocus ();

			var drawFrame = new CGRect (0, 0, textureSize.Width, textureSize.Height);

			nfloat hue, saturation, brightness, alpha;

			(color.UsingColorSpace (NSColorSpace.DeviceRGBColorSpace)).GetHsba (out hue, out saturation, out brightness, out alpha);
			var lightColor = NSColor.FromDeviceHsba (hue, saturation - 0.2f, brightness + 0.3f, alpha);
			lightColor.Set ();

			NSGraphics.RectFill (drawFrame);

			NSBezierPath fillpath = null;

			if (cornerRadius == 0 && centered == false) {
				//special case for the "labs" slide
				drawFrame.Offset (0, -2);
				fillpath = NSBezierPath.FromRoundedRect (drawFrame, cornerRadius, cornerRadius);
			} else {
				drawFrame.Inflate (-3, -3);
				fillpath = NSBezierPath.FromRoundedRect (drawFrame, cornerRadius, cornerRadius);
			}

			color.Set ();
			fillpath.Fill ();

			// draw the title if any
			if (title != null) {
				if (titleAttributes == null) {
					var paraphStyle = new NSMutableParagraphStyle ();
					paraphStyle.LineBreakMode = NSLineBreakMode.ByWordWrapping;
					paraphStyle.Alignment = NSTextAlignment.Center;
					paraphStyle.MinimumLineHeight = 38;
					paraphStyle.MaximumLineHeight = 38;

					var font = NSFont.FromFontName ("Myriad Set Semibold", 34) != null ? NSFont.FromFontName ("Myriad Set Semibold", 34) : NSFont.FromFontName ("Avenir Medium", 34);

					var shadow = new NSShadow ();
					shadow.ShadowOffset = new CGSize (0, -2);
					shadow.ShadowBlurRadius = 4;
					shadow.ShadowColor = NSColor.FromDeviceWhite (0.0f, 0.5f);

					titleAttributes = new NSMutableDictionary ();
					titleAttributes.SetValueForKey (font, NSAttributedString.FontAttributeName);
					titleAttributes.SetValueForKey (NSColor.White, NSAttributedString.ForegroundColorAttributeName);
					titleAttributes.SetValueForKey (shadow, NSAttributedString.ShadowAttributeName);
					titleAttributes.SetValueForKey (paraphStyle, NSAttributedString.ParagraphStyleAttributeName);

					var centeredParaphStyle = (NSMutableParagraphStyle)paraphStyle.MutableCopy ();
					centeredParaphStyle.Alignment = NSTextAlignment.Center;

					centeredTitleAttributes = new NSMutableDictionary ();
					centeredTitleAttributes.SetValueForKey (font, NSAttributedString.FontAttributeName);
					centeredTitleAttributes.SetValueForKey (NSColor.White, NSAttributedString.ForegroundColorAttributeName);
					centeredTitleAttributes.SetValueForKey (shadow, NSAttributedString.ShadowAttributeName);
					centeredTitleAttributes.SetValueForKey (paraphStyle, NSAttributedString.ParagraphStyleAttributeName);
				}

				var attrString = new NSAttributedString (title, centered ? centeredTitleAttributes : titleAttributes);
				var textSize = attrString.Size;

				//check if we need two lines to draw the text
				var twoLines = title.Contains ("\n");
				if (!twoLines)
					twoLines = textSize.Width > frame.Size.Width && title.Contains (" ");

				//if so, we need to adjust the size to center vertically
				if (twoLines)
					textSize.Height += 38;

				if (!centered)
					drawFrame.Inflate (-15, 0);

				//center vertically
				var dy = (drawFrame.Size.Height - textSize.Height) * 0.5f;
				var drawFrameHeight = drawFrame.Size.Height;
				drawFrame.Size = new CGSize (drawFrame.Size.Width, drawFrame.Size.Height - dy);
				attrString.DrawString (drawFrame);
			}

			texture.UnlockFocus ();

			//set the created image as the diffuse texture of our 3D box
			var front = SCNMaterial.Create ();
			front.Diffuse.Contents = texture;
			front.LocksAmbientWithDiffuse = true;

			//use a lighter color for the chamfer and sides
			var sides = SCNMaterial.Create ();
			sides.Diffuse.Contents = lightColor;
			node.Geometry.Materials = new SCNMaterial[] {
				front,
				sides,
				sides,
				sides,
				sides
			};

			return node;
		}

		public static SCNNode SCPlaneNodeWithImage (NSImage image, nfloat size, bool isLit)
		{
			var node = SCNNode.Create ();

			var factor = size / (nfloat)(Math.Max (image.Size.Width, image.Size.Height));

			node.Geometry = SCNPlane.Create (image.Size.Width * factor, image.Size.Height * factor);
			node.Geometry.FirstMaterial.Diffuse.Contents = image;

			//if we don't want the image to be lit, set the lighting model to "constant"
			if (!isLit)
				node.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;

			return node;
		}

		public static SCNNode SCPlaneNode (string path, float size, bool isLit)
		{
			return Utils.SCPlaneNodeWithImage (new NSImage (path), size, isLit);
		}

		public static SCNNode SCLabelNode (string message, LabelSize size, bool isLit)
		{
			var node = SCNNode.Create ();

			var text = SCNText.Create (message, 0);
			node.Geometry = text;
			node.Scale = new SCNVector3 ((0.01f * (int)size), (0.01f * (int)size), (0.01f * (int)size));
			text.Flatness = 0.4f;

			// Use Myriad it's if available, otherwise Avenir
			text.Font = NSFont.FromFontName ("Myriad Set", 50) != null ? NSFont.FromFontName ("Myriad Set", 50) : NSFont.FromFontName ("Avenir Medium", 50);

			if (!isLit)
				text.FirstMaterial.LightingModelName = SCNLightingModel.Constant;

			return node;
		}

		public static SCNNode SCGaugeNode (string title, ref SCNNode progressNode)
		{
			var gaugeGroup = SCNNode.Create ();

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			var gauge = SCNNode.Create ();
			gauge.Geometry = SCNCapsule.Create (0.4f, 8);
			gauge.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
			gauge.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI / 2));
			gauge.Geometry.FirstMaterial.Diffuse.Contents = NSColor.White;
			gauge.Geometry.FirstMaterial.CullMode = SCNCullMode.Front;

			var gaugeValue = SCNNode.Create ();
			gaugeValue.Geometry = SCNCapsule.Create (0.3f, 7.8f);
			gaugeValue.Pivot = SCNMatrix4.CreateTranslation (new SCNVector3 (0, 3.8f, 0));
			gaugeValue.Position = new SCNVector3 (0, 3.8f, 0);
			gaugeValue.Scale = new SCNVector3 (1, 0.01f, 1);
			gaugeValue.Opacity = 0.0f;
			gaugeValue.Geometry.FirstMaterial.Diffuse.Contents = NSColor.FromDeviceRgba (0, 1, 0, 1);
			gaugeValue.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
			gauge.AddChildNode (gaugeValue);

			if (progressNode != null) {
				progressNode = gaugeValue;
			}

			var titleNode = Utils.SCLabelNode (title, LabelSize.Normal, false);
			titleNode.Position = new SCNVector3 (-8, -0.55f, 0);

			gaugeGroup.AddChildNode (titleNode);
			gaugeGroup.AddChildNode (gauge);
			SCNTransaction.Commit ();

			return gaugeGroup;
		}

		public static NSBezierPath SCArrowBezierPath (CGSize baseSize, CGSize tipSize, nfloat hollow, bool twoSides)
		{
			var arrow = new NSBezierPath ();

			var h = new nfloat[5];
			var w = new nfloat[4];

			w [0] = 0;
			w [1] = baseSize.Width - tipSize.Width - hollow;
			w [2] = baseSize.Width - tipSize.Width;
			w [3] = baseSize.Width;

			h [0] = 0;
			h [1] = (tipSize.Height - baseSize.Height) * 0.5f;
			h [2] = (tipSize.Height) * 0.5f;
			h [3] = (tipSize.Height + baseSize.Height) * 0.5f;
			h [4] = tipSize.Height;

			if (twoSides) {
				arrow.MoveTo (new CGPoint (tipSize.Width, h [1]));
				arrow.LineTo (new CGPoint (tipSize.Width + hollow, h [0]));
				arrow.LineTo (new CGPoint (0, h [2]));
				arrow.LineTo (new CGPoint (tipSize.Width + hollow, h [4]));
				arrow.LineTo (new CGPoint (tipSize.Width, h [3]));
			} else {
				arrow.MoveTo (new CGPoint (0, h [1]));
				arrow.LineTo (new CGPoint (0, h [3]));
			}

			arrow.LineTo (new CGPoint (w [2], h [3]));
			arrow.LineTo (new CGPoint (w [1], h [4]));
			arrow.LineTo (new CGPoint (w [3], h [2]));
			arrow.LineTo (new CGPoint (w [1], h [0]));
			arrow.LineTo (new CGPoint (w [2], h [1]));

			arrow.ClosePath ();

			return arrow;
		}

		public static NSImage SCImageFromApplication (string name)
		{
			NSImage image = null;

			var path = NSWorkspace.SharedWorkspace.FullPathForApplication (name);
			if (path != null) {
				image = NSWorkspace.SharedWorkspace.IconForFile (path); 
				image = Utils.SCCopyWithResolution (image, 512);
			}

			if (image == null) {
				image = NSImage.ImageNamed (NSImageName.Caution);
			}

			return image;
		}

		private static NSImage SCCopyWithResolution (NSImage image, float size)
		{
			var imageRep = image.BestRepresentation (new CGRect (0, 0, size, size), null, null);
			if (imageRep != null) {
				return new NSImage (imageRep.CGImage, imageRep.Size);
			}
			return image;
		}
	}
}