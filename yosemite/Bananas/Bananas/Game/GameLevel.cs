using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using SceneKit;

namespace Bananas
{
	public class GameLevel : NSObject, IGameUIState
	{
		SCNVector3 lightOffsetFromCharacter;
		SCNVector3 screenSpacePlayerPosition;
		SCNVector3 worldSpaceLabelScorePosition;
		SCNAction hoverAction;
		SCNAction bananaIdleAction;
		SCNNode bananaCollectable;
		SCNNode largeBananaCollectable;

		List<MonkeyCharacter> Monkeys { get; set; }

		SCNNode RootNode { get; set; }

		nfloat TimeAlongPath { get; set; }

		SCNNode SunLight { get; set; }

		List<SCNVector3> PathPositions { get; set; }

		public bool HitByLavaReset { get; set; }

		public PlayerCharacter PlayerCharacter { get; set; }

		public List<Coconut> Coconuts { get; set; }

		public List<SCNNode> Bananas { get; set; }

		public List<SCNNode> LargeBananas { get; set; }

		public SCNNode Camera { get; set; }

		public int Score { get; set; }

		public int CoinsCollected { get; set; }

		public int BananasCollected { get; set; }

		public double SecondsRemaining { get; set; }

		public CGPoint ScoreLabelLocation { get; set; }

		SCNAction BananaIdleAction {
			get {
				if (bananaIdleAction == null) {
					SCNAction rotateAction = SCNAction.RotateBy (0f, (nfloat)Math.PI / 2f, 0f, 1.0);
					rotateAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;
					SCNAction reversed = rotateAction.ReversedAction ();
					bananaIdleAction = SCNAction.Sequence (new SCNAction[] { rotateAction, reversed });
				}

				return bananaIdleAction;
			}
		}

		SCNAction HoverAction {
			get {
				if (hoverAction == null) {
					SCNAction floatAction = SCNAction.MoveBy (new SCNVector3 (0f, 10f, 0f), 1.0f);
					SCNAction floatAction2 = floatAction.ReversedAction ();
					floatAction.TimingMode = SCNActionTimingMode.EaseInEaseOut;
					floatAction2.TimingMode = SCNActionTimingMode.EaseInEaseOut;
					hoverAction = SCNAction.Sequence (new SCNAction[] { floatAction, floatAction2 });
				}

				return hoverAction;
			}
		}

		SCNAction PulseAction {
			get {
				float duration = 8.0f / 6.0f;
				SCNAction pulseAction = SCNAction.RepeatActionForever (SCNAction.Sequence (new SCNAction[] {
					SCNAction.FadeOpacityTo (0.5f, duration),
					SCNAction.FadeOpacityTo (1.0f, duration),
					SCNAction.FadeOpacityTo (0.7f, duration),
					SCNAction.FadeOpacityTo (0.4f, duration),
					SCNAction.FadeOpacityTo (0.8f, duration)
				}));

				return pulseAction;
			}
		}

		SCNLight TorchLight {
			get {
				var light = new SCNLight ();
				light.LightType = SCNLightType.Omni;
				light.Color = AppKit.NSColor.Orange;

				light.AttenuationStartDistance = 350;
				light.AttenuationEndDistance = 400;
				light.AttenuationFalloffExponent = 1;
				return light;
			}
		}

		public GameLevel ()
		{
		}

