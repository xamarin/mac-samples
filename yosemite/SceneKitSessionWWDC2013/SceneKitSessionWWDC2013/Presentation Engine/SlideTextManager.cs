using System;

using AppKit;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013 {
	public class SlideTextManager {
		const float TEXT_SCALE = 0.02f;
		const float TEXT_CHAMFER = 1;
		const float TEXT_DEPTH = 0.0f;
		const float TEXT_FLATNESS = 0.4f;

		public enum TextType {
			None,
			Chapter,
			Title,
			Subtitle,
			Bullet,
			Body,
			Code,
			Count
		}

		SCNNode[] SubGroups = new SCNNode[(int)TextType.Count];

		TextType PreviousType { get; set; }

		float CurrentBaseline { get; set; }

		float[] BaselinePerType = new float[(int)TextType.Count];

		public SCNNode TextNode { get; set; }

		public bool FadesIn { get; set; }

		public SlideTextManager ()
		{
			TextNode = SCNNode.Create ();
			CurrentBaseline = 16;
		}

		NSColor ColorForTextType (TextType type, int level)
		{
			NSColor color = null;
			switch (type) {
			case TextType.Subtitle:
				color = NSColor.FromDeviceRgba (160.0f / 255.0f, 182.0f / 255.0f, 203.0f / 255.0f, 1);
				break;
			case TextType.Code:
				color = level == 0 ? NSColor.White : NSColor.FromDeviceRgba (242.0f / 255.0f, 173.0f / 255.0f, 24.0f / 255.0f, 1);
				break;
			case TextType.Body:
				if (level == 2)
					color = NSColor.FromDeviceRgba (115.0f / 255.0f, 170.0f / 255.0f, 230.0f / 255.0f, 1);
				break;
			default:
				color = NSColor.White;
				break;
			}
			return color;
		}

		float ExtrusionDepthForTextType (TextType type)
		{
			return type == TextType.Chapter ? 10.0f : TEXT_DEPTH;
		}

		float FontCGSizeorTextType (TextType type, int level)
		{
			float fontSize = 0;
			switch (type) {
			case TextType.Title:
				fontSize = 88;
				break;
			case TextType.Chapter:
				fontSize = 94;
				break;
			case TextType.Code:
				fontSize = 36;
				break;
			case TextType.Subtitle:
				fontSize = 64;
				break;
			case TextType.Body:
				fontSize = level == 0 ? 50 : 40;
				break;
			default:
				fontSize = 56;
				break;
			}
			return fontSize;
		}

		NSFont FontForTextType (TextType type, int level)
		{
			var fontSize = FontCGSizeorTextType (type, level);
			var font = NSFont.FromFontName("Myriad Set Semibold", fontSize) != null ? NSFont.FromFontName("Myriad Set", fontSize) : NSFont.FromFontName("Avenir Medium", fontSize);

			switch (type) {
			case TextType.Code:
				font = NSFont.FromFontName ("Menlo", fontSize);
				break;
			case TextType.Bullet:
				font = NSFont.FromFontName ("Myriad Set", fontSize) != null ? NSFont.FromFontName ("Myriad Set", fontSize) : NSFont.FromFontName ("Avenir Medium", fontSize);
				break;
			case TextType.Body:
				if (level != 0)
					font = NSFont.FromFontName ("Myriad Set", fontSize) != null ? NSFont.FromFontName ("Myriad Set", fontSize) : NSFont.FromFontName ("Avenir Medium", fontSize);
				break;
			}
			return font;
		}

		private float LineHeightForTextType (TextType type, int level)
		{
			var lineHeight = 0.0f;
			switch (type) {
			case TextType.Title:
				lineHeight = 2.26f;
				break;
			case TextType.Chapter:
				lineHeight = 3;
				break;
			case TextType.Code:
				lineHeight = 1.22f;
				break;
			case TextType.Subtitle:
				lineHeight = 1.78f;
				break;
			case TextType.Body:
				lineHeight = (level == 0 ? 1.2f : 1.0f);
				break;
			default:
				lineHeight = 1.65f;
				break;
			}
			return lineHeight;
		}

		SCNNode TextContainerForType (TextType type)
		{
			if (type == TextType.Chapter)
				return TextNode.ParentNode;

			if (SubGroups [(int)type] != null)
				return SubGroups [(int)type];

			var container = SCNNode.Create ();
			TextNode.AddChildNode (container);

			SubGroups [(int)type] = container;
			BaselinePerType [(int)type] = CurrentBaseline;

			return container;
		}

		public void AddEmptyLine ()
		{
			CurrentBaseline -= 1.2f;
		}


		SCNNode NodeWithText (string message, TextType type, int level)
		{
			var textNode = SCNNode.Create ();

			// Bullet
			if (type == TextType.Bullet) {
				if (level == 0)
					message = "• " + message;
				else {
					var bullet = SCNNode.Create ();
					bullet.Geometry = SCNPlane.Create (10.0f, 10.0f);
					bullet.Geometry.FirstMaterial.Diffuse.Contents = NSColor.FromDeviceRgba ((float)(160 / 255.0), (float)(182 / 255.0), (float)(203 / 255.0), 1);
					bullet.Position = new SCNVector3 (80, 30, 0);
					bullet.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
					bullet.Geometry.FirstMaterial.WritesToDepthBuffer = false;
					bullet.RenderingOrder = 1;
					textNode.AddChildNode (bullet);
					message = "\t\t\t\t" + message;
				}
			}

			// Text attributes
			var extrusion = ExtrusionDepthForTextType (type);
			var text = SCNText.Create (message, extrusion);
			textNode.Geometry = text;
			text.Flatness = TEXT_FLATNESS;
			text.ChamferRadius = (extrusion == 0 ? 0 : TEXT_CHAMFER);
			text.Font = FontForTextType (type, level);

			// Layout
			var layoutManager = new NSLayoutManager ();
			var leading = layoutManager.DefaultLineHeightForFont (text.Font);
			var descender = text.Font.Descender;
			int newlineCount = (((text.String.ToString ()).Split ('\n'))).Length;
			textNode.Pivot = SCNMatrix4.CreateTranslation (0, -descender + newlineCount * leading, 0);

			if (type == TextType.Chapter) {
				var min = new SCNVector3 ();
				var max = new SCNVector3 ();
				textNode.GetBoundingBox (ref min, ref max);
				textNode.Position = new SCNVector3 (-11, (-min.Y + textNode.Pivot.M42) * TEXT_SCALE, 7);
				textNode.Scale = new SCNVector3 (TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);
				textNode.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 270.0));
			} else {
				textNode.Position = new SCNVector3 (-16, CurrentBaseline, 0);
				textNode.Scale = new SCNVector3 (TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);
			}

			// Material
			if (type == TextType.Chapter) {
				var frontMaterial = SCNMaterial.Create ();
				var sideMaterial = SCNMaterial.Create ();

				frontMaterial.Emission.Contents = NSColor.DarkGray;
				frontMaterial.Diffuse.Contents = ColorForTextType (type, level);
				sideMaterial.Diffuse.Contents = NSColor.LightGray;
				textNode.Geometry.Materials = new SCNMaterial[] {
					frontMaterial,
					frontMaterial,
					sideMaterial,
					frontMaterial,
					frontMaterial
				};
			} else {
				// Full white emissive material (visible even when there is no light)
				textNode.Geometry.FirstMaterial = SCNMaterial.Create ();
				textNode.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Black;
				textNode.Geometry.FirstMaterial.Emission.Contents = ColorForTextType (type, level);

				// Don't write to the depth buffer because we don't want the text to be reflected
				textNode.Geometry.FirstMaterial.WritesToDepthBuffer = false;

				// Render last
				textNode.RenderingOrder = 1;
			}

			return textNode;
		}

		private SCNNode NodeWithCode (string code)
		{
			// Node hierarchy:
			// codeNode
			// |__ regularCodeNode
			// |__ emphasis-0 (can be highlighted separately)
			// |__ emphasis-1 (can be highlighted separately)
			// |__ emphasis-2 (can be highlighted separately)
			// |__ ...

			var codeNode = SCNNode.Create ();

			var chunk = 0;
			var regularCode = "";
			var whitespacesCode = "";

			// Automatically highlight the parts of the code that are delimited by '#'
			var components = code.Split ('#');

			for (int i = 0; i < components.Length; i++) {
				var component = components [i];

				var whitespaces = "";
				for (int j = 0; j < component.Length; j++) {
					string character = component.Substring (j, 1);
					if (character == "\n") {
						whitespaces = whitespaces + "\n";
					} else {
						whitespaces = whitespaces + " ";
					}
				}

				if ((i % 2) == 0) {
					var emphasisedCodeNode = NodeWithText (whitespacesCode + component, TextType.Code, 1);
					emphasisedCodeNode.Name = "emphasis-" + (chunk++);
					codeNode.AddChildNode (emphasisedCodeNode);

					regularCode = regularCode + whitespaces;
				} else
					regularCode = regularCode + component;

				whitespacesCode = whitespacesCode + whitespaces;
			}

			var regularCodeNode = NodeWithText (regularCode, TextType.Code, 0);
			regularCodeNode.Name = "regular";
			codeNode.AddChildNode (regularCodeNode);

			return codeNode;
		}

		private SCNNode AddText (string message, TextType type, int level)
		{
			var parentNode = TextContainerForType (type);

			CurrentBaseline -= LineHeightForTextType (type, level);

			if (type > TextType.Subtitle) {
				if (PreviousType <= TextType.Title)
					CurrentBaseline -= 1.0f;

				if (PreviousType <= TextType.Subtitle && type > TextType.Subtitle)
					CurrentBaseline -= 1.3f;
				else if (PreviousType != type)
					CurrentBaseline -= 1.0f;
			}

			var textNode = (type == TextType.Code) ? NodeWithCode (message) : NodeWithText (message, type, level);
			parentNode.AddChildNode (textNode);

			if (FadesIn) {
				textNode.Opacity = 0;
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				textNode.Opacity = 1;
				SCNTransaction.Commit ();
			}

			PreviousType = type;

			return textNode;
		}

		public SCNNode SetTitle (string title)
		{
			return AddText (title, TextType.Title, 0);
		}

		public SCNNode SetSubtitle (string title)
		{
			return AddText (title, TextType.Subtitle, 0);
		}

		public SCNNode SetChapterTitle (string title)
		{
			return AddText (title, TextType.Chapter, 0);
		}

		public SCNNode AddTextAtLevel (string text, int level)
		{
			return AddText (text, TextType.Body, level);
		}

		public SCNNode AddBulletAtLevel (string text, int level)
		{
			return AddText (text, TextType.Bullet, level);
		}

		public SCNNode AddCode (string text)
		{
			return AddText (text, TextType.Code, 0);
		}

		private const float PIVOT_X = 16;
		private const float FLIP_ANGLE = (float)(Math.PI / 2);
		private const float FLIP_DURATION = 1.0f;

		// Animate (fade out) to remove the text of specified type
		public void FadeOutText (TextType type)
		{
			var node = SubGroups [(int)type];
			SubGroups [(int)type] = null;
			if (node != null) {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = FLIP_DURATION;
				SCNTransaction.SetCompletionBlock (node.RemoveFromParentNode);
				node.Opacity = 0;
				SCNTransaction.Commit ();

				// Reset the baseline to what it was before adding this text
				CurrentBaseline = Math.Max (CurrentBaseline, BaselinePerType [(int)type]);
			}
		}

		// Animate (flip) to remove the text of specified type
		public void FlipOutText (TextType type)
		{
			var node = SubGroups [(int)type];
			SubGroups [(int)type] = null;
			if (node != null) {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				node.Position = new SCNVector3 (-PIVOT_X, 0, 0);
				node.Pivot = SCNMatrix4.CreateTranslation (-PIVOT_X, 0, 0);
				SCNTransaction.Commit ();

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = FLIP_DURATION;
				SCNTransaction.SetCompletionBlock (node.RemoveFromParentNode);
				node.Rotation = new SCNVector4 (0, 1, 0, FLIP_ANGLE);
				node.Opacity = 0;
				SCNTransaction.Commit ();
			}

			// Reset the baseline to what it was before adding this text
			CurrentBaseline = Math.Max (CurrentBaseline, BaselinePerType [(int)type]);
		}

		// Animate to reveal the text of specified type
		public void FlipInText (TextType type)
		{
			var node = SubGroups [(int)type];
			if (node != null) {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0;
				node.Position = new SCNVector3 (-PIVOT_X, 0, 0);
				node.Pivot = SCNMatrix4.CreateTranslation (-PIVOT_X, 0, 0);
				node.Rotation = new SCNVector4 (0, 1, 0, -FLIP_ANGLE);
				node.Opacity = 0;
				SCNTransaction.Commit ();

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = FLIP_DURATION;
				node.Rotation = new SCNVector4 (0, 1, 0, 0);
				node.Opacity = 1;
				SCNTransaction.Commit ();
			}
		}

		public void HighlightBullet (int index)
		{
			// Highlight is done by changing the emission color
			var node = SubGroups [(int)TextType.Bullet];
			if (node != null) {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;

				// Reset all
				foreach (SCNNode child in node.ChildNodes)
					child.Geometry.FirstMaterial.Emission.Contents = NSColor.White;

				// Unhighlight everything but index
				if (index > -1) {
					var i = 0;
					foreach (SCNNode child in node.ChildNodes) {
						if (i != index)
							child.Geometry.FirstMaterial.Emission.Contents = NSColor.DarkGray;
						i++;
					}
				}
				SCNTransaction.Commit ();
			}
		}

		public void HighlightCodeChunks (int[] chunks)
		{
			var node = SubGroups [(int)TextType.Code];

			// Unhighlight everything
			foreach (SCNNode child in node.ChildNodes[0]) {
				if (child.Geometry != null)
					child.Geometry.FirstMaterial.Emission.Contents = ColorForTextType (TextType.Code, 0);
			}

			// Highlight text inside range
			if (chunks != null) {
				foreach (int i in chunks) {
					SCNNode chunkNode = node.FindChildNode (new NSString ("emphasis-" + (i + 1)), true);
					chunkNode.Geometry.FirstMaterial.Emission.Contents = ColorForTextType (TextType.Code, 1);
				}
			}
		}
	}
}

