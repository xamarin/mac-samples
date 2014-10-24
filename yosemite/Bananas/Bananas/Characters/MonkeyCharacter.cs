using System;
using CoreAnimation;
using Foundation;
using SceneKit;

namespace Bananas
{
	public class MonkeyCharacter : SkinnedCharacter
	{
		bool isIdle;
		bool hasCoconut;

		SCNNode rightHand;
		SCNNode coconutInHand;

		public MonkeyCharacter (SCNNode monkeyNode) : base (monkeyNode)
		{
		}

		public void CreateAnimations ()
		{
			Name = "monkey";
			rightHand = FindChildNode ("Bone_R_Hand", true);
			isIdle = true;
			hasCoconut = false;

			SetupTauntAnimation ();
			SetupHangAnimation ();
			SetupGetCoconutAnimation ();
			SetupThrowAnimation ();

			ChainAnimation ("monkey_get_coconut-1", "monkey_throw_coconut-1");
			mainSkeleton.AddAnimation (CachedAnimationForKey ("monkey_tree_hang-1"), "monkey_idle");
		}

		public override void Update (double deltaTime)
		{
			PlayerCharacter playerCharacter = GameSimulation.Sim.GameLevel.PlayerCharacter;
			SCNVector3 pos = ParentNode.Position;
			nfloat distanceToCharacter = SCNVector3.Subtract (playerCharacter.Position, pos).Length;

			if (distanceToCharacter >= 1000f)
				return;

			if (!isIdle)
				return;

			if (playerCharacter.IsWalking) {
				mainSkeleton.AddAnimation (CachedAnimationForKey ("monkey_get_coconut-1"), null);
				isIdle = false;
			} else if (MathUtils.RandomPercent () <= 0.001f) {
				isIdle = false;
				mainSkeleton.AddAnimation (CachedAnimationForKey ("monkey_tree_hang_taunt-1"), null);
			}
		}

		void SetupTauntAnimation ()
		{
			string path = GameSimulation.PathForArtResource ("characters/monkey/monkey_tree_hang_taunt");
			CAAnimation taunt = LoadAndCacheAnimation (path, "monkey_tree_hang_taunt-1");
			taunt.RepeatCount = 0;

			SCNAnimationEventHandler ackBlock = (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
				GameSimulation.Sim.PlaySound ("ack.caf");
			};

			SCNAnimationEventHandler idleBlock = (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {
				isIdle = true;
			};

			taunt.AnimationEvents = new SCNAnimationEvent[] { 
				SCNAnimationEvent.Create (0.35f, ackBlock),
				SCNAnimationEvent.Create (1.0f, idleBlock)
			};
		}

		void SetupHangAnimation ()
		{
			CAAnimation hang = LoadAndCacheAnimation (GameSimulation.PathForArtResource ("characters/monkey/monkey_tree_hang"), "monkey_tree_hang-1");
			hang.RepeatCount = float.MaxValue;
		}

		void SetupGetCoconutAnimation ()
		{
			SCNAnimationEventHandler pickupEventBlock = (CAAnimation animation, NSObject animatedObject, bool playingBackward) => {

				if (coconutInHand != null)
					coconutInHand.RemoveFromParentNode ();

				coconutInHand = Coconut.CoconutProtoObject;
				rightHand.AddChildNode (coconutInHand);
				hasCoconut = true;
			};

			CAAnimation getAnimation = LoadAndCacheAnimation (GameSimulation.PathForArtResource ("characters/monkey/monkey_get_coconut"), "monkey_get_coconut-1");
			if (getAnimation.AnimationEvents == null)
				getAnimation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.4f, pickupEventBlock) };

			getAnimation.RepeatCount = 1;
		}

		void SetupThrowAnimation ()
		{
			CAAnimation throwAnimation = LoadAndCacheAnimation (GameSimulation.PathForArtResource ("characters/monkey/monkey_throw_coconut"), "monkey_throw_coconut-1");
			throwAnimation.Speed = 1.5f;

			if (throwAnimation.AnimationEvents == null || throwAnimation.AnimationEvents.Length == 0) {
				SCNAnimationEventHandler throwEventBlock = ThrowCoconut;

				throwAnimation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.35f, throwEventBlock) };
			}
				
			throwAnimation.RepeatCount = 0;
		}

		void ThrowCoconut (CAAnimation animation, NSObject animatedObject, bool playingBackward)
		{
			if (!hasCoconut)
				return;

			SCNMatrix4 worldMtx = coconutInHand.PresentationNode.WorldTransform;
			coconutInHand.RemoveFromParentNode ();

			Coconut node = Coconut.CoconutThrowProtoObject;
			SCNPhysicsShape coconutPhysicsShape = Coconut.CoconutPhysicsShape;
			node.PhysicsBody = SCNPhysicsBody.CreateBody (SCNPhysicsBodyType.Dynamic, coconutPhysicsShape);
			node.PhysicsBody.Restitution = 0.9f;
			node.PhysicsBody.CollisionBitMask = GameCollisionCategory.Player | GameCollisionCategory.Ground;
			node.PhysicsBody.CategoryBitMask = GameCollisionCategory.Coconut;

			node.Transform = worldMtx;
			GameSimulation.Sim.RootNode.AddChildNode (node);
			GameSimulation.Sim.GameLevel.Coconuts.Add (node);
			node.PhysicsBody.ApplyForce (new SCNVector3 (-200, 500, 300), true);
			hasCoconut = false;
			isIdle = true;
		}
	}
}