		public SCNNode CreateLevel ()
		{
			RootNode = new SCNNode ();

			// load level dae and add all root children to the scene.
			var options = new SCNSceneLoadingOptions { ConvertToYUp = true };
			SCNScene scene = SCNScene.FromFile ("level", GameSimulation.PathForArtResource ("level/"), options);
			foreach (SCNNode node in scene.RootNode.ChildNodes)
				RootNode.AddChildNode (node);

			// retrieve the main camera
			Camera = RootNode.FindChildNode ("camera_game", true);

			// create our path that the player character will follow.
			CalculatePathPositions ();

			// Sun/Moon light
			SunLight = RootNode.FindChildNode ("FDirect001", true);
			SunLight.EulerAngles = new SCNVector3 (7.1f * (nfloat)Math.PI / 4, (nfloat)Math.PI / 4, 0f);
			SunLight.Light.ShadowSampleCount = 1;
			lightOffsetFromCharacter = new SCNVector3 (1500f, 2000f, 1000f);

			//workaround directional light deserialization issue
			SunLight.Light.ZNear = 100f;
			SunLight.Light.ZFar = 5000f;
			SunLight.Light.OrthographicScale = 1000f;

			// Torches
			var torchesPos = new float[] { 0f, -1f, 0.092467f, -1f, -1f, 0.5f, 0.792f, 0.95383f };
			for (int i = 0; i < torchesPos.Length; i++) {
				if (torchesPos [i] != -1) {
					SCNVector3 location = LocationAlongPath (torchesPos [i]);
					location.Y += 50;
					location.Z += 150;

					SCNNode node = CreateTorchNode ();

					node.Position = location;
					RootNode.AddChildNode (node);
				}
			}

			// After load, we add nodes that are dynamic / animated / or otherwise not static.
			CreateLavaAnimation ();
			CreateSwingingTorch ();
			AnimateDynamicNodes ();

			// Create our player character
			SCNNode characterRoot = GameSimulation.LoadNodeWithName (string.Empty, "art.scnassets/characters/explorer/explorer_skinned.dae");
			PlayerCharacter = new PlayerCharacter (characterRoot); 
			TimeAlongPath = 0;

			PlayerCharacter.Position = LocationAlongPath (TimeAlongPath);
			PlayerCharacter.Rotation = GetPlayerDirectionFromCurrentPosition ();
			RootNode.AddChildNode (PlayerCharacter);

			// Optimize lighting and shadows
			// only the charadcter should cast shadows
			foreach (var child in RootNode.ChildNodes)
				child.CastsShadow = false;

			foreach (var child in PlayerCharacter.ChildNodes)
				child.CastsShadow = true;

			// Add some monkeys to the scene.
			AddMonkeyAtPosition (new SCNVector3 (0f, -30f, -400f), 0f);
			AddMonkeyAtPosition (new SCNVector3 (3211f, 146f, -400f), -(nfloat)Math.PI / 4f);
			AddMonkeyAtPosition (new SCNVector3 (5200f, 330f, 600f), 0f);

			// Volcano
			SCNNode oldVolcano = RootNode.FindChildNode ("volcano", true);
			string volcanoDaeName = GameSimulation.PathForArtResource ("level/volcano_effects.dae");
			SCNNode newVolcano = GameSimulation.LoadNodeWithName ("dummy_master", volcanoDaeName);

			oldVolcano.AddChildNode (newVolcano);
			oldVolcano.Geometry = null;
			oldVolcano = newVolcano.FindChildNode ("volcano", true);
			oldVolcano = oldVolcano.ChildNodes [0];

			// Animate our dynamic volcano node.
			string shaderCode = "uniform float speed;\n" +
			                    "_geometry.color = vec4(a_color.r, a_color.r, a_color.r, a_color.r);\n" +
			                    "_geometry.texcoords[0] += (vec2(0.0, 1.0) * 0.05 * u_time);\n";

			string fragmentShaderCode = "#pragma transparent\n";

			// Dim background
			SCNNode back = RootNode.FindChildNode ("dumy_rear", true);
			foreach (var child in back.ChildNodes) {
				child.CastsShadow = false;

				if (child.Geometry == null)
					continue;

				foreach (SCNMaterial material in child.Geometry.Materials) {
					material.LightingModelName = SCNLightingModel.Constant;
					material.Multiply.Contents = AppKit.NSColor.FromDeviceWhite (0.3f, 1f);

					material.Multiply.Intensity = 1;
				}
			}

			SCNNode backMiddle = RootNode.FindChildNode ("dummy_middle", true);
			foreach (var child in backMiddle.ChildNodes) {

				if (child.Geometry == null)
					continue;

				foreach (SCNMaterial material in child.Geometry.Materials)
					material.LightingModelName = SCNLightingModel.Constant;
			}

			foreach (var child in newVolcano.ChildNodes)
				foreach (var volc in child.ChildNodes) {
					if (volc != oldVolcano && volc.Geometry != null) {
						volc.Geometry.FirstMaterial.LightingModelName = SCNLightingModel.Constant;
						volc.Geometry.FirstMaterial.Multiply.Contents = AppKit.NSColor.White;

						volc.Geometry.ShaderModifiers = new SCNShaderModifiers { 
							EntryPointGeometry = shaderCode,
							EntryPointFragment = fragmentShaderCode
						};
					}
				}

			Coconuts = new List<Coconut> ();
			return RootNode;
		}

