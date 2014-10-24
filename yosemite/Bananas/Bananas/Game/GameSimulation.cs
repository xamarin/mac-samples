using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using SceneKit;
using SpriteKit;

namespace Bananas
{
	public enum WalkDirection
	{
		Left = 0,
		Right,
	}

	public enum CharacterAnimation
	{
		Die = 0,
		Run,
		Jump,
		JumpFalling,
		JumpLand,
		Idle,
		GetHit,
		Bored,
		RunStart,
		RunStop,
		Count
	}

	public static class GameCollisionCategory
	{
		public static readonly nuint Ground = 1 << 2;
		public static readonly nuint Banana = 1 << 3;
		public static readonly nuint Player = 1 << 4;
		public static readonly nuint Lava = 1 << 5;
		public static readonly nuint Coin = 1 << 6;
		public static readonly nuint Coconut = 1 << 7;
		public static readonly nuint NoCollide = 1 << 14;
	}

	public static class NodeCategory
	{
		public static readonly nuint Torch = 1 << 2;
		public static readonly nuint Lava = 1 << 3;
		public static readonly nuint Lava2 = 1 << 4;
	}

	public class GameSimulation : SCNScene, ISCNSceneRendererDelegate, ISCNPhysicsContactDelegate
	{
		static GameSimulation sharedSimulation;

		const string artFolderRoot = "art.scnassets";

		SCNTechnique desaturationTechnique;
		double previousUpdateTime;
		double deltaTime;

		public InGameScene GameUIScene { get; set; }

		public GameState CurrentGameState { get; set; }

		public GameLevel GameLevel { get; set; }

		public static GameSimulation Sim {
			get {
				if (sharedSimulation == null)
					sharedSimulation = new GameSimulation ();

				return sharedSimulation;
			}
		}

		public GameSimulation ()
		{
			// We create one level in our simulation.
			GameLevel = new GameLevel ();
			CurrentGameState = GameState.Paused;

			// Register ourself as a listener to physics callbacks.
			SCNNode levelNode = GameLevel.CreateLevel ();
			RootNode.AddChildNode (levelNode);
			PhysicsWorld.WeakContactDelegate = this;
			PhysicsWorld.Gravity = new SCNVector3 (0f, -800f, 0f);

			SetupTechniques ();
		}

		public static string PathForArtResource (string resourceName)
		{
			return string.Format ("{0}/{1}", artFolderRoot, resourceName);
		}

		public static SCNNode LoadNodeWithName (string name, string path)
		{
			var loadingOptions = NSDictionary.FromObjectsAndKeys (new object[] { "YES", SCNSceneSourceLoading.AnimationImportPolicyPlayRepeatedly },
				                     new object[] { SCNSceneSourceLoading.ConvertToYUpKey, SCNSceneSourceLoading.AnimationImportPolicyKey });

			string sceneName = new NSUrl (path).LastPathComponent.ToString ();
			string scenePath = new NSUrl (path).RemoveLastPathComponent ().ToString ();
			SCNScene scene = SCNScene.FromFile (sceneName, scenePath, loadingOptions);
			SCNNode node = scene.RootNode;

			// Search for the node named "name"
			if (!string.IsNullOrEmpty (name))
				node = node.FindChildNode (name, true);
			else
				node = node.ChildNodes [0];

			return node;
		}

		public static SCNParticleSystem LoadParticleSystemWithName (string name, string paticleImageName = "")
		{
			string path = string.Format ("level/effects/{0}.scnp", name);
			path = PathForArtResource (path);

			var newSystem = (SCNParticleSystem)NSKeyedUnarchiver.UnarchiveFile (path);

			if (!string.IsNullOrEmpty (paticleImageName))
				newSystem.ParticleImage = (NSString)PathForArtResource (string.Format ("level/effects/{0}.png", paticleImageName));

			return newSystem;
		}

