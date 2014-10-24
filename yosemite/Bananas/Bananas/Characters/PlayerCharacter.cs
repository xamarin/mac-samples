using System;
using SceneKit;
using Foundation;
using CoreAnimation;

namespace Bananas
{
	public class PlayerCharacter : SkinnedCharacter
	{
		SCNVector3 velocity;
		bool changingDirection;
		nfloat baseWalkSpeed;
		nfloat groundPlaneHeight;
		SCNNode cameraHelper;

		nfloat jumpForce;
		nfloat dustWalkingBirthRate;

		WalkDirection playerWalkDirection;
		bool inHitAnimation;
		bool inRunAnimation;
		bool inJumpAnimation;

		public bool IsWalking { get; private set; }

		public nfloat WalkSpeed { get; set; }

		public nfloat JumpBoost { get; private set; }

		public WalkDirection PlayerWalkDirection { 
			get {
				return playerWalkDirection;
			}

			set {
				if ((playerWalkDirection != value && IsWalking && !Launching && !Jumping) && !changingDirection) {
					mainSkeleton.RemoveAllAnimations ();
					NSString key = KeyForAnimationType (CharacterAnimation.RunStop);
					CAAnimation anim = CachedAnimationForKey (key);
					mainSkeleton.AddAnimation (anim, key);
					changingDirection = true;
					WalkSpeed = baseWalkSpeed;
				} else {
					playerWalkDirection = value;
				}
			}
		}

		public bool InRunAnimation { 
			get {
				return inRunAnimation;
			}

			set {
				if (inRunAnimation == value)
					return;

				inRunAnimation = value;

				if (value) {
					WalkSpeed = baseWalkSpeed * 2;

					NSString runKey = KeyForAnimationType (CharacterAnimation.Run);
					NSString idleKey = KeyForAnimationType (CharacterAnimation.Idle);
					CAAnimation runAnim = CachedAnimationForKey (runKey);

					mainSkeleton.RemoveAnimation (idleKey, 0.15f);
					mainSkeleton.AddAnimation (runAnim, runKey);

					if (DustWalking != null)
						DustWalking.BirthRate = dustWalkingBirthRate;
				} else {
					NSString runKey = KeyForAnimationType (CharacterAnimation.Run);
					NSString runStopKey = KeyForAnimationType (CharacterAnimation.Idle);
					CAAnimation runStopAnim = CachedAnimationForKey (runStopKey);

					runStopAnim.FadeInDuration = 0.15f;
					runStopAnim.FadeOutDuration = 0.15f;

					mainSkeleton.RemoveAnimation (runKey, 0.15f);
					mainSkeleton.AddAnimation (runStopAnim, runStopKey);
					WalkSpeed = baseWalkSpeed;
					TurnOffWalkingDust ();
					IsWalking = false;
				}
			}
		}

		public SCNNode CollideSphere { get; set; }

		public bool Jumping { get; set; }

		public bool InHitAnimation { 
			get {
				return inHitAnimation;
			} 

			set {
				inHitAnimation = value;

				CAAnimation anim = CachedAnimationForKey (KeyForAnimationType (CharacterAnimation.GetHit));
				anim.RepeatCount = 0;
				anim.FadeInDuration = 0.15f;
				anim.FadeOutDuration = 0.15f;

				mainSkeleton.AddAnimation (anim, KeyForAnimationType (CharacterAnimation.GetHit));
				inHitAnimation = false;
				GameSimulation.Sim.PlaySound ("coconuthit.caf");
			}
		}

		bool Running { get; set; }

		bool Launching { get; set; }

		SCNParticleSystem DustPoof { get; set; }

		SCNParticleSystem DustWalking { get; set; }

		bool InJumpAnimation { 
			get {
				return inJumpAnimation;
			}

			set {
				if (inJumpAnimation == value)
					return;

				inJumpAnimation = value;
				if (value) {
					// Launching YES means we are in the preflight jump animation.
					Launching = true;
					CAAnimation anim = CachedAnimationForKey (KeyForAnimationType (CharacterAnimation.Jump));
					mainSkeleton.RemoveAllAnimations ();
					mainSkeleton.AddAnimation (anim, KeyForAnimationType (CharacterAnimation.Jump));
					TurnOffWalkingDust ();
				} else {
					Launching = false;
				}
			}
		}