		public void CollectBanana (SCNNode banana)
		{
			// Flyoff the banana to the screen space position score label.
			// Don't increment score until the banana hits the score label.

			// ignore collisions
			banana.PhysicsBody = null;
			BananasCollected++;

			int variance = 60;
			nfloat duration = 0.25f;

			nfloat apexY = worldSpaceLabelScorePosition.Y * 0.8f + (new Random ().Next (0, variance) - variance / 2);
			worldSpaceLabelScorePosition.Z = banana.Position.Z;
			var apex = new SCNVector3 (banana.Position.X + 10 + (new Random ().Next (0, variance) - variance / 2), apexY, banana.Position.Z);

			SCNAction startFlyOff = SCNAction.MoveTo (apex, duration);
			startFlyOff.TimingMode = SCNActionTimingMode.EaseOut;

			SCNAction endFlyOff = SCNAction.CustomAction (duration, new SCNActionNodeWithElapsedTimeHandler ((node, elapsedTime) => {
				nfloat t = elapsedTime / duration;
				var v = new SCNVector3 (
					        apex.X + ((worldSpaceLabelScorePosition.X - apex.X) * t),
					        apex.Y + ((worldSpaceLabelScorePosition.Y - apex.Y) * t),
					        apex.X + ((worldSpaceLabelScorePosition.Z - apex.Z) * t));
				node.Position = v;
			}));

			endFlyOff.TimingMode = SCNActionTimingMode.EaseInEaseOut;
			SCNAction flyoffSequence = SCNAction.Sequence (new SCNAction[] { startFlyOff, endFlyOff });

			banana.RunAction (flyoffSequence, () => {
				Bananas.Remove (banana);
				banana.RemoveFromParentNode ();
				// Add to score.
				Score++;
				GameSimulation.Sim.PlaySound ("deposit.caf");

				// Game Over
				if (Bananas.Count == 0)
					DoGameOver ();
			});
		}

		public void CollectLargeBanana (SCNNode largeBanana)
		{
			// When the player hits a large banana, explode it into smaller bananas.
			// We explode into a predefined pattern: square, diamond, letterA, letterB

			// ignore collisions
			largeBanana.PhysicsBody = null;
			CoinsCollected++;

			LargeBananas.Remove (largeBanana);
			largeBanana.RemoveAllParticleSystems ();
			largeBanana.RemoveFromParentNode ();

			// Add to score.
			Score += 100;

			var square = new string[] {
				"1", "1", "1", "1", "1",
				"1", "1", "1", "1", "1",
				"1", "1", "1", "1", "1",
				"1", "1", "1", "1", "1",
				"1", "1", "1", "1", "1"
			};

			var diamond = new string[] {
				"0", "0", "1", "0", "0",
				"0", "1", "1", "1", "0",
				"1", "1", "1", "1", "1",
				"0", "1", "1", "1", "0",
				"0", "0", "1", "0", "0"
			};

			var letterA = new string[] {
				"1", "0", "0", "1", "0",
				"1", "0", "0", "1", "0",
				"1", "1", "1", "1", "0",
				"1", "0", "0", "1", "0",
				"0", "1", "1", "0", "0"
			};

			var letterB = new string[] {
				"1", "1", "0", "0", "0",
				"1", "0", "1", "0", "0",
				"1", "1", "0", "0", "0",
				"1", "0", "1", "0", "0",
				"1", "1", "0", "0", "0"
			};

			var choices = new List<string[]> { square, diamond, letterA, letterB };
			float vertSpacing = 40f;
			float spacing = 0.0075f;
			string[] choice = choices [new Random ().Next (0, choices.Count)];
			for (int y = 0; y < 5; y++) {
				for (int x = 0; x < 5; x++) {

					int place = Int32.Parse (choice [(y * 5) + x]);
					if (place != 1)
						continue;

					SCNNode banana = CreateBanana ();
					RootNode.AddChildNode (banana);
					banana.Position = largeBanana.Position;
					banana.PhysicsBody.CategoryBitMask = GameCollisionCategory.NoCollide;
					banana.PhysicsBody.CollisionBitMask = GameCollisionCategory.Ground;

					SCNVector3 endPoint = LocationAlongPath (TimeAlongPath + spacing * (x + 1));
					endPoint.Y += (vertSpacing * (y + 1));

					SCNAction flyoff = SCNAction.MoveTo (endPoint, MathUtils.RandomPercent () * 0.25f);
					flyoff.TimingMode = SCNActionTimingMode.EaseInEaseOut;

					banana.RunAction (flyoff, () => {
						banana.PhysicsBody.CategoryBitMask = GameCollisionCategory.Banana;
						banana.PhysicsBody.CollisionBitMask = GameCollisionCategory.Ground | GameCollisionCategory.Player;
						GameSimulation.Sim.PlaySound ("deposit.caf");
					});

					Bananas.Add (banana);
				}
			}
		}

