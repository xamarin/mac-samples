using System;

using AppKit;
using SceneKit;
using Foundation;
using ObjCRuntime;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideIK : Slide, ISCNSceneRendererDelegate
	{
		private SCNIKConstraint Ik { get; set; }

		private SCNLookAtConstraint LookAt { get; set; }

		private CAAnimation Attack { get; set; }

		private SCNNode Hero { get; set; }

		private SCNNode Target { get; set; }

		private bool IkActive { get; set; }

		private double AnimationStartTime { get; set; }

		private double AnimationDuration { get; set; }

		public override int NumberOfSteps ()
		{
			return 9;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Constraints");
			TextManager.SetSubtitle ("SCNIKConstraint");

			TextManager.AddBulletAtLevel ("Inverse Kinematics", 0);
			TextManager.AddBulletAtLevel ("Node chain", 0);
			TextManager.AddBulletAtLevel ("Target", 0);

			//load the hero
			Hero = Utils.SCAddChildNode (GroundNode, "heroGroup", "Scenes.scnassets/hero/hero", 12);
			Hero.Position = new SCNVector3 (0, 0, 5);
			Hero.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2);

			//hide the sword
			var sword = Hero.FindChildNode ("sword", true);
			sword.Hidden = true;

			//load attack animation
			var path = NSBundle.MainBundle.PathForResource ("Scenes.scnassets/hero/attack", "dae");
			var source = SCNSceneSource.FromUrl (NSUrl.FromFilename (path), (NSDictionary)null);
			Attack = (CAAnimation)source.GetEntryWithIdentifier ("attackID", new Class ("CAAnimation"));
			Attack.RepeatCount = 0;
			Attack.FadeInDuration = 0.1f;
			Attack.FadeOutDuration = 0.3f;
			Attack.Speed = 0.75f;

			Attack.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.55f, (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
					if (IkActive)
						DestroyTarget ();
				})
			};

			AnimationDuration = Attack.Duration;

			//setup IK
			var hand = Hero.FindChildNode ("Bip01_R_Hand", true);
			var clavicle = Hero.FindChildNode ("Bip01_R_Clavicle", true);
			var head = Hero.FindChildNode ("Bip01_Head", true);

			Ik = SCNIKConstraint.Create (clavicle);
			hand.Constraints = new SCNConstraint[] { Ik };
			Ik.InfluenceFactor = 0.0f;

			//add target
			Target = SCNNode.Create ();
			Target.Position = new SCNVector3 (-4, 7, 10);
			Target.Opacity = 0;
			Target.Geometry = SCNPlane.Create (2, 2);
			Target.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Images/target", "png"));
			GroundNode.AddChildNode (Target);

			//look at
			LookAt = SCNLookAtConstraint.Create (Target);
			LookAt.InfluenceFactor = 0;
			head.Constraints = new SCNConstraint[] { LookAt };

			((SCNView)presentationViewController.View).WeakSceneRendererDelegate = this;
		}

		//[Export ("renderer:didApplyAnimationsAtTime:")]
		public void DidApplyAnimations (SCNSceneRenderer renderer, double timeInSeconds)
		{
			if (IkActive) {
				// update the influence factor of the IK constraint based on the animation progress
				var currProgress = (float)(Attack.Speed * (timeInSeconds - AnimationStartTime) / AnimationDuration);

				//clamp
				currProgress = (float)Math.Max (0, currProgress);
				currProgress = (float)Math.Min (1, currProgress);

				if (currProgress >= 1) {
					IkActive = false;
				}

				float middle = 0.5f;
				float f;

				// smoothly increate from 0% to 50% then smoothly decrease from 50% to 100%
				if (currProgress > middle) {
					f = (1.0f - currProgress) / (1.0f - middle);
				} else {
					f = currProgress / middle;
				}

				Ik.InfluenceFactor = f;
				LookAt.InfluenceFactor = 1 - f;
			}
		}

		private void MoveTarget (int step)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			switch (step) {
			case 0:
				Ik.TargetPosition = GroundNode.ConvertPositionToNode (new SCNVector3 (-70, 2, 50), null);
				break;
			case 1:
				Target.Position = new SCNVector3 (-1, 4, 10);
				Ik.TargetPosition = GroundNode.ConvertPositionToNode (new SCNVector3 (-30, -50, 50), null);
				break;
			case 2:
				Target.Position = new SCNVector3 (-5, 5, 10);
				Ik.TargetPosition = GroundNode.ConvertPositionToNode (new SCNVector3 (-70, 2, 50), null);
				break;
			}
			Target.Opacity = 1;
			SCNTransaction.Commit ();
		}

		private void DestroyTarget ()
		{
			Target.Opacity = 0;
			var ps = SCNParticleSystem.Create ("explosion", "Particles");
			Target.AddParticleSystem (ps);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				break;
			case 1: //punch
				Hero.AddAnimation (Attack, new NSString ("attack"));
				AnimationStartTime = CAAnimation.CurrentMediaTime ();
				break;
			case 2://add target
				MoveTarget (0);
				break;
			case 3://punch
				Hero.AddAnimation (Attack, new NSString ("attack"));
				AnimationStartTime = CAAnimation.CurrentMediaTime ();
				break;
			case 4://punch + IK
				IkActive = true;
				LookAt.InfluenceFactor = 1;
				Hero.AddAnimation (Attack, new NSString ("attack"));
				AnimationStartTime = CAAnimation.CurrentMediaTime ();
				break;
			case 5://punch
				MoveTarget (1);
				break;
			case 6://punch
				IkActive = true;
				Hero.AddAnimation (Attack, new NSString ("attack"));
				AnimationStartTime = CAAnimation.CurrentMediaTime ();
				break;
			case 7://punch
				MoveTarget (2);
				break;
			case 8://punch
				IkActive = true;
				Hero.AddAnimation (Attack, new NSString ("attack"));
				AnimationStartTime = CAAnimation.CurrentMediaTime ();
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
		}
	}
}

