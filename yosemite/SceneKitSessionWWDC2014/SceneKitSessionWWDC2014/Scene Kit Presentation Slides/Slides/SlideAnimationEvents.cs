using System;
using AppKit;
using SceneKit;
using Foundation;
using ObjCRuntime;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideAnimationEvents : Slide
	{
		public enum CharacterAnimation
		{
			Attack = 0,
			Walk,
			Die,
			Count
		}

		private SCNNode HeroSkeletonNode { get; set; }

		private CAAnimation[] Animations = new CAAnimation[(int)CharacterAnimation.Count];

		public override int NumberOfSteps ()
		{
			return 5;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Load the character and add it to the scene
			var heroNode = Utils.SCAddChildNode (GroundNode, "bossGroup", "Scenes.scnassets/boss/boss", 0.0f);

			heroNode.Scale = new SCNVector3 (0.015f, 0.015f, 0.015f);
			heroNode.Position = new SCNVector3 (3.0f, 0.0f, 15.0f);
			heroNode.Rotation = new SCNVector4 (1.0f, 0.0f, 0.0f, -(float)(Math.PI / 2));

			GroundNode.AddChildNode (heroNode);

			// Convert sceneTime-based animations into systemTime-based animations.
			// Animations loaded from DAE files will play according to the `currentTime` property of the scene renderer if this one is playing
			// (see the SCNSceneRenderer protocol). Here we don't play a specific DAE so we want the animations to animate as soon as we add
			// them to the scene (i.e have them to play according the time of the system when the animation was added).

			HeroSkeletonNode = heroNode.FindChildNode ("skeleton", true);

			// Load other animations so that we will use them later
			SetAnimation (CharacterAnimation.Attack, "attackID", "boss_attack");
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				TextManager.SetTitle ("Animation Events");
				TextManager.AddBulletAtLevel ("SCNAnimationEvent", 0);
				TextManager.AddBulletAtLevel ("Smooth transitions", 0);

				TextManager.AddCode ("#var anEvent = #SCNAnimationEvent.Create# (0.2,  aBlock); \n"
				+ "anAnimation.#AnimationEvents# = @[anEvent, anotherEvent];#");

				var path = NSBundle.MainBundle.PathForResource ("Sounds/bossaggro", "wav");
				var soundUrl = NSUrl.FromFilename (path);
				var bossaggro = new NSSound (soundUrl, false);
				bossaggro.Play ();
				break;
			case 1:
				// Trigger the attack animation
				HeroSkeletonNode.AddAnimation (Animations [(int)CharacterAnimation.Attack], new NSString ("attack"));
				break;
			case 2:
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.AddCode ("#\n"
				+ "\n"
				+ "\n\n"
				+ "anAnimation.FadeInDuration = #0.0#;\n"
				+ "anAnimation.FadeOutDuration = #0.0#;#");
				break;
			case 3:
			case 4:
				Animations[(int)CharacterAnimation.Attack].FadeInDuration = 0;
				Animations[(int)CharacterAnimation.Attack].FadeOutDuration = 0;
				// Trigger the attack animation
				HeroSkeletonNode.AddAnimation (Animations [(int)CharacterAnimation.Attack], new NSString ("attack"));
				break;
			case 5:
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.AddCode ("#\n"
					+ "\n"
					+ "\n\n"
					+ "anAnimation.FadeInDuration = #0.3#;\n"
					+ "anAnimation.FadeOutDuration = #0.3#;#");
					break;
			case 6:
			case 7:
				Animations[(int)CharacterAnimation.Attack].FadeInDuration = 0.3f;
				Animations[(int)CharacterAnimation.Attack].FadeOutDuration = 0.3f;
				// Trigger the attack animation
				HeroSkeletonNode.AddAnimation (Animations [(int)CharacterAnimation.Attack], new NSString ("attack"));
				break;
			}
		}

		private void SetAnimation (CharacterAnimation index, string animationName, string sceneName)
		{
			// Load the DAE using SCNSceneSource in order to be able to retrieve the animation by its identifier
			var sceneURL = NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/boss/" + sceneName, "dae"));
			var sceneSource = SCNSceneSource.FromUrl (sceneURL, (NSDictionary)null);

			var bossAnimation = (CAAnimation)sceneSource.GetEntryWithIdentifier (animationName, new Class ("CAAnimation"));
			Animations [(int)index] = bossAnimation;

			// Blend animations for smoother transitions
			bossAnimation.FadeInDuration = 0.3f;
			bossAnimation.FadeOutDuration = 0.3f;

			if (index == CharacterAnimation.Attack) {
				// Create an animation event and set it to the animation
				var attackSoundEvent = new SCNAnimationEventHandler ((CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
					InvokeOnMainThread (delegate {
						var soundUrl = NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("Sounds/attack4", "wav"));
						new NSSound (soundUrl, false).Play ();
					});
				});
				bossAnimation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.4f, attackSoundEvent) };
			}
		}
	}
}