		public void CollideWithCoconut (SCNNode coconut, SCNVector3 contactPoint)
		{
			// No more collisions. Let it bounce away and fade out.
			coconut.PhysicsBody.CollisionBitMask = 0;
			coconut.RunAction (SCNAction.Sequence (new SCNAction[] {
				SCNAction.Wait (1.0),
				SCNAction.FadeOut (1.0),
				SCNAction.RemoveFromParentNode ()
			}), () => {
				Coconuts.Remove ((Coconut)coconut);
			});

			// Decrement score
			int amountToDrop = Score / 10;

			amountToDrop = Math.Max (1, amountToDrop);
			amountToDrop = Math.Min (10, amountToDrop);

			if (amountToDrop > Score)
				amountToDrop = Score;

			Score -= amountToDrop;

			// Throw bananas
			float spacing = 40f;
			for (int x = 0; x < amountToDrop; x++) {
				SCNNode banana = CreateBanana ();
				RootNode.AddChildNode (banana);
				banana.Position = contactPoint;
				banana.PhysicsBody.CategoryBitMask = GameCollisionCategory.NoCollide;
				banana.PhysicsBody.CollisionBitMask = GameCollisionCategory.Ground;
				SCNVector3 endPoint = SCNVector3.Zero;
				endPoint.X -= (spacing * x) + spacing;

				SCNAction flyoff = SCNAction.MoveBy (endPoint, MathUtils.RandomPercent () * 0.750f);
				flyoff.TimingMode = SCNActionTimingMode.EaseInEaseOut;

				banana.RunAction (flyoff, () => {
					banana.PhysicsBody.CategoryBitMask = GameCollisionCategory.Banana;
					banana.PhysicsBody.CollisionBitMask = GameCollisionCategory.Ground | GameCollisionCategory.Player;
				});

				Bananas.Add (banana);
			}

			PlayerCharacter.InHitAnimation = true;
		}

		public void Update (double deltaTime, SCNSceneRenderer aRenderer)
		{
			// Based on gamestate:
			// ingame: Move the character if running.
			// ingame: prevent movement of the character past our level bounds.
			// ingame: perform logic for the player character.
			// any: move the directional light with any player movement.
			// ingame: update the coconuts kinematically.
			// ingame: perform logic for each monkey.
			// ingame: because our camera could have moved, update the transforms needs to fly
			//         collected bananas from the player (world space) to score (screen space)

			var appDelegate = SharedAppDelegate.AppDelegate;
			var currentState = GameSimulation.Sim.CurrentGameState;

			// Move character along path if walking.
			MoveCharacterAlongPathWith (deltaTime, currentState);

			// Based on the time along path, rotation the character to face the correct direction.
			PlayerCharacter.Rotation = GetPlayerDirectionFromCurrentPosition ();
			if (currentState == GameState.InGame)
				PlayerCharacter.Update (deltaTime);

			// Move the light
			UpdateSunLightPosition ();

			if (currentState == GameState.PreGame ||
			    currentState == GameState.PostGame ||
			    currentState == GameState.Paused)
				return;

			foreach (MonkeyCharacter monkey in Monkeys)
				monkey.Update (deltaTime);

			// Update timer and check for Game Over.
			SecondsRemaining -= deltaTime;
			if (SecondsRemaining < 0.0)
				DoGameOver ();

			// update the player's SP position.
			SCNVector3 playerPosition = PlayerCharacter.WorldTransform.GetPosition ();
			screenSpacePlayerPosition = appDelegate.Scene.ProjectPoint (playerPosition);

			// Update the SP position of the score label
			CGPoint pt = ScoreLabelLocation;

			worldSpaceLabelScorePosition = appDelegate.Scene.UnprojectPoint (new SCNVector3 (pt.X, pt.Y, screenSpacePlayerPosition.Z));
		}

		public void CollideWithLava ()
		{
			if (HitByLavaReset == true)
				return;

			PlayerCharacter.InRunAnimation = false;

			GameSimulation.Sim.PlaySound ("ack.caf");

			// Blink for a second
			SCNAction blinkOffAction = SCNAction.FadeOut (0.15f);
			SCNAction blinkOnAction = SCNAction.FadeIn (0.15f);
			SCNAction cycle = SCNAction.Sequence (new SCNAction[] { blinkOffAction, blinkOnAction });
			SCNAction repeatCycle = SCNAction.RepeatAction (cycle, 17);

			HitByLavaReset = true;

			PlayerCharacter.RunAction (repeatCycle, () => {
				TimeAlongPath = 0;
				PlayerCharacter.Position = LocationAlongPath (TimeAlongPath);
				PlayerCharacter.Rotation = GetPlayerDirectionFromCurrentPosition ();
				HitByLavaReset = false;
			});
		}

