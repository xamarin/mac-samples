using System;
using SceneKit;
using AppKit;
using Foundation;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideActions : Slide
	{
		public override int NumberOfSteps ()
		{
			return 5;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Actions");
			TextManager.SetSubtitle ("SCNAction");

			TextManager.AddBulletAtLevel ("Easy to sequence, group and repeat", 0);
			TextManager.AddBulletAtLevel ("Limited to SCNNode", 0);
			TextManager.AddBulletAtLevel ("Same programming model as SpriteKit", 0);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			// Animate by default
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0;

			switch (index) {
			case 0:
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddEmptyLine ();
				TextManager.AddCode ("#// Rotate forever\n"
				+ "[aNode #runAction:#\n"
				+ "  [SCNAction repeatActionForever:\n"
				+ "  [SCNAction rotateByX:0 y:M_PI*2 z:0 duration:5.0]]];#");

				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			case 2:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Code);

				TextManager.AddBulletAtLevel ("Move", 0);
				TextManager.AddBulletAtLevel ("Rotate", 0);
				TextManager.AddBulletAtLevel ("Scale", 0);
				TextManager.AddBulletAtLevel ("Opacity", 0);
				TextManager.AddBulletAtLevel ("Remove", 0);
				TextManager.AddBulletAtLevel ("Wait", 0);
				TextManager.AddBulletAtLevel ("Custom block", 0);

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			case 3:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddEmptyLine ();
				TextManager.AddBulletAtLevel ("Directly targets the render tree", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			case 4:
				TextManager.AddBulletAtLevel ("node.position / node.presentationNode.position", 0);

				//labels
				var label1 = TextManager.AddTextAtLevel ("Action", 0);
				label1.Position = new SCNVector3 (-15, 3, 0);
				var label2 = TextManager.AddTextAtLevel ("Animation", 0);
				label2.Position = new SCNVector3 (-15, -2, 0);

				//animation
				var animNode = SCNNode.Create ();
				var cubeSize = 4.0f;
				animNode.Position = new SCNVector3 (-5, cubeSize / 2, 0);

				var cube = SCNBox.Create (cubeSize, cubeSize, cubeSize, 0.05f * cubeSize);

				cube.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/texture", "png"));
				cube.FirstMaterial.Diffuse.MipFilter = SCNFilterMode.Linear;
				cube.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
				cube.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;
				animNode.Geometry = cube;
				ContentNode.AddChildNode (animNode);

				SCNTransaction.Begin ();


				SCNNode animPosIndicator = null;
				SCNAnimationEvent startEvt = SCNAnimationEvent.Create (0, (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 0;
					animPosIndicator.Position = new SCNVector3 (10, animPosIndicator.Position.Y, animPosIndicator.Position.Z);
					SCNTransaction.Commit ();
				});
				SCNAnimationEvent endEvt = SCNAnimationEvent.Create (1, (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 0;
					animPosIndicator.Position = new SCNVector3 (-5, animPosIndicator.Position.Y, animPosIndicator.Position.Z);
					SCNTransaction.Commit ();
				});

				var anim = CABasicAnimation.FromKeyPath ("position.x");
				anim.Duration = 3;
				anim.From = new NSNumber (0.0);
				anim.To = new NSNumber (15.0);
				anim.Additive = true;
				anim.AutoReverses = true;
				anim.AnimationEvents = new SCNAnimationEvent[] { startEvt, endEvt };
				anim.RepeatCount = float.MaxValue;
				animNode.AddAnimation (anim, new NSString ("cubeAnimation"));

				//action
				var actionNode = SCNNode.Create ();
				actionNode.Position = new SCNVector3 (-5, cubeSize * 1.5f + 1, 0);
				actionNode.Geometry = cube;

				ContentNode.AddChildNode (actionNode);

				var mv = SCNAction.MoveBy (15, 0, 0, 3);

				actionNode.RunAction (SCNAction.RepeatActionForever (SCNAction.Sequence (new SCNAction[] {
					mv,
					mv.ReversedAction ()
				})));

				//position indicator
				var positionIndicator = SCNNode.Create ();
				positionIndicator.Geometry = SCNCylinder.Create (0.5f, 0.01f);
				positionIndicator.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Red;
				positionIndicator.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
				positionIndicator.EulerAngles = new SCNVector3 (NMath.PI / 2, 0, 0);
				positionIndicator.Position = new SCNVector3 (0, 0, cubeSize * 0.5f);
				actionNode.AddChildNode (positionIndicator);

				//anim pos indicator
				animPosIndicator = positionIndicator.Clone ();
				animPosIndicator.Position = new SCNVector3 (5, cubeSize / 2, cubeSize * 0.5f);
				ContentNode.AddChildNode (animPosIndicator);

				SCNTransaction.Commit ();

				break;
			}
			SCNTransaction.Commit ();
		}
	}
}