		public PlayerCharacter (SCNNode characterNode) : base (characterNode)
		{
			CategoryBitMask = NodeCategory.Lava;
			velocity = SCNVector3.Zero;
			IsWalking = false;
			changingDirection = false;
			baseWalkSpeed = 0.0167f;
			JumpBoost = 0.0f;

			WalkSpeed = baseWalkSpeed * 2;
			Jumping = false;
			groundPlaneHeight = 0.0f;
			playerWalkDirection = WalkDirection.Right;

			cameraHelper = new SCNNode {
				Position = new SCNVector3 (1000f, 200f, 0f)
			};

			AddChildNode (cameraHelper);

			CollideSphere = new SCNNode {
				Position = new SCNVector3 (0f, 80f, 0f)
			};

			SCNGeometry geo = SCNCapsule.Create (90f, 160f);
			SCNPhysicsShape shape2 = SCNPhysicsShape.Create (geo, (NSDictionary)null);
			CollideSphere.PhysicsBody = SCNPhysicsBody.CreateBody (SCNPhysicsBodyType.Kinematic, shape2);

			CollideSphere.PhysicsBody.CollisionBitMask = 
				GameCollisionCategory.Banana |
			GameCollisionCategory.Coin |
			GameCollisionCategory.Coconut |
			GameCollisionCategory.Lava;

			CollideSphere.PhysicsBody.CategoryBitMask = GameCollisionCategory.Player;
			AddChildNode (CollideSphere);

			DustPoof = GameSimulation.LoadParticleSystemWithName ("dust");
			NSString artResourcePath = (NSString)GameSimulation.PathForArtResource ("level/effects/effects_transparent.png");
			DustPoof.ParticleImage = artResourcePath;
			DustWalking = GameSimulation.LoadParticleSystemWithName ("dustWalking");
			DustWalking.ParticleImage = artResourcePath;
			dustWalkingBirthRate = DustWalking.BirthRate;

			// Load the animations and store via a lookup table.
			SetupIdleAnimation ();
			SetupRunAnimation ();
			SetupJumpAnimation ();
			SetupBoredAnimation ();
			SetupHitAnimation ();

			PlayIdle ();
		}

		public void PerformJumpAndStop (bool stop)
		{
			jumpForce = 13.0f;
			if (stop)
				return;

			JumpBoost += 0.0005f;
			nfloat maxBoost = WalkSpeed * 2.0f;

			if (JumpBoost > maxBoost)
				JumpBoost = maxBoost;
			else
				velocity.Y += 0.55f;

			if (!Jumping) {
				Jumping = true;
				Launching = true;
				InJumpAnimation = true;
			}
		}

		public override void Update (double deltaTime)
		{
			SCNMatrix4 mtx = Transform;
			var gravity = new SCNVector3 (0f, -90f, 0f);
			var gravitystep = SCNVector3.Multiply (gravity, (nfloat)deltaTime);

			velocity = SCNVector3.Add (velocity, gravitystep);
			var minMovement = new SCNVector3 (0f, -50f, 0f);
			var maxMovement = new SCNVector3 (100f, 100f, 100f);
			velocity = MathUtils.GetMaxVector (velocity, minMovement);
			velocity = MathUtils.GetMinVector (velocity, maxMovement);
			mtx = mtx.TranslateWithVector (velocity);
			groundPlaneHeight = GetGroundHeight (mtx);

			if (mtx.M42 < groundPlaneHeight) {
				if (!Launching && velocity.Y < 0.0f) {
					if (Jumping) {
						Jumping = false;
						if (DustPoof != null) {
							AddParticleSystem (DustPoof);
							DustPoof.Loops = false;
						}
						PlayLand ();
						JumpBoost = 0.0f;
					}
				}
				// tie to ground
				mtx.M42 = groundPlaneHeight;
				
				velocity.Y = 0.0f;
			}

			Transform = mtx;
			//-- move the camera
			SCNNode camera = GameSimulation.Sim.GameLevel.Camera.ParentNode;
			if (camera != null) {
				nfloat x = Position.X + ((PlayerWalkDirection == WalkDirection.Right) ? 250 : -250);
				nfloat y = (Position.Y + 261) - (0.85f * (Position.Y - groundPlaneHeight));
				nfloat z = Position.Z + 1500;

				var pos = new SCNVector3 (x, y, z);

				SCNMatrix4 desiredTransform = camera.Transform.SetPosition (pos);
				camera.Transform = MathUtils.Interpolate (camera.Transform, desiredTransform, 0.025f);
			}
		}