		public void ResetLevel ()
		{
			Score = 0;
			SecondsRemaining = 120;
			CoinsCollected = 0;
			BananasCollected = 0;

			TimeAlongPath = 0;
			PlayerCharacter.Position = LocationAlongPath (TimeAlongPath);
			PlayerCharacter.Rotation = GetPlayerDirectionFromCurrentPosition ();
			HitByLavaReset = false;

			// Remove dynamic objects from the level.
			SCNTransaction.Begin ();

			if (Bananas != null) {
				foreach (SCNNode b in Coconuts)
					b.RemoveFromParentNode ();
			}

			if (Bananas != null) {
				foreach (SCNNode b in Bananas)
					b.RemoveFromParentNode ();
			}

			if (LargeBananas != null) {
				foreach (SCNNode largeBanana in LargeBananas)
					largeBanana.RemoveFromParentNode ();
			}

			SCNTransaction.Commit ();

			// Add dynamic objects to the level, like bananas and large bananas
			Bananas = new List<SCNNode> ();
			Coconuts = new List<Coconut> ();

			for (int i = 0; i < 10; i++) {
				SCNNode banana = CreateBanana ();
				RootNode.Add (banana);
				SCNVector3 location = LocationAlongPath ((i + 1) / 20.0f - 0.01f);
				location.Y += 50;
				banana.Position = location;
				Bananas.Add (banana);
			}

			LargeBananas = new List<SCNNode> ();

			for (int i = 0; i < 6; i++) {
				SCNNode largeBanana = CreateLargeBanana ();
				RootNode.AddChildNode (largeBanana);
				SCNVector3 location = LocationAlongPath (MathUtils.RandomPercent ());
				location.Y += 50;
				largeBanana.Position = location;
				LargeBananas.Add (largeBanana);
			}

			GameSimulation.Sim.PlayMusic ("music.caf");
			GameSimulation.Sim.PlayMusic ("night.caf");
		}

		SCNNode CreateLargeBanana ()
		{
			//Create model
			if (largeBananaCollectable == null) {
				var node = GameSimulation.LoadNodeWithName ("banana", GameSimulation.PathForArtResource ("level/banana.dae"));
				float scaleMode = 0.5f * 10 / 4;
				node.Scale = new SCNVector3 (scaleMode, scaleMode, scaleMode);


				SCNSphere sphereGeometry = SCNSphere.Create (100);
				SCNPhysicsShape physicsShape = SCNPhysicsShape.Create (sphereGeometry, new SCNPhysicsShapeOptions ());
				node.PhysicsBody = SCNPhysicsBody.CreateBody (SCNPhysicsBodyType.Kinematic, physicsShape);

				// Only collide with player and ground
				node.PhysicsBody.CollisionBitMask = GameCollisionCategory.Player | GameCollisionCategory.Ground;
				// Declare self in the banana category
				node.PhysicsBody.CategoryBitMask = GameCollisionCategory.Coin;

				// Rotate forever.
				SCNAction rotateCoin = SCNAction.RotateBy (0f, 8f, 0f, 2f);
				SCNAction repeat = SCNAction.RepeatActionForever (rotateCoin);

				node.Rotation = new SCNVector4 (0f, 1f, 0f, (nfloat)Math.PI / 2);
				node.RunAction (repeat);

				largeBananaCollectable = node;
			}

			SCNNode nodeSparkle = largeBananaCollectable.Clone ();

			SCNParticleSystem newSystem = GameSimulation.LoadParticleSystemWithName ("sparkle");
			nodeSparkle.AddParticleSystem (newSystem);

			return nodeSparkle;
		}

		SCNNode CreateBanana ()
		{
			//Create model
			if (bananaCollectable == null) {
				bananaCollectable = GameSimulation.LoadNodeWithName ("banana", GameSimulation.PathForArtResource ("level/banana.dae"));
				bananaCollectable.Scale = new SCNVector3 (0.5f, 0.5f, 0.5f);


				SCNSphere sphereGeometry = SCNSphere.Create (40);
				SCNPhysicsShape physicsShape = SCNPhysicsShape.Create (sphereGeometry, new SCNPhysicsShapeOptions ());
				bananaCollectable.PhysicsBody = SCNPhysicsBody.CreateBody (SCNPhysicsBodyType.Kinematic, physicsShape);

				// Only collide with player and ground
				bananaCollectable.PhysicsBody.CollisionBitMask = GameCollisionCategory.Player | GameCollisionCategory.Ground;
				// Declare self in the banana category
				bananaCollectable.PhysicsBody.CategoryBitMask = GameCollisionCategory.Banana;

				// Rotate and Hover forever.
				bananaCollectable.Rotation = new SCNVector4 (0.5f, 1f, 0.5f, -(nfloat)Math.PI / 4);
				SCNAction idleHoverGroupAction = SCNAction.Group (new SCNAction[] { BananaIdleAction, HoverAction });
				SCNAction repeatForeverAction = SCNAction.RepeatActionForever (idleHoverGroupAction);
				bananaCollectable.RunAction (repeatForeverAction);
			}

			return bananaCollectable.Clone ();
		}

