using System;
using AppKit;
using SceneKit;
using Foundation;
using ObjCRuntime;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
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
			var heroNode = Utils.SCAddChildNode (GroundNode, "heroGroup", "Scenes/hero/hero", 0.0f);

			heroNode.Scale = new SCNVector3 (0.023f, 0.023f, 0.023f);
			heroNode.Position = new SCNVector3 (0.0f, 0.0f, 15.0f);
			heroNode.Rotation = new SCNVector4 (1.0f, 0.0f, 0.0f, -(float)(Math.PI / 2));

			GroundNode.AddChildNode (heroNode);

			// Convert sceneTime-based animations into systemTime-based animations.
			// Animations loaded from DAE files will play according to the `currentTime` property of the scene renderer if this one is playing
			// (see the SCNSceneRenderer protocol). Here we don't play a specific DAE so we want the animations to animate as soon as we add
			// them to the scene (i.e have them to play according the time of the system when the animation was added).

			HeroSkeletonNode = heroNode.FindChildNode ("skeleton", true);

			foreach (var animationKey in HeroSkeletonNode.GetAnimationKeys ()) {
				// Find all the animations. Make them system time based and repeat forever.
				// And finally replace the old animation.

				var animation = HeroSkeletonNode.GetAnimation (animationKey);
				animation.UsesSceneTimeBase = false;
				animation.RepeatCount = float.MaxValue;

				HeroSkeletonNode.AddAnimation (animation, animationKey);
			}

			// Load other animations so that we will use them later
			SetAnimation (CharacterAnimation.Attack, "attackID", "attack");
			SetAnimation (CharacterAnimation.Die, "DeathID", "death");
			SetAnimation (CharacterAnimation.Walk, "WalkID", "walk");
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				TextManager.SetTitle ("Animation Events");
				TextManager.AddBulletAtLevel ("SCNAnimationEvent", 0);

				TextManager.AddCode ("#var anEvent = #SCNAnimationEvent.Create# (0.2,  aBlock); \n"
				+ "anAnimation.#AnimationEvents# = @[anEvent, anotherEvent];#");

				// Warm up NSSound by playing an empty sound.
				// Otherwise the first sound may take some time to start playing and will be desynchronised.
				var path = NSBundle.MainBundle.PathForResource ("Sounds/emptySound", "m4a");
				var soundUrl = NSUrl.FromFilename (path);
				var emptySound = new NSSound (soundUrl, false);
				emptySound.Play ();
				break;
			case 1:
			case 2:
				// Trigger the attack animation
				HeroSkeletonNode.AddAnimation (Animations [(int)CharacterAnimation.Attack], new NSString ("attack"));
				break;
			case 3:
				// Trigger the walk animation
				HeroSkeletonNode.AddAnimation (Animations [(int)CharacterAnimation.Walk], new NSString ("walk"));
				break;
			case 4:
				// Trigger the death animation
				// Make sure to remove the "idle" animation and prevent the model from intersecting with the floor.

				HeroSkeletonNode.RemoveAllAnimations ();
				HeroSkeletonNode.AddAnimation (Animations [(int)CharacterAnimation.Die], new NSString ("death"));

				SCNTransaction.Begin ();
				//TODO heroSkeletonNode.ParentNode.Transform = SCNMatrix4.CreateTranslation (new SCNVector3 (0, 0, 40)); //CATransform3DTranslate(_heroSkeletonNode.parentNode.transform, 0, 0, 40);
				SCNTransaction.Commit ();
				break;
			}
		}

		private void SetAnimation (CharacterAnimation index, string animationName, string sceneName)
		{
			// Load the DAE using SCNSceneSource in order to be able to retrieve the animation by its identifier
			var path = NSBundle.MainBundle.PathForResource ("Scenes/hero/" + sceneName, "dae");
			var sceneURL = NSUrl.FromFilename (path);
			var sceneSource = SCNSceneSource.FromUrl (sceneURL, (NSDictionary)null);

			var animation = (CAAnimation)sceneSource.GetEntryWithIdentifier (animationName, new Class ("CAAnimation"));
			Animations [(int)index] = animation;

			// Blend animations for smoother transitions
			animation.FadeInDuration = 0.3f;
			animation.FadeOutDuration = 0.3f;

			if (index == CharacterAnimation.Die) {
				// We want the "death" animation to remain at its final state at the end of the animation
				animation.RemovedOnCompletion = false;
				animation.FillMode = CAFillMode.Both;

				// Create animation events and set them to the animation
				var swipeSoundEventHandler = new SCNAnimationEventHandler ((CAAnimation handlerAnimation, NSObject animatedObject, bool playingBackward) => {
					InvokeOnMainThread (delegate {
						var soundUrl = NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("Sounds/swipe", "wav"));
						new NSSound (soundUrl, false).Play ();
					});
				});

				var deathSoundEventBlock = new SCNAnimationEventHandler ((CAAnimation handlerAnimation, NSObject animatedObject, bool playingBackward) => {
					InvokeOnMainThread (delegate {
						var soundUrl = NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("Sounds/death", "wav"));
						new NSSound (soundUrl, false).Play ();
					});
				});

				animation.AnimationEvents = new SCNAnimationEvent[] {
					SCNAnimationEvent.Create (0.0f, swipeSoundEventHandler),
					SCNAnimationEvent.Create (0.3f, deathSoundEventBlock)
				};
			}

			if (index == CharacterAnimation.Attack) {
				// Create an animation event and set it to the animation
				var swordSoundEventHandler = new SCNAnimationEventHandler ((CAAnimation handlerAnimation, NSObject animatedObject, bool playingBackward) => {
					InvokeOnMainThread (delegate {
						var soundUrl = NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("Sounds/sword", "wav"));
						var attackSound = new NSSound (soundUrl, false);
						attackSound.Play ();
					});
				});

				animation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.4f, swordSoundEventHandler) };
			}

			if (index == CharacterAnimation.Walk) {
				// Repeat the walk animation 3 times
				animation.RepeatCount = 3;

				// Create an animation event and set it to the animation
				var stepSoundEventHandler = new SCNAnimationEventHandler ((CAAnimation handlerAnimation, NSObject animatedObject, bool playingBackward) => {
					InvokeOnMainThread (delegate {
						var soundUrl = NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("Sounds/walk", "wav"));
						new NSSound (soundUrl, false).Play ();
					});
				});

				animation.AnimationEvents = new SCNAnimationEvent[] {
					SCNAnimationEvent.Create (0.2f, stepSoundEventHandler),
					SCNAnimationEvent.Create (0.7f, stepSoundEventHandler)
				};
			}
		}
	}
}

