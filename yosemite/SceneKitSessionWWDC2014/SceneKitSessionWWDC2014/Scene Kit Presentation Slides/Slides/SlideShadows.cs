using System;
using AppKit;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideShadows : Slide
	{
		private SCNNode PalmTree { get; set; }

		private SCNNode Character { get; set; }

		private SCNNode LightHandle { get; set; }

		private SCNNode Projector { get; set; }

		private SCNNode StaticShadowNode { get; set; }

		private SCNVector3 OldSpotPosition { get; set; }

		private SCNNode OldSpotParent { get; set; }

		private nfloat OldSpotZNear { get; set; }

		private NSColor OldSpotShadowColor { get; set; }

		private const float DIST = 0.3f;

		public override int NumberOfSteps ()
		{
			return 6;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Shadows");

			TextManager.AddBulletAtLevel ("Static", 0);
			TextManager.AddBulletAtLevel ("Dynamic", 0);
			TextManager.AddBulletAtLevel ("Projected", 0);

			var sceneryHolder = SCNNode.Create ();
			sceneryHolder.Name = "scenery";
			sceneryHolder.Position = new SCNVector3 (5, -19, 12);

			GroundNode.AddChildNode (sceneryHolder);

			//add scenery
			var scenery = Utils.SCAddChildNode (sceneryHolder, "scenery", "Scenes.scnassets/banana/level", 130);
			scenery.Position = new SCNVector3 (-291.374969f, 1.065581f, -30.519293f);
			scenery.Scale = new SCNVector3 (0.044634f, 0.044634f, 0.044634f);
			scenery.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2);

			PalmTree = Utils.SCAddChildNode (GroundNode, "PalmTree", "Scenes.scnassets/palmTree/palm_tree", 15);

			PalmTree.Position = new SCNVector3 (3, -1, 7);
			PalmTree.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2);

			foreach (var child in PalmTree.ChildNodes)
				child.CastsShadow = false;

			//add a static shadow
			var shadowPlane = SCNNode.FromGeometry (SCNPlane.Create (15, 15));
			shadowPlane.EulerAngles = new SCNVector3 (-(float)Math.PI / 2, (float)(Math.PI / 4) * 0.5f, 0);
			shadowPlane.Position = new SCNVector3 (0.5f, 0.1f, 2);
			shadowPlane.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Images/staticShadow", "tiff"));
			GroundNode.AddChildNode (shadowPlane);
			StaticShadowNode = shadowPlane;
			StaticShadowNode.Opacity = 0;

			var character = Utils.SCAddChildNode (GroundNode, "explorer", "Scenes.scnassets/explorer/explorer_skinned", 9);

			var animScene = SCNScene.FromFile ("Scenes.scnassets/explorer/idle");
			var animatedNode = animScene.RootNode.FindChildNode ("Bip001_Pelvis", true);
			character.AddAnimation (animatedNode.GetAnimation (animatedNode.GetAnimationKeys () [0]), new NSString ("idle"));

			character.EulerAngles = new SCNVector3 (0, (nfloat)Math.PI / 2, (nfloat)Math.PI / 2);
			character.Position = new SCNVector3 (20, 0, 7);
			Character = character;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				break;
			case 1:
				TextManager.HighlightBullet (0);

				StaticShadowNode.Opacity = 1;

				var node = TextManager.AddCode ("#aMaterial.#multiply#.contents = aShadowMap;#");
				node.Position = new SCNVector3 (node.Position.X, node.Position.Y - 4, node.Position.Z);
				foreach (var child in node.ChildNodes) {
					child.RenderingOrder = 1;
					foreach (var m in child.Geometry.Materials)
						m.ReadsFromDepthBuffer = false;
				}
				break;
			case 2:
				//move the tree
				PalmTree.RunAction (SCNAction.RotateBy (0, (nfloat)Math.PI * 4, 0, 8));
				break;
			case 3:
				TextManager.FadesIn = true;
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.AddEmptyLine ();

				node = TextManager.AddCode ("#aLight.#castsShadow# = YES;#");
				foreach (SCNNode child in node.ChildNodes) {
					child.RenderingOrder = 1;
					foreach (SCNMaterial m in child.Geometry.Materials) {
						m.ReadsFromDepthBuffer = false;
						m.WritesToDepthBuffer = false;
					}
				}

				node.Position = new SCNVector3 (node.Position.X, node.Position.Y - 6, node.Position.Z);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;

				var spot = presentationViewController.SpotLight;
				OldSpotShadowColor = spot.Light.ShadowColor;
				spot.Light.ShadowColor = NSColor.Black;
				spot.Light.ShadowRadius = 3;

				var tp = TextManager.TextNode.Position;

				var superNode = presentationViewController.CameraNode.ParentNode.ParentNode;

				var p0 = GroundNode.ConvertPositionToNode (SCNVector3.Zero, null);
				var p1 = GroundNode.ConvertPositionToNode (new SCNVector3 (20, 0, 0), null);
				var tr = new SCNVector3 (p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);

				var p = superNode.Position;
				p.X += tr.X;
				p.Y += tr.Y;
				p.Z += tr.Z;
				tp.X += 20;
				tp.Y += 0;
				tp.Z += 0;
				superNode.Position = p;
				TextManager.TextNode.Position = tp;
				SCNTransaction.Commit ();

				TextManager.HighlightBullet (1);

				break;
			case 4:
				//move the light
				var lightPivot = SCNNode.Create ();
				lightPivot.Position = Character.Position;
				GroundNode.AddChildNode (lightPivot);

				spot = presentationViewController.SpotLight;
				OldSpotPosition = spot.Position;
				OldSpotParent = spot.ParentNode;
				OldSpotZNear = spot.Light.ZNear;

				spot.Light.ZNear = 20;
				spot.Position = lightPivot.ConvertPositionFromNode (spot.Position, spot.ParentNode);
				lightPivot.AddChildNode (spot);

				//add an object to represent the light
				var lightModel = SCNNode.Create ();
				var lightHandle = SCNNode.Create ();
				var cone = SCNCone.Create (0, 0.5f, 1);
				cone.RadialSegmentCount = 10;
				cone.HeightSegmentCount = 5;
				lightModel.Geometry = cone;
				lightModel.Geometry.FirstMaterial.Emission.Contents = NSColor.Yellow;
				lightHandle.Position = new SCNVector3 (spot.Position.X * DIST, spot.Position.Y * DIST, spot.Position.Z * DIST);
				lightModel.CastsShadow = false;
				lightModel.EulerAngles = new SCNVector3 ((nfloat)Math.PI / 2, 0, 0);
				lightHandle.AddChildNode (lightModel);
				lightHandle.Constraints = new SCNConstraint[] { SCNLookAtConstraint.Create (Character) };
				lightPivot.AddChildNode (lightHandle);
				LightHandle = lightHandle;

				var animation = CABasicAnimation.FromKeyPath ("eulerAngles.z");
				animation.From = new NSNumber ((nfloat)(Math.PI / 4) * 1.7f);
				animation.To = new NSNumber ((nfloat)(-Math.PI / 4) * 0.3f);
				animation.Duration = 4;
				animation.AutoReverses = true;
				animation.RepeatCount = float.MaxValue;
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				animation.TimeOffset = animation.Duration / 2;
				lightPivot.AddAnimation (animation, new NSString ("lightAnim"));
				break;
			case 5:
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				var text = TextManager.AddCode ("#aLight.#shadowMode# =\n#SCNShadowModeModulated#;\naLight.#gobo# = anImage;#");
				text.Position = new SCNVector3 (text.Position.X, text.Position.Y - 6, text.Position.Z);
				text.EnumerateChildNodes ((SCNNode child, out bool stop) => {
					stop = false;
					child.RenderingOrder = 1;
					foreach (var m in child.Geometry.Materials) {
						m.ReadsFromDepthBuffer = false;
						m.WritesToDepthBuffer = false;
					}
					return stop;
				});

				LightHandle.RemoveFromParentNode ();

				RestoreSpotPosition (presentationViewController);
				TextManager.HighlightBullet (2);


				spot = presentationViewController.SpotLight;
				spot.Light.CastsShadow = false;

				var head = Character.FindChildNode ("Bip001_Pelvis", true);

				node = SCNNode.Create ();
				node.Light = SCNLight.Create ();
				node.Light.LightType = SCNLightType.Spot;
				node.Light.SpotOuterAngle = 30;
				node.Constraints = new SCNConstraint[] { SCNLookAtConstraint.Create (head) };
				node.Position = new SCNVector3 (0, 220, 0);
				node.Light.ZNear = 10;
				node.Light.ZFar = 1000;
				node.Light.Gobo.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/blobShadow", "jpg"));
				node.Light.Gobo.Intensity = 0.65f;
				node.Light.ShadowMode = SCNShadowMode.Modulated;

				//exclude character from shadow
				node.Light.CategoryBitMask = 0x1;
				Character.FindNodes ((SCNNode child, out bool stop) => {
					stop = false;
					child.CategoryBitMask = 0x2;
					return stop;
				});

				Projector = node;
				Character.AddChildNode (node);

				break;
			}
		}

		private void RestoreSpotPosition (PresentationViewController presentationViewController)
		{
			var spot = presentationViewController.SpotLight;
			spot.Light.CastsShadow = true;
			OldSpotParent.AddChildNode (spot);
			spot.Position = OldSpotPosition;
			spot.Light.ZNear = OldSpotZNear;
			spot.Light.ShadowColor = OldSpotShadowColor;
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			if (Projector != null)
				Projector.RemoveFromParentNode ();

			if (OldSpotParent != null)
				RestoreSpotPosition (presentationViewController);

			if (OldSpotShadowColor != null) {
				var spot = presentationViewController.SpotLight;
				spot.Light.ShadowColor = OldSpotShadowColor;
			}
		}
	}
}