		void SetupPathColliders ()
		{
			// Collect all the nodes that start with path_ under the dummy_front object.
			// Set those objects as Physics category ground and create a static concave mesh collider.
			// The simulation will use these as the ground to walk on.
			SCNNode front = RootNode.FindChildNode ("dummy_front", true);
			foreach (var fronChild in front.ChildNodes)
				foreach (var child in fronChild.ChildNodes) {
					if (child.Name.Contains ("path_")) {
						//the geometry is attached to the first child node of the node named path
						SCNNode path = child.ChildNodes [0];
						var options = new SCNPhysicsShapeOptions { ShapeType = SCNPhysicsShapeType.ConcavePolyhedron };
						path.PhysicsBody = SCNPhysicsBody.CreateBody (SCNPhysicsBodyType.Static, SCNPhysicsShape.Create (path.Geometry, options));
						path.PhysicsBody.CategoryBitMask = GameCollisionCategory.Ground;
					}
				}
		}

		List<SCNNode> CollectSortedPathNodes ()
		{
			// Gather all the children under the dummy_master
			// Sort left to right, in the world.
			SCNNode pathNode = RootNode.FindChildNode ("dummy_master", true);
			return pathNode.OrderBy (x => x.Position.X).ToList<SCNNode> ();
		}

		void ConvertPathNodesIntoPathPositions ()
		{
			// Walk the path, sampling every little bit, creating a path to follow.
			// We use this path to move along left to right and right to left.
			List<SCNNode> sortedNodes = CollectSortedPathNodes ();

			PathPositions = new List<SCNVector3> ();
			PathPositions.Add (SCNVector3.Zero);

			foreach (SCNNode d in sortedNodes)
				if (d.Name.Contains ("dummy_path_")) {
					PathPositions.Add (d.Position);
				}

			PathPositions.Add (SCNVector3.Zero);
		}

		void ResamplePathPositions ()
		{
			// Calc the phatom end control point.
			SCNVector3 controlPointA = PathPositions [PathPositions.Count - 2];
			SCNVector3 controlPointB = PathPositions [PathPositions.Count - 3];
			SCNVector3 controlPoint;

			controlPoint.X = controlPointA.X + (controlPointA.X - controlPointB.X);
			controlPoint.Y = controlPointA.Y + (controlPointA.Y - controlPointB.Y);
			controlPoint.Z = controlPointA.Z + (controlPointA.Z - controlPointB.Z);

			PathPositions [PathPositions.Count - 1] = controlPoint;

			// Calc the phatom begin control point.
			controlPointA = PathPositions [1];
			controlPointB = PathPositions [2];

			controlPoint.X = controlPointA.X + (controlPointA.X - controlPointB.X);
			controlPoint.Y = controlPointA.Y + (controlPointA.Y - controlPointB.Y);
			controlPoint.Z = controlPointA.Z + (controlPointA.Z - controlPointB.Z);
			PathPositions [0] = controlPoint;

			var newPath = new List<SCNVector3> ();
			SCNVector3 lastPosition;
			float minDistanceBetweenPoints = 10.0f;
			float steps = 10000f;

			for (int i = 0; i < steps; i++) {
				float t = i / steps;
				SCNVector3 currentPostion = LocationAlongPath (t);
				if (i == 0) {
					newPath.Add (currentPostion);
					lastPosition = currentPostion;
				} else {
					var sub = SCNVector3.Subtract (lastPosition, currentPostion);
					nfloat dist = sub.Length;

					if (dist > minDistanceBetweenPoints) {
						newPath.Add (currentPostion);
						lastPosition = currentPostion;
					}
				}
			}

			// Last Step. Return the path position array for our pathing system to query.
			PathPositions = newPath;
		}

		void CalculatePathPositions ()
		{
			SetupPathColliders ();
			ConvertPathNodesIntoPathPositions ();
			ResamplePathPositions ();
		}

