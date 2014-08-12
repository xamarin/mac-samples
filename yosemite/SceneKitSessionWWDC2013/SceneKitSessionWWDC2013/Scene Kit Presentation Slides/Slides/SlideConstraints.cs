using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.SceneKit;
using MonoMac.Foundation;
using MonoMac.CoreAnimation;
using MonoMac.CoreFoundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideConstraints : Slide
	{
		private SCNNode BallNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 7;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Constraints");
				TextManager.SetSubtitle ("SCNConstraint");

				TextManager.AddBulletAtLevel ("Applied sequentially at render time", 0);
				TextManager.AddBulletAtLevel ("Only affect presentation values", 0);

				TextManager.AddCode ("#aNode.#Constraints# = new SCNConstraint[] { aConstraint, anotherConstraint, ... };#");

				// Tweak the near clipping plane of the spot light to get a precise shadow map
				presentationViewController.SpotLight.Light.SetAttribute (new NSNumber (10), SCNLightAttribute.ShadowNearClippingKey);
				break;
			case 1:
				// Remove previous text
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Code);

				// Add new text
				TextManager.SetSubtitle ("SCNLookAtConstraint");
				TextManager.AddBulletAtLevel ("Makes a node to look at another node", 0);
				TextManager.AddCode ("#nodeA.Constraints = new SCNConstraint[] { #SCNLookAtConstraint.Create# (nodeB) };#");

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			case 2:
				// Setup the scene
				SetupLookAtScene ();

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
					// Dim the text and move back a little bit
				TextManager.TextNode.Opacity = 0.5f;
				presentationViewController.CameraHandle.Position = presentationViewController.CameraNode.ConvertPositionToNode (new SCNVector3 (0, 0, 5.0f), presentationViewController.CameraHandle.ParentNode);
				SCNTransaction.Commit ();
				break;
			case 3:
				// Add constraints to the arrows
				var container = ContentNode.FindChildNode ("arrowContainer", true);

				// "Look at" constraint
				var constraint = SCNLookAtConstraint.Create (BallNode);

				var i = 0;
				foreach (var arrow in container.ChildNodes) {
					var delayInSeconds = 0.1 * i++;
					var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * Utils.NSEC_PER_SEC));
					DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
						SCNTransaction.Begin ();
						SCNTransaction.AnimationDuration = 1;
						// Animate to the result of applying the constraint
						((SCNNode)arrow.ChildNodes [0]).Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 2));
						arrow.Constraints = new SCNConstraint[] { constraint };
						SCNTransaction.Commit ();
					});
				}
				break;
			case 4:
				// Create a keyframe animation to move the ball
				var animation = CAKeyFrameAnimation.GetFromKeyPath ("position");
				animation.KeyTimes = new NSNumber[] {
					0.0f,
					(1.0f / 8.0f),
					(2.0f / 8.0f),
					(3.0f / 8.0f),
					(4.0f / 8.0f),
					(5.0f / 8.0f),
					(6.0f / 8.0f),
					(7.0f / 8.0f),
					1.0f
				};

				animation.Values = new NSObject[] { 
					NSValue.FromVector (new SCNVector3 (0, 0.0f, 0)),
					NSValue.FromVector (new SCNVector3 (20.0f, 0.0f, 20.0f)),
					NSValue.FromVector (new SCNVector3 (40.0f, 0.0f, 0)),
					NSValue.FromVector (new SCNVector3 (20.0f, 0.0f, -20.0f)),
					NSValue.FromVector (new SCNVector3 (0, 0.0f, 0)),
					NSValue.FromVector (new SCNVector3 (-20.0f, 0.0f, 20.0f)),
					NSValue.FromVector (new SCNVector3 (-40.0f, 0.0f, 0)),
					NSValue.FromVector (new SCNVector3 (-20.0f, 0.0f, -20.0f)),
					NSValue.FromVector (new SCNVector3 (0, 0.0f, 0))
				};

				animation.CalculationMode = CAKeyFrameAnimation.AnimationCubicPaced; // smooth the movement between keyframes
				animation.RepeatCount = float.MaxValue;
				animation.Duration = 10.0f;
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
				BallNode.AddAnimation (animation, new NSString ("ballNodeAnimation"));

				// Rotate the ball to give the illusion of a rolling ball
				// We need two animations to do that:
				// - one rotation to orient the ball in the right direction
				// - one rotation to spin the ball
				animation = CAKeyFrameAnimation.GetFromKeyPath ("rotation");
				animation.KeyTimes = new NSNumber[] { 
					0.0f, 
					(0.7f / 8.0f), 
					(1.0f / 8.0f), 
					(2.0f / 8.0f), 
					(3.0f / 8.0f), 
					(3.3f / 8.0f), 
					(4.7f / 8.0f),
					(5.0f / 8.0f),
					(6.0f / 8.0f),
					(7.0f / 8.0f),
					(7.3f / 8.0f),
					1.0f
				};

				animation.Values = new NSObject[] {
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI / 4))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI / 4))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI / 2))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI + Math.PI / 2))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2 - Math.PI / 4))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2 - Math.PI / 4))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI * 2 - Math.PI / 2))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI - Math.PI / 2))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI / 4))),
					NSValue.FromVector (new SCNVector4 (0, 1, 0, (float)(Math.PI / 4))) 
				};

				animation.RepeatCount = float.MaxValue;
				animation.Duration = 10.0f;
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.Linear);
				BallNode.AddAnimation (animation, new NSString ("ballNodeAnimation2"));

				var rotationAnimation = CABasicAnimation.FromKeyPath ("rotation");
				rotationAnimation.Duration = 1.0f;
				rotationAnimation.RepeatCount = float.MaxValue;
				rotationAnimation.To = NSValue.FromVector (new SCNVector4 (1, 0, 0, (float)(Math.PI * 2)));
				BallNode.ChildNodes [1].AddAnimation (rotationAnimation, new NSString ("ballNodeRotation"));
				break;
			case 5:
				// Add a constraint to the camera
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				constraint = SCNLookAtConstraint.Create (BallNode);
				presentationViewController.CameraNode.Constraints = new SCNConstraint[] { constraint };
				SCNTransaction.Commit ();
				break;
			case 6:
				// Add a constraint to the light
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				var cameraTarget = ContentNode.FindChildNode ("cameraTarget", true);
				constraint = SCNLookAtConstraint.Create (cameraTarget);
				presentationViewController.SpotLight.Constraints = new SCNConstraint[] { constraint };
				SCNTransaction.Commit ();
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			presentationViewController.CameraNode.Constraints = null;
			presentationViewController.SpotLight.Constraints = null;
			SCNTransaction.Commit ();
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			presentationViewController.EnableShadows (true);
		}

		private void SetupLookAtScene ()
		{
			var intermediateNode = SCNNode.Create ();
			intermediateNode.Scale = new SCNVector3 (0.5f, 0.5f, 0.5f);
			intermediateNode.Position = new SCNVector3 (0, 0, 10);
			ContentNode.AddChildNode (intermediateNode);

			var ballMaterial = SCNMaterial.Create ();
			ballMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes/pool/pool_8", "png"));
			ballMaterial.Specular.Contents = NSColor.White;
			ballMaterial.Shininess = 0.9f; // shinny
			ballMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/color_envmap", "png"));
			ballMaterial.Reflective.Intensity = 0.5f;

			// Node hierarchy for the ball :
			//   _ballNode
			//  |__ cameraTarget      : the target for the "look at" constraint
			//  |__ ballRotationNode  : will rotate to animate the rolling ball
			//      |__ ballPivotNode : will own the geometry and will be rotated so that the "8" faces the camera at the beginning

			BallNode = SCNNode.Create ();
			BallNode.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 4));
			intermediateNode.AddChildNode (BallNode);

			var cameraTarget = SCNNode.Create ();
			cameraTarget.Name = "cameraTarget";
			cameraTarget.Position = new SCNVector3 (0, 6, 0);
			BallNode.AddChildNode (cameraTarget);

			var ballRotationNode = SCNNode.Create ();
			ballRotationNode.Position = new SCNVector3 (0, 4, 0);
			BallNode.AddChildNode (ballRotationNode);

			var ballPivotNode = SCNNode.Create ();
			ballPivotNode.Geometry = SCNSphere.Create (4.0f);
			ballPivotNode.Geometry.FirstMaterial = ballMaterial;
			ballPivotNode.Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 2));
			ballRotationNode.AddChildNode (ballPivotNode);

			var arrowMaterial = SCNMaterial.Create ();
			arrowMaterial.Diffuse.Contents = NSColor.White;
			arrowMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/chrome", "jpg"));

			var arrowContainer = SCNNode.Create ();
			arrowContainer.Name = "arrowContainer";
			intermediateNode.AddChildNode (arrowContainer);

			var arrowPath = Utils.SCArrowBezierPath (new SizeF (6, 2), new SizeF (3, 5), 0.5f, false);
			// Create the arrows
			for (var i = 0; i < 11; i++) {
				var arrowNode = SCNNode.Create ();
				arrowNode.Position = new SCNVector3 ((float)Math.Cos (Math.PI * i / 10.0f) * 20.0f, 3 + 18.5f * (float)Math.Sin (Math.PI * i / 10.0f), 0);

				var arrowGeometry = SCNShape.Create (arrowPath, 1);
				arrowGeometry.ChamferRadius = 0.2f;

				var arrowSubNode = SCNNode.Create ();
				arrowSubNode.Geometry = arrowGeometry;
				arrowSubNode.Geometry.FirstMaterial = arrowMaterial;
				arrowSubNode.Pivot = SCNMatrix4.CreateTranslation (new SCNVector3 (0, 2.5f, 0)); // place the pivot (center of rotation) at the middle of the arrow
				arrowSubNode.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI / 2));

				arrowNode.AddChildNode (arrowSubNode);
				arrowContainer.AddChildNode (arrowNode);
			}
		}
	}
}