		NSString KeyForAnimationType (CharacterAnimation animType)
		{
			var resultString = string.Empty;
			switch (animType) {
			case CharacterAnimation.Bored:
				resultString = "bored-1";
				break;
			case CharacterAnimation.Die:
				resultString = "die-1";
				break;
			case CharacterAnimation.GetHit:
				resultString = "hit-1";
				break;
			case CharacterAnimation.Idle:
				resultString = "idle-1";
				break;
			case CharacterAnimation.Jump:
				resultString = "jump_start-1";
				break;
			case CharacterAnimation.JumpFalling:
				resultString = "jump_falling-1";
				break;
			case CharacterAnimation.JumpLand:
				resultString = "jump_land-1";
				break;
			case CharacterAnimation.Run:
				resultString = "run-1";
				break;
			case CharacterAnimation.RunStart:
				resultString = "run_start-1";
				break;
			case CharacterAnimation.RunStop:
				resultString = "run_stop-1";
				break;
			}

			return new NSString (resultString);
		}

		nfloat GetGroundHeight (SCNMatrix4 matrix)
		{
			SCNVector3 start = new SCNVector3 (matrix.M41, matrix.M42 + 1000, matrix.M43);
			SCNVector3 end = new SCNVector3 (matrix.M41, matrix.M42 - 3000, matrix.M43);

			var options = NSDictionary.FromObjectsAndKeys (new NSObject[] { new NSNumber (GameCollisionCategory.Ground | GameCollisionCategory.Lava), (NSString)"closest" }, 
				              new NSObject[] { SCNPhysicsTestKeys.CollisionBitMaskKey, SCNPhysicsTestKeys.SearchModeKey });

			SCNHitTestResult[] hits = GameSimulation.Sim.PhysicsWorld.RayTestWithSegmentFromPoint (start, end, options);

			if (hits == null || hits.Length == 0)
				return 0;

			foreach (SCNHitTestResult result in hits) {
				if (result.Node.PhysicsBody.CategoryBitMask != (GameCollisionCategory.Ground | GameCollisionCategory.Lava))
					return result.WorldCoordinates.Y;
			}

			return 0;
		}

		void SetupIdleAnimation ()
		{
			CAAnimation idleAnimation = LoadAndCacheAnimation ("art.scnassets/characters/explorer/idle", KeyForAnimationType (CharacterAnimation.Idle));

			if (idleAnimation == null)
				return;

			idleAnimation.RepeatCount = float.MaxValue;
			idleAnimation.FadeInDuration = 0.15f;
			idleAnimation.FadeOutDuration = 0.15f;
		}

		void SetupHitAnimation ()
		{
			SetupAnimation ("art.scnassets/characters/explorer/hit", CharacterAnimation.GetHit);
		}

		void SetupBoredAnimation ()
		{
			SetupAnimation ("art.scnassets/characters/explorer/bored", CharacterAnimation.Bored);
		}

		void SetupAnimation (string path, CharacterAnimation animationType)
		{
			LoadAndCacheAnimation (path, KeyForAnimationType (animationType));
			CAAnimation animation = CachedAnimationForKey (KeyForAnimationType (animationType));

			if (animation != null)
				animation.RepeatCount = float.MaxValue;
		}