		SCNVector3 LocationAlongPath (nfloat percent)
		{
			if (PathPositions.Count <= 3)
				return SCNVector3.Zero;

			int numSections = PathPositions.Count - 3;

			nfloat dist = percent * numSections;

			int currentPointIndex = (int)Math.Min ((uint)Math.Floor (dist), numSections - 1);
			dist -= currentPointIndex;
			var a = new SCNVector3 (PathPositions [currentPointIndex]);
			var b = new SCNVector3 (PathPositions [currentPointIndex + 1]);
			var c = new SCNVector3 (PathPositions [currentPointIndex + 2]);
			var d = new SCNVector3 (PathPositions [currentPointIndex + 3]);

			var location = new SCNVector3 ();
			location.X = CatmullRomValue (a.X, b.X, c.X, d.X, dist);
			location.Y = CatmullRomValue (a.Y, b.Y, c.Y, d.Y, dist);
			location.Z = CatmullRomValue (a.Z, b.Z, c.Z, d.Z, dist);

			return location;
		}

		nfloat CatmullRomValue (nfloat a, nfloat b, nfloat c, nfloat d, nfloat dist)
		{
			return (nfloat)(((-a + 3.0f * b - 3.0f * c + d) * (dist * dist * dist)) +
			((2.0f * a - 5.0f * b + 4.0f * c - d) * (dist * dist)) +
			((-a + c) * dist) + (2.0f * b)) * 0.5f; 
		}

		SCNVector4 GetDirectionFromPosition (SCNVector3 currentPosition)
		{
			SCNVector3 target = LocationAlongPath (TimeAlongPath - 0.05f);
			SCNMatrix4 lookAt = SCNMatrix4.LookAt (currentPosition, target, new SCNVector3 (0f, 1f, 0f));
			SCNQuaternion q = lookAt.ToQuaternion ();

			nfloat angle = 0;
			q.ToAxisAngle (out target, out angle);

			if (PlayerCharacter.PlayerWalkDirection == WalkDirection.Left)
				angle -= (nfloat)Math.PI;

			return  new SCNVector4 (0f, 1f, 0f, angle);
		}

		SCNVector4 GetPlayerDirectionFromCurrentPosition ()
		{
			return GetDirectionFromPosition (PlayerCharacter.Position);
		}

		void CreateSwingingTorch ()
		{
			var scnene = SCNScene.FromFile ("torch", GameSimulation.PathForArtResource ("level/"), new NSDictionary ());
			SCNNode torchSwing = scnene.RootNode.FindChildNode ("dummy_master", true);
			RootNode.AddChildNode (torchSwing);
		}

		void CreateLavaAnimation ()
		{
			var dummyFront = RootNode.FindChildNode ("dummy_front", true);
			var lavaNodes = new List<SCNNode> ();
			foreach (var dummyFrontNode in dummyFront.ChildNodes) {
				foreach (var lavaNode in dummyFrontNode.ChildNodes)
					if (!string.IsNullOrEmpty (lavaNode.Name) && lavaNode.Name.Contains ("lava_0"))
						lavaNodes.Add (lavaNode);
			}

			foreach (SCNNode lava in lavaNodes) {
				var childrenWithGeometry = new List<SCNNode> ();

				foreach (var child in lava.ChildNodes) {
					if (child.Geometry != null)
						childrenWithGeometry.Add (child);
				}

				if (childrenWithGeometry.Count == 0)
					continue;

				SCNNode lavaGeometry = childrenWithGeometry [0];
				lavaGeometry.PhysicsBody = SCNPhysicsBody.CreateBody (SCNPhysicsBodyType.Static,
					SCNPhysicsShape.Create (lavaGeometry.Geometry, new SCNPhysicsShapeOptions { ShapeType = SCNPhysicsShapeType.ConcavePolyhedron }));
				lavaGeometry.PhysicsBody.CategoryBitMask = GameCollisionCategory.Lava;
				lavaGeometry.CategoryBitMask = NodeCategory.Lava;

				string shaderCode = "uniform float speed;\n" +
				                    "#pragma body\n" +
				                    "_geometry.texcoords[0] += vec2(sin(_geometry.position.z*0.1 + u_time * 0.1) * 0.1, -1.0* 0.05 * u_time);\n";

				lavaGeometry.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointGeometry = shaderCode };
			}
		}

		SCNNode CreateTorchNode ()
		{
			SCNGeometry geometry = SCNBox.Create (20f, 100f, 20f, 0f);
			geometry.FirstMaterial.Diffuse.Contents = AppKit.NSColor.Brown;
			var template = new SCNNode {
				Geometry = geometry
			};

			var particleEmitter = new SCNNode {
				Position = new SCNVector3 (0f, 50f, 0f)
			};

			SCNParticleSystem fire = GameSimulation.LoadParticleSystemWithName ("torch", "spark");
			particleEmitter.AddParticleSystem (fire);
			particleEmitter.Light = TorchLight;

			template.AddChildNode (particleEmitter);
			return template;
		}

