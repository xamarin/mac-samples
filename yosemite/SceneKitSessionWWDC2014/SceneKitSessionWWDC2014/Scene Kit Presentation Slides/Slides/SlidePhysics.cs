using System;
using System.Collections.Generic;

using AppKit;
using OpenGL;
using SceneKit;
using Foundation;
using CoreFoundation;

namespace SceneKitSessionWWDC2014
{
	public class SlidePhysics : Slide
	{
		private NSTimer Timer { get; set; }

		private List<SCNNode> Dices { get; set; }

		private List<SCNNode> Balls { get; set; }

		private List<SCNNode> Shapes { get; set; }

		private List<SCNNode> Meshes { get; set; }

		private List<SCNNode> Hinges { get; set; }

		private List<SCNNode> KinematicItems { get; set; }

		private int Step { get; set; }

		private static SCNGeometry DiceMesh { get; set; }

		private const int MIDDLE_Z = 0;

		private static nfloat RandFloat (double min, double max)
		{
			var random = new Random ((int)DateTime.Now.Ticks);
			return (nfloat)(min + (max - min) * random.NextDouble ());
		}

		static float NextFloat (Random random)
		{
			double mantissa = (random.NextDouble () * 2.0) - 1.0;
			double exponent = Math.Pow (2.0, random.Next (-126, 128));
			return (float)(mantissa * exponent);
		}

		public override int NumberOfSteps ()
		{
			return 20;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			Shapes = new List<SCNNode> ();
			Dices = new List<SCNNode> ();
			Balls = new List<SCNNode> ();
			Meshes = new List<SCNNode> ();
			Hinges = new List<SCNNode> ();
			KinematicItems = new List<SCNNode> ();

			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Physics");

			TextManager.AddBulletAtLevel ("Nodes are automatically animated by SceneKit", 0);
			TextManager.AddBulletAtLevel ("Same approach as SpriteKit", 0);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			Step = index;

			switch (index) {
			case 0:
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				TextManager.SetSubtitle ("SCNPhysicsBody");

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				TextManager.AddBulletAtLevel ("Dynamic Bodies", 0);

					// Add some code
				TextManager.AddCode ("#// Make a node dynamic\n"
				+ "aNode.#physicsBody# = [SCNPhysicsBody #dynamicBody#];#");

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			case 2:
				//add a cube
				var worldPos = GroundNode.ConvertPositionToNode (new SCNVector3 (0, 12, 2), null);
				var dice = CreateBlock (worldPos, new SCNVector3 (1.5f, 1.5f, 1.5f));
				dice.PhysicsBody = null; //wait!
				dice.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI / 4) * 0.5f);
				dice.Scale = new SCNVector3 (0.001f, 0.001f, 0.001f);

				((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (dice);
				SCNTransaction.Begin (); 
				SCNTransaction.AnimationDuration = 0.75f;
				dice.Scale = new SCNVector3 (2, 2, 2);
				SCNTransaction.Commit ();

				Dices.Add (dice);
				break;
			case 3:
				foreach (var node in Dices)
					node.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
				break;
			case 4:
				PresentDices (presentationViewController);
				break;
			case 5:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Code);

				TextManager.AddBulletAtLevel ("Manipulate with forces", 0);

				// Add some code
				TextManager.AddCode ("#// Apply an impulse\n"
				+ "[aNode.physicsBody #applyForce:#aVector3 #atPosition:#aVector3 #impulse:#YES];#");

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			case 6:
				// remove dices
				var center = new SCNVector3 (0, -5, 20);
				center = GroundNode.ConvertPositionToNode (center, null);

				Explosion (center, Dices);

				var popTime = new DispatchTime (DispatchTime.Now, (long)(1 * Utils.NSEC_PER_SEC));
				DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
					TextManager.FlipOutText (SlideTextManager.TextType.Code);
					TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

					TextManager.AddBulletAtLevel ("Static Bodies", 0);
					TextManager.AddCode ("#// Make a node static\n"
					+ "aNode.#physicsBody# = [SCNPhysicsBody #staticBody#];#");
					TextManager.FlipInText (SlideTextManager.TextType.Bullet);
					TextManager.FlipInText (SlideTextManager.TextType.Code);
				});
				break;
			case 7:
				PresentWalls (presentationViewController);
				break;
			case 8:
				PresentBalls (presentationViewController);
				break;
			case 9:
				//remove walls
				var walls = new List<SCNNode> ();
				GroundNode.EnumerateChildNodes (delegate(SCNNode node, out bool stop) {
					stop = false;
					if (node.Name == "container-wall") {
						node.RunAction (SCNAction.Sequence (new SCNAction [] {
							SCNAction.MoveBy (new SCNVector3 (0, -2, 0), 0.5),
							SCNAction.RemoveFromParentNode ()
						}));
						walls.Add (node);
					}
					return stop;
				});
				break;
			case 10:
				// remove balls
				center = new SCNVector3 (0, -5, 5);
				center = GroundNode.ConvertPositionToNode (center, null);
				Explosion (center, Balls);