		public void PlaySound (string soundFileName)
		{
			if (string.IsNullOrEmpty (soundFileName))
				return;

			var path = string.Format ("Sounds/{0}", soundFileName);
			GameUIScene.RunAction (SKAction.PlaySoundFileNamed (path, false));
		}

		public void PlayMusic (string soundFileName)
		{
			if (soundFileName == null)
				return;

			if (GameUIScene.GetActionForKey (soundFileName) != null)
				return;

			string path = string.Format ("Sounds/{0}", soundFileName);
			SKAction repeatAction = SKAction.RepeatActionForever (SKAction.PlaySoundFileNamed (path, true));
			GameUIScene.RunAction (repeatAction, soundFileName);
		}


		public void SetGameState (GameState gameState)
		{
			// Ignore redundant state changes.
			if (CurrentGameState == gameState)
				return;

			// Change the UI system according to gameState.
			GameUIScene.SetGameState (gameState);

			// Only reset the level from a non paused mode.
			if (gameState == GameState.InGame && CurrentGameState != GameState.Paused)
				GameLevel.ResetLevel ();

			CurrentGameState = gameState;

			// Based on the new game state... set the saturation value
			// that the techniques will use to render the scenekit view.
			if (CurrentGameState == GameState.PostGame) {
				SetPostGameFilters ();
			} else if (CurrentGameState == GameState.Paused) {
				Sim.PlaySound ("deposit.caf");
				SetPauseFilters ();
			} else if (CurrentGameState == GameState.PreGame) {
				SetPregameFilters ();
			} else {
				Sim.PlaySound ("ack.caf");
				SetIngameFilters ();
			}
		}

		[Export ("renderer:didSimulatePhysicsAtTime:")]
		public virtual void DidSimulatePhysics (SCNSceneRenderer renderer, double timeInSeconds)
		{
			GameLevel.Update (deltaTime, renderer);
		}

		[Export ("renderer:updateAtTime:")]
		public virtual void Update (ISCNSceneRenderer renderer, double timeInSeconds)
		{
			if (previousUpdateTime == 0.0)
				previousUpdateTime = timeInSeconds;

			deltaTime = timeInSeconds - previousUpdateTime;
			previousUpdateTime = timeInSeconds;

			SharedSceneView aView = (SharedSceneView)renderer;

			bool pressingLeft = aView.KeysPressed.Contains (SharedSceneView.LeftKey);
			bool pressingRight = aView.KeysPressed.Contains (SharedSceneView.RightKey);
			bool pressingJump = aView.KeysPressed.Contains (SharedSceneView.JumpKey);

			if (CurrentGameState == GameState.InGame && !GameLevel.HitByLavaReset) {
				if (pressingLeft)
					GameLevel.PlayerCharacter.PlayerWalkDirection = WalkDirection.Left;
				else if (pressingRight)
					GameLevel.PlayerCharacter.PlayerWalkDirection = WalkDirection.Right;

				if (pressingLeft || pressingRight)
					//Run if not running
					GameLevel.PlayerCharacter.InRunAnimation = true;
				else
					//Stop running if running
					GameLevel.PlayerCharacter.InRunAnimation = false;

				if (pressingJump)
					GameLevel.PlayerCharacter.PerformJumpAndStop (false);
				else
					GameLevel.PlayerCharacter.PerformJumpAndStop (true);
			} else if (CurrentGameState == GameState.PreGame || CurrentGameState == GameState.PostGame) {
				if (pressingJump)
					SetGameState (GameState.InGame);
			}
		}

		[Export ("physicsWorld:didBeginContact:")]
		public virtual void DidBeginContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
		{
			if (CurrentGameState == GameState.InGame) {
				// Player to banana, large banana, or coconut
				if (contact.NodeA == GameLevel.PlayerCharacter.CollideSphere) {
					PlayerCollideWithContact (contact.NodeB, contact.ContactPoint);
					return;
				} else if (contact.NodeB == GameLevel.PlayerCharacter.CollideSphere) {
					PlayerCollideWithContact (contact.NodeA, contact.ContactPoint);
					return;
				}

				// Coconut to anything but the player.
				if ((contact.NodeB.CategoryBitMask == GameCollisionCategory.Coconut)) {
					HandleCollideForCoconut ((Coconut)contact.NodeB);
				} else if ((contact.NodeA.CategoryBitMask == GameCollisionCategory.Coconut)) {
					HandleCollideForCoconut ((Coconut)contact.NodeA);
				}
			}
		}