		void AddMonkeyAtPosition (SCNVector3 worldPos, nfloat rotation)
		{
			if (Monkeys == null)
				Monkeys = new List<MonkeyCharacter> ();

			SCNNode palmTree = CreateMonkeyPalmTree ();
			palmTree.Position = worldPos;
			palmTree.Rotation = new SCNVector4 (0f, 1f, 0f, rotation);
			RootNode.AddChildNode (palmTree);

			MonkeyCharacter monkey = (MonkeyCharacter)palmTree.FindChildNode ("monkey", true);

			if (monkey != null)
				Monkeys.Add (monkey);
		}

		SCNNode CreateMonkeyPalmTree ()
		{
			SCNNode palmTreeProtoObject = null;

			string palmTreeDae = "art.scnassets/characters/monkey/monkey_palm_tree.dae";
			palmTreeProtoObject = GameSimulation.LoadNodeWithName ("PalmTree", palmTreeDae);

			SCNNode monkeyNode = GameSimulation.LoadNodeWithName (string.Empty, "art.scnassets/characters/monkey/monkey_skinned.dae");

			var monkey = new MonkeyCharacter (monkeyNode);
			monkey.CreateAnimations ();
			palmTreeProtoObject.AddChildNode (monkey);

			return palmTreeProtoObject;
		}

		void AnimateDynamicNodes ()
		{
			SCNNode dummyFront = RootNode.FindChildNode ("dummy_front", true);
			var dynamicNodesWithVertColorAnimation = new List<SCNNode> ();

			foreach (var split in dummyFront.ChildNodes)
				foreach (var splitElement in split.ChildNodes)
					foreach (var child in splitElement.ChildNodes) {

						var range = new NSRange ();
						if (!string.IsNullOrEmpty (child.ParentNode.Name)) {
							int index = child.ParentNode.Name.LastIndexOf ("vine");
							range = new NSRange (index, 4);
						}

						if (child.Geometry != null && child.Geometry.GetGeometrySourcesForSemantic (SCNGeometrySourceSemantic.Color) != null
						    && range.Location != NSRange.NotFound) {
							dynamicNodesWithVertColorAnimation.Add (child);
						}
					}

			string shaderCode = "uniform float timeOffset;\n" +
			                    "#pragma body\n" +
			                    "float speed = 20.05;\n" +
			                    "_geometry.position.xyz += (speed * sin(u_time + timeOffset) * _geometry.color.rgb);\n";

			foreach (SCNNode dynamicNode in dynamicNodesWithVertColorAnimation) {
				dynamicNode.Geometry.ShaderModifiers = new SCNShaderModifiers { EntryPointGeometry = shaderCode };
				var explodeAnimation = new CABasicAnimation {
					KeyPath = "timeOffset",
					Duration = 2.0,
					RepeatCount = float.MaxValue,
					AutoReverses = true,
					To = new NSNumber (MathUtils.RandomPercent ()),
					TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut)
				};

				dynamicNode.Geometry.AddAnimation (explodeAnimation, (NSString)"sway");
			}
		}

		void DoGameOver ()
		{
			PlayerCharacter.InRunAnimation = false;
			GameSimulation.Sim.SetGameState (GameState.PostGame);
		}

		void MoveCharacterAlongPathWith (double deltaTime, GameState currentState)
		{
			if (PlayerCharacter.IsWalking == true) {
				if (currentState == GameState.InGame) {
					nfloat walkSpeed = PlayerCharacter.WalkSpeed;
					if (PlayerCharacter.Jumping == true)
						walkSpeed += PlayerCharacter.JumpBoost;

					TimeAlongPath += ((nfloat)deltaTime * walkSpeed * (PlayerCharacter.PlayerWalkDirection == WalkDirection.Right ? 1 : -1));

					// limit how far the player can go in left and right directions.
					if (TimeAlongPath < 0.0f)
						TimeAlongPath = 0.0f;
					else if (TimeAlongPath > 1.0f)
						TimeAlongPath = 1.0f;

					SCNVector3 newPosition = LocationAlongPath (TimeAlongPath);
					PlayerCharacter.Position = new SCNVector3 (newPosition.X, PlayerCharacter.Position.Y, newPosition.Z);

					if (TimeAlongPath >= 1.0)
						DoGameOver ();

				} else {
					PlayerCharacter.InRunAnimation = false;
				}
			}
		}

		void UpdateSunLightPosition ()
		{
			SCNVector3 lightPos = lightOffsetFromCharacter;
			SCNVector3 charPos = PlayerCharacter.Position;
			lightPos.X += charPos.X;
			lightPos.Y += charPos.Y;
			lightPos.Z += charPos.Z;
			SunLight.Position = lightPos;
		}
	}
}

