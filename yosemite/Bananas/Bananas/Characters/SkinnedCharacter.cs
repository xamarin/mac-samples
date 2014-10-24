using System;
using SceneKit;
using CoreAnimation;
using Foundation;
using ObjCRuntime;
using System.Collections.Generic;

namespace Bananas
{
	public class SkinnedCharacter : SCNNode
	{
		protected SCNNode mainSkeleton;
		Dictionary<string, CAAnimation> animationsDict;

		public SkinnedCharacter (SCNNode characterRootNode)
		{
			characterRootNode.Position = SCNVector3.Zero;
			AddChildNode (characterRootNode);

			foreach (var child in ChildNodes[0].ChildNodes) {
				if (child.Skinner != null) {
					mainSkeleton = child.Skinner.Skeleton;
					break;
				}

				foreach (var childOfChild in child.ChildNodes) {
					if (childOfChild.Skinner != null) {
						mainSkeleton = childOfChild.Skinner.Skeleton;
						break;
					}
				}
			}

		}

		public static CAAnimation LoadAnimationNamed (string animationName, string sceneName)
		{
			NSUrl url = NSBundle.MainBundle.GetUrlForResource (sceneName, "dae");
			var options = new SCNSceneLoadingOptions () {
				ConvertToYUp = true
			};

			var sceneSource = new SCNSceneSource (url, options);
			var animation = (CAAnimation)sceneSource.GetEntryWithIdentifier (animationName, new Class (typeof(CAAnimation)));

			return animation;
		}

		protected CAAnimation CachedAnimationForKey (string key)
		{
			return animationsDict [key];
		}

		public CAAnimation LoadAndCacheAnimation (string daeFile, string name, string key)
		{
			animationsDict = animationsDict ?? new Dictionary<string, CAAnimation> ();

			CAAnimation anim = SkinnedCharacter.LoadAnimationNamed (name, daeFile);

			if (anim != null) {
				animationsDict.Add (key, anim);
				anim.WeakDelegate = this;
			}

			return anim;
		}

		public CAAnimation LoadAndCacheAnimation (string daeFile, string key)
		{
			return LoadAndCacheAnimation (daeFile, key, key);
		}

		public void ChainAnimation (string firstKey, string secondKey)
		{
			ChainAnimation (firstKey, secondKey, 0.85f);
		}

		public void ChainAnimation (string firstKey, string secondKey, float fadeTime)
		{
			CAAnimation firstAnim = animationsDict [firstKey];
			CAAnimation secondAnim = animationsDict [secondKey];

			if (firstAnim == null || secondAnim == null)
				return;

			SCNAnimationEventHandler chainEventBlock = (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
				mainSkeleton.AddAnimation (secondAnim, secondKey);
			};

			if (firstAnim.AnimationEvents == null || firstAnim.AnimationEvents.Length == 0) {
				firstAnim.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (fadeTime, chainEventBlock) };
			} else {
				var pastEvents = new List<SCNAnimationEvent> (firstAnim.AnimationEvents);
				pastEvents.Add (SCNAnimationEvent.Create (fadeTime, chainEventBlock));
				firstAnim.AnimationEvents = pastEvents.ToArray ();
			}
		}

		public virtual void Update (double deltaTime)
		{
		}
	}
}