		void SetupTechniques ()
		{
			// The scene can be de-saturarted as a full screen effect.
			NSUrl url = NSBundle.MainBundle.GetUrlForResource ("art.scnassets/techniques/desaturation", "plist");
			NSDictionary options = NSDictionary.FromUrl (url);
			desaturationTechnique = SCNTechnique.Create (options);
			desaturationTechnique.SetValueForKey (new NSNumber (0.0), (NSString)"Saturation");
		}

		void SetPostGameFilters ()
		{
			SCNTransaction.Begin ();

			desaturationTechnique.SetValueForKey (new NSNumber (1.0), (NSString)"Saturation");
			SCNTransaction.AnimationDuration = 1.0;

			SCNTransaction.Commit ();

			SharedAppDelegate appDelegate = SharedAppDelegate.AppDelegate;
			appDelegate.Scene.Technique = desaturationTechnique;
		}

		void SetPauseFilters ()
		{
			SCNTransaction.Begin ();

			desaturationTechnique.SetValueForKey (new NSNumber (1.0), (NSString)"Saturation");
			SCNTransaction.AnimationDuration = 1.0;
			desaturationTechnique.SetValueForKey (new NSNumber (0.0), (NSString)"Saturation");

			SCNTransaction.Commit ();

			SharedAppDelegate appDelegate = SharedAppDelegate.AppDelegate;
			appDelegate.Scene.Technique = desaturationTechnique;
		}

		void SetPregameFilters ()
		{
			SCNTransaction.Begin ();

			desaturationTechnique.SetValueForKey (new NSNumber (1.0), (NSString)"Saturation");
			SCNTransaction.AnimationDuration = 1.0;
			desaturationTechnique.SetValueForKey (new NSNumber (0.0), (NSString)"Saturation");

			SCNTransaction.Commit ();

			SharedAppDelegate appDelegate = SharedAppDelegate.AppDelegate;
			appDelegate.Scene.Technique = desaturationTechnique;
		}

		void SetIngameFilters ()
		{
			SCNTransaction.Begin ();

			desaturationTechnique.SetValueForKey (new NSNumber (0.0), (NSString)"Saturation");
			SCNTransaction.AnimationDuration = 1.0;
			desaturationTechnique.SetValueForKey (new NSNumber (1.0), (NSString)"Saturation");

			SCNTransaction.Commit ();

			SCNAction dropTechnique = SCNAction.Wait (1.0f);

			SharedAppDelegate appDelegate = SharedAppDelegate.AppDelegate;
			appDelegate.Scene.Scene.RootNode.RunAction (dropTechnique, () => {
				appDelegate.Scene.Technique = null;
			});
		}

		void HandleCollideForCoconut (Coconut coconut)
		{
			// Remove coconut from the world after it has time to fall offscreen.
			coconut.RunAction (SCNAction.Wait (3.0), () => {
				coconut.RemoveFromParentNode ();
				GameLevel.Coconuts.Remove (coconut);
			});
		}

		void PlayerCollideWithContact (SCNNode node, SCNVector3 contactPoint)
		{
			if (GameLevel.Bananas.Contains (node) == true) {
				GameLevel.CollectBanana (node);
			} else if (GameLevel.LargeBananas.Contains (node) == true) {
				GameLevel.CollectLargeBanana (node);
			} else if (node.CategoryBitMask == 1) {
				GameLevel.CollideWithCoconut (node, contactPoint);
			} else if (node.CategoryBitMask == 8) {
				GameLevel.CollideWithLava ();
			}
		}
	}
}