		void SetupJumpAnimation ()
		{
			NSString jumpKey = KeyForAnimationType (CharacterAnimation.Jump);
			NSString fallingKey = KeyForAnimationType (CharacterAnimation.JumpFalling);
			NSString landKey = KeyForAnimationType (CharacterAnimation.JumpLand);
			NSString idleKey = KeyForAnimationType (CharacterAnimation.Idle);

			CAAnimation jumpAnimation = LoadAndCacheAnimation ("art.scnassets/characters/explorer/jump_start", jumpKey);
			CAAnimation fallAnimation = LoadAndCacheAnimation ("art.scnassets/characters/explorer/jump_falling", fallingKey);
			CAAnimation landAnimation = LoadAndCacheAnimation ("art.scnassets/characters/explorer/jump_land", landKey);

			jumpAnimation.FadeInDuration = 0.15f;
			jumpAnimation.FadeOutDuration = 0.15f;
			fallAnimation.FadeInDuration = 0.15f;
			landAnimation.FadeInDuration = 0.15f;
			landAnimation.FadeOutDuration = 0.15f;

			jumpAnimation.RepeatCount = 0;
			fallAnimation.RepeatCount = 0;
			landAnimation.RepeatCount = 0;

			jumpForce = 7.0f;


			SCNAnimationEventHandler leaveGroundBlock = (animation, animatedObject, playingBackward) => {
				var jumpVelocity = new SCNVector3 (0f, jumpForce * 2.1f, 0f);
				velocity = SCNVector3.Add (velocity, jumpVelocity);
				Launching = false;
				InJumpAnimation = false;
			};

			SCNAnimationEventHandler pause = (animation, animatedObject, playingBackward) => {
				mainSkeleton.PauseAnimation (fallingKey);
			};

			jumpAnimation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.25f, leaveGroundBlock) };
			fallAnimation.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (0.5f, pause) };

			// Animation Sequence is to Jump -> Fall -> Land -> Idle.
			ChainAnimation (jumpKey, fallingKey);
			ChainAnimation (landKey, idleKey);
		}

		void SetupRunAnimation ()
		{
			NSString runKey = KeyForAnimationType (CharacterAnimation.Run);
			NSString runStartKey = KeyForAnimationType (CharacterAnimation.RunStart);
			NSString runStopKey = KeyForAnimationType (CharacterAnimation.RunStop);

			CAAnimation runAnim = LoadAndCacheAnimation ("art.scnassets/characters/explorer/run", runKey);
			CAAnimation runStartAnim = LoadAndCacheAnimation ("art.scnassets/characters/explorer/run_start", runStartKey);
			CAAnimation runStopAnim = LoadAndCacheAnimation ("art.scnassets/characters/explorer/run_stop", runStopKey);

			runAnim.RepeatCount = float.MaxValue;
			runStartAnim.RepeatCount = 0;
			runStopAnim.RepeatCount = 0;

			runAnim.FadeInDuration = 0.05f;
			runAnim.FadeOutDuration = 0.05f;
			runStartAnim.FadeInDuration = 0.05f;
			runStartAnim.FadeOutDuration = 0.05f;
			runStopAnim.FadeInDuration = 0.05f;
			runStopAnim.FadeOutDuration = 0.05f;

			SCNAnimationEventHandler stepLeftBlock = (animation, animatedObject, playingBackward) => {
				GameSimulation.Sim.PlaySound ("leftstep.caf");
			};

			SCNAnimationEventHandler stepRightBlock = (animation, animatedObject, playingBackward) => {
				GameSimulation.Sim.PlaySound ("rightstep.caf");
			};

			SCNAnimationEventHandler startWalkStateBlock = (animation, animatedObject, playingBackward) => {
				if (InRunAnimation)
					IsWalking = true;
				else
					mainSkeleton.RemoveAnimation (runKey, 0.15f);
			};

			SCNAnimationEventHandler stopWalkStateBlock = (animation, animatedObject, playingBackward) => {
				IsWalking = false;
				TurnOffWalkingDust ();
				if (changingDirection) {
					inRunAnimation = false;
					InRunAnimation = true;
					changingDirection = false;
					playerWalkDirection = playerWalkDirection == WalkDirection.Left ? WalkDirection.Right : WalkDirection.Left;
				}
			};

			runStopAnim.AnimationEvents = new SCNAnimationEvent[] { SCNAnimationEvent.Create (1f, stopWalkStateBlock) };
			runAnim.AnimationEvents = new SCNAnimationEvent[] { 
				SCNAnimationEvent.Create (0.0f, startWalkStateBlock),
				SCNAnimationEvent.Create (0.25f, stepRightBlock),
				SCNAnimationEvent.Create (0.75f, stepLeftBlock)
			};
		}

		void PlayIdle ()
		{
			TurnOffWalkingDust ();

			CAAnimation animation = CachedAnimationForKey (KeyForAnimationType (CharacterAnimation.Idle));
			animation.RepeatCount = float.MaxValue;
			animation.FadeInDuration = 0.1f;
			animation.FadeOutDuration = 0.1f;
			
			mainSkeleton.AddAnimation (animation, KeyForAnimationType (CharacterAnimation.Idle));
		}

		void PlayLand ()
		{
			NSString fallKey = KeyForAnimationType (CharacterAnimation.JumpFalling);
			NSString key = KeyForAnimationType (CharacterAnimation.JumpLand);
			CAAnimation anim = CachedAnimationForKey (key);
			anim.TimeOffset = 0.65f;
			mainSkeleton.RemoveAnimation (fallKey, 0.15f);
			InJumpAnimation = false;
			if (IsWalking) {
				inRunAnimation = false;
				InRunAnimation = true;
			} else {
				mainSkeleton.AddAnimation (anim, key);
			}

			GameSimulation.Sim.PlaySound ("Land.wav");
		}

		void TurnOffWalkingDust ()
		{
			// Stop the flow of dust by turning the birthrate to 0.
			if (DustWalking != null && ParticleSystems != null && Array.IndexOf (ParticleSystems, DustWalking) > -1)
				DustWalking.BirthRate = 0;
		}
	}
}