				popTime = new DispatchTime (DispatchTime.Now, (long)(0.5 * Utils.NSEC_PER_SEC));
				DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
					TextManager.FlipOutText (SlideTextManager.TextType.Code);
					TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

					TextManager.AddBulletAtLevel ("Kinematic Bodies", 0);
					TextManager.AddCode ("#// Make a node kinematic\n"
					+ "aNode.#physicsBody# = [SCNPhysicsBody #kinematicBody#];#");
					TextManager.FlipInText (SlideTextManager.TextType.Bullet);
					TextManager.FlipInText (SlideTextManager.TextType.Code);
				});
				break;
			case 11:
				var boxNode = SCNNode.Create ();
				boxNode.Geometry = SCNBox.Create (10, 0.2f, 10, 0);
				boxNode.Position = new SCNVector3 (0, 5, MIDDLE_Z);
				boxNode.Geometry.FirstMaterial.Emission.Contents = NSColor.DarkGray;
				boxNode.PhysicsBody = SCNPhysicsBody.CreateKinematicBody ();
				boxNode.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, 0, NMath.PI * 2, 2.0)));
				GroundNode.AddChildNode (boxNode);
				KinematicItems.Add (boxNode);

				var invisibleWall = SCNNode.Create ();
				invisibleWall.Geometry = SCNBox.Create (4, 40, 10, 0);
				invisibleWall.Position = new SCNVector3 (-7, 0, MIDDLE_Z);
				invisibleWall.Geometry.FirstMaterial.Transparency = 0;
				invisibleWall.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
				GroundNode.AddChildNode (invisibleWall);
				KinematicItems.Add (invisibleWall);

				invisibleWall = (SCNNode)invisibleWall.Copy ();
				invisibleWall.Position = new SCNVector3 (7, 0, MIDDLE_Z);
				GroundNode.AddChildNode (invisibleWall);
				KinematicItems.Add (invisibleWall);

				invisibleWall = (SCNNode)invisibleWall.Copy ();
				invisibleWall.Geometry = SCNBox.Create (10, 40, 4, 0);
				invisibleWall.Geometry.FirstMaterial.Transparency = 0;
				invisibleWall.Position = new SCNVector3 (0, 0, MIDDLE_Z - 7);
				invisibleWall.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
				GroundNode.AddChildNode (invisibleWall);
				KinematicItems.Add (invisibleWall);

				invisibleWall = (SCNNode)invisibleWall.Copy ();
				invisibleWall.Position = new SCNVector3 (0, 0, MIDDLE_Z + 7);
				GroundNode.AddChildNode (invisibleWall);
				KinematicItems.Add (invisibleWall);


				for (int i = 0; i < 100; i++) {
					var ball = SCNNode.Create ();
					worldPos = boxNode.ConvertPositionToNode (new SCNVector3 (RandFloat (-4, 4), RandFloat (10, 30), RandFloat (-1, 4)), null);
					ball.Position = worldPos;
					ball.Geometry = SCNSphere.Create (0.5f);
					ball.Geometry.FirstMaterial.Diffuse.Contents = NSColor.Cyan;
					ball.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
					((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (ball);

					KinematicItems.Add (ball);
				}
				break;
			case 12:
				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.SetSubtitle ("SCNPhysicsShape");
				TextManager.AddCode ("#// Configure the physics shape\n\n"
				+ "aNode.physicsBody.#physicsShape# = \n\t[#SCNPhysicsShape# shapeWithGeometry:aGeometry options:options];#");
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);

				KinematicItems[0].RunAction (SCNAction.Sequence (new SCNAction[] {
					SCNAction.FadeOut (0.5),
					SCNAction.RemoveFromParentNode ()
				}));
				for (int i = 1; i < 5; i++)
					KinematicItems[i].RemoveFromParentNode ();

				KinematicItems = null;
				break;
			case 13:
				//add meshes
				PresentMeshes (presentationViewController);
				break;
			case 14:
				// remove meshes
				center = new SCNVector3 (0, -5, 20);
				center = GroundNode.ConvertPositionToNode (center, null);
				Explosion (center, Meshes);
				break;
			case 15:
				// add shapes
				PresentPrimitives (presentationViewController);
				break;
			case 16:
				// remove shapes
				center = new SCNVector3 (0, -5, 20);
				center = GroundNode.ConvertPositionToNode (center, null);
				Explosion (center, Shapes);

				popTime = new DispatchTime (DispatchTime.Now, (long)(0.5 * Utils.NSEC_PER_SEC));
				DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
					TextManager.FlipOutText (SlideTextManager.TextType.Code);
					TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
					TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

					TextManager.SetSubtitle ("SCNPhysicsBehavior");
					TextManager.AddCode ("#// setup a physics behavior\n\n"
						+ "#SCNPhysicsHingeJoint# *joint = [SCNPhysicsHingeJoint\n\n" 
						+ "jointWithBodyA:#nodeA.physicsBody# axisA:[...] anchorA:[...]\n\n"
						+ "bodyB:#nodeB.physicsBody# axisB:[...] anchorB:[...]];\n\n\n"
						+ "[scene.#physicsWorld# addBehavior:joint];#");

					TextManager.FlipInText (SlideTextManager.TextType.Bullet);
					TextManager.FlipInText (SlideTextManager.TextType.Subtitle);
					TextManager.FlipInText (SlideTextManager.TextType.Code);
				});
				break;
			case 17:
				//add meshes
				PresentHinge (presentationViewController);
				break;
			case 18:
				//remove constraints
				((SCNView)presentationViewController.View).Scene.PhysicsWorld.RemoveAllBehaviors ();

				foreach (var node in Hinges)
					node.RunAction (SCNAction.Sequence (new SCNAction[] { SCNAction.Wait (3.0), SCNAction.FadeOut (0.5), SCNAction.RemoveFromParentNode () }));

				break;
			case 19:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.FlipOutText (SlideTextManager.TextType.Code);

				TextManager.SetSubtitle ("More...");

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				TextManager.AddBulletAtLevel ("SCNPhysicsField", 0);
				TextManager.AddBulletAtLevel ("SCNPhysicsVehicle", 0);

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			((SCNView)presentationViewController.View).Scene.PhysicsWorld.Speed = 0;

			foreach (var node in Dices)
				node.RemoveFromParentNode ();

			foreach (var node in Balls)
				node.RemoveFromParentNode ();

			foreach (var node in Shapes)
				node.RemoveFromParentNode ();

			foreach (var node in Meshes)
				node.RemoveFromParentNode ();

			foreach (var node in Hinges)
				node.RemoveFromParentNode ();
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			((SCNView)presentationViewController.View).Scene.PhysicsWorld.Speed = 2;
		}

		private void Explosion (SCNVector3 center, List<SCNNode> nodes)
		{
			foreach (var node in nodes) {
				var position = node.PresentationNode.Position;
				var dir = SCNVector3.Subtract (position, center);

				var force = (nfloat)25.0f;
				var distance = dir.Length;

				dir = SCNVector3.Multiply (dir, force / NMath.Max (0.01f, distance));

				node.PhysicsBody.ApplyForce (dir, new SCNVector3 (RandFloat (-0.2, 0.2), RandFloat (-0.2, 0.2), RandFloat (-0.2, 0.2)), true);

				node.RunAction (SCNAction.Sequence (new SCNAction[] {
					SCNAction.Wait (2),
					SCNAction.FadeOut (0.5f),
					SCNAction.RemoveFromParentNode ()
				}));
			}
		}

		private SCNNode CreatePoolBall (SCNVector3 position)
		{
			var model = SCNNode.Create ();
			model.Position = position;
			model.Geometry = SCNSphere.Create (0.7f);
			model.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("Scenes.scnassets/pool/pool_8", "png"));
			model.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
			return model;
		}

		private SCNGeometry CreateBlockMesh (SCNVector3 size)
		{
			var diceMesh = SCNBox.Create (size.X, size.Y, size.Z, 0.05f * size.X);

			diceMesh.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/texture", "png"));
			diceMesh.FirstMaterial.Diffuse.MipFilter = SCNFilterMode.Linear;
			diceMesh.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
			diceMesh.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;

			return diceMesh;
		}

		private SCNNode CreateBlock (SCNVector3 position, SCNVector3 size)
		{
			if (DiceMesh == null) {
				DiceMesh = CreateBlockMesh (size);
			}

			var model = SCNNode.Create ();
			model.Position = position;
			model.Geometry = DiceMesh;
			model.PhysicsBody =  SCNPhysicsBody.CreateDynamicBody ();

			return model;
		}

		private void PresentDices (PresentationViewController presentationViewController)
		{
			var count = 200;
			var spread = 6;

			// drop rigid bodies cubes
			var intervalTime = 5.0 / count;
			var remainingCount = count;

			Timer = NSTimer.CreateRepeatingTimer (intervalTime, delegate(NSTimer obj) {

				if (Step > 4) {
					Timer.Invalidate ();
					return;
				}

				SCNTransaction.Begin ();

				var worldPos = GroundNode.ConvertPositionToNode (new SCNVector3 (0, 30, 0), null);

				var dice = CreateBlock (worldPos, new SCNVector3 (1.5f, 1.5f, 1.5f));

				//add to scene
				((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (dice);

				dice.PhysicsBody.Velocity = new SCNVector3 (RandFloat (-spread, spread), -10, RandFloat (-spread, spread));
				dice.PhysicsBody.AngularVelocity = new SCNVector4 (RandFloat (-1, 1), RandFloat (-1, 1), RandFloat (-1, 1), RandFloat (-3, 3));

				SCNTransaction.Commit ();

				Dices.Add (dice);

				// ensure we stop firing
				if (--remainingCount < 0)
					Timer.Invalidate ();
			});

			NSRunLoop.Current.AddTimer (Timer, NSRunLoopMode.Default);
		}

		private void PresentWalls (PresentationViewController presentationViewController)
		{
			//add spheres and container
			var height = 2;
			var width = 1;

			var count = 3;
			var margin = 2;

			var totalWidth = count * (margin + width);

			var blockMesh = CreateBlockMesh (new SCNVector3 (width, height, width));

			for (int i = 0; i < count; i++) {
				//create a static block
				var wall = SCNNode.Create ();
				wall.Position = new SCNVector3 ((i - (count / 2)) * (width + margin), -height / 2, totalWidth / 2);
				wall.Geometry = blockMesh;
				wall.Name = "container-wall";
				wall.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();

				GroundNode.AddChildNode (wall);
				wall.RunAction (SCNAction.MoveBy (new SCNVector3 (0, height, 0), 0.5f));

				//one more
				wall = (SCNNode)wall.Copy ();
				wall.Position = new SCNVector3 ((i - (count / 2)) * (width + margin), -height / 2, -totalWidth / 2);
				GroundNode.AddChildNode (wall);

				// one more
				wall = (SCNNode)wall.Copy ();
				wall.Position = new SCNVector3 (totalWidth / 2, -height / 2, (i - (count / 2)) * (width + margin));
				GroundNode.AddChildNode (wall);

				//one more
				wall = (SCNNode)wall.Copy ();
				wall.Position = new SCNVector3 (-totalWidth / 2, -height / 2, (i - (count / 2)) * (width + margin));
				GroundNode.AddChildNode (wall);
			}
		}

		private void PresentBalls (PresentationViewController presentationViewController)
		{
			var count = 150;

			for (int i = 0; i < count; ++i) {
				var worldPos = GroundNode.ConvertPositionToNode (new SCNVector3 (RandFloat (-5, 5), RandFloat (25, 30), RandFloat (-5, 5)), null);

				var ball = CreatePoolBall (worldPos);

				((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (ball);
				Balls.Add (ball);
			}
		}

		private void PresentPrimitives (PresentationViewController presentationViewController)
		{
			var count = 100;
			var spread = 0.0f;

			// create a cube with a sphere shape
			for (int i = 0; i < count; ++i) {
				var model = SCNNode.Create ();
				model.Position = GroundNode.ConvertPositionToNode (new SCNVector3 (RandFloat (-1, 1), RandFloat (30, 50), RandFloat (-1, 1)), null);
				model.EulerAngles = new SCNVector3 (RandFloat (0, NMath.PI * 2), RandFloat (0, NMath.PI * 2), RandFloat (0, NMath.PI * 2));

				var size = new SCNVector3 (RandFloat (1.0, 1.5), RandFloat (1.0, 1.5), RandFloat (1.0, 1.5));
				var random = new Random ((int)DateTime.Now.Ticks);
				int geometryIndex = random.Next (0, 7);
				switch (geometryIndex) {
				case 0: // Box
					model.Geometry = SCNBox.Create (size.X, size.Y, size.Z, 0);
					break;
				case 1: // Pyramid
					model.Geometry = SCNPyramid.Create (size.X, size.Y, size.Z);
					break;
				case 2: // Sphere
					model.Geometry = SCNSphere.Create (size.X);
					break;
				case 3: // Cylinder
					model.Geometry = SCNCylinder.Create (size.X, size.Y);
					break;
				case 4: // Tube
					model.Geometry = SCNTube.Create (size.X, size.X + size.Z, size.Y);
					break;
				case 5: // Capsule
					model.Geometry = SCNCapsule.Create (size.X, size.Y + 2 * size.X);
					break;
				case 6: // Torus
					model.Geometry = SCNTorus.Create (size.X, NMath.Min (size.X, size.Y) / 2);
					break;
				default:
					break;
				}

				model.Geometry.FirstMaterial.Multiply.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/texture", "png"));

				model.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
				model.PhysicsBody.Velocity = new SCNVector3 (RandFloat (-spread, spread), -10, RandFloat (-spread, spread));
				model.PhysicsBody.AngularVelocity = new SCNVector4 (RandFloat (-1, 1), RandFloat (-1, 1), RandFloat (-1, 1), RandFloat (-3, 3));

				Shapes.Add (model);

				((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (model);
			}
		}

		private void PresentMeshes (PresentationViewController presentationViewController)
		{
			// add meshes
			var container = SCNNode.Create ();
			var black = Utils.SCAddChildNode (container, "teapot", "Scenes.scnassets/lod/midResTeapot.dae", 5);

			int count = 100;
			for (int i = 0; i < count; ++i) {
				var worldPos = GroundNode.ConvertPositionToNode (new SCNVector3 (RandFloat (-1, 1), RandFloat (30, 50), RandFloat (-1, 1)), null);

				var node = (SCNNode)black.Copy ();
				node.Position = worldPos;
				node.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
				node.PhysicsBody.Friction = 0.5f;

				((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (node);
				Meshes.Add (node);
			}
		}

		private void PresentHinge (PresentationViewController presentationViewController)
		{
			var count = 10.0f;

			var material = SCNMaterial.Create ();
			material.Diffuse.Contents = NSColor.White;
			material.Specular.Contents = NSColor.White;
			material.LocksAmbientWithDiffuse = true;

			var cubeWidth = 10.0f/count;
			var cubeHeight = 0.2f;
			var cubeLength = 5.0f;
			var offset = 0;
			var height = 5 + count * cubeWidth;

			SCNNode oldModel = null;
			for (int i = 0; i < count; ++i) {
				var model = SCNNode.Create ();

				var worldtr = GroundNode.ConvertTransformToNode (SCNMatrix4.CreateTranslation (-offset + cubeWidth * i, height, 5), null);

				model.Transform = worldtr;

				model.Geometry = SCNBox.Create (cubeWidth, cubeHeight, cubeLength, 0);
				model.Geometry.FirstMaterial = material;

				var body = SCNPhysicsBody.CreateDynamicBody ();
				body.Restitution = 0.6f;
				model.PhysicsBody = body;

				((SCNView)presentationViewController.View).Scene.RootNode.AddChildNode (model);

				var joint = SCNPhysicsHingeJoint.Create (model.PhysicsBody, new SCNVector3 (0, 0, 1), new SCNVector3 (-cubeWidth * 0.5f, 0, 0), (oldModel != null ? oldModel.PhysicsBody : null), new SCNVector3 (0, 0, 1), new SCNVector3 (cubeWidth * 0.5f, 0, 0));
				((SCNView)presentationViewController.View).Scene.PhysicsWorld.AddBehavior (joint);

				Hinges.Add (model);

				oldModel = model;
			}
		}
	}
}

