using System;
using System.Collections.Generic;

using AppKit;
using SceneKit;
using SpriteKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;
using CoreFoundation;
using System.IO;

namespace SceneKitReel
{
	[Register ("GameViewController")]
	public class GameViewController : NSViewController, ISCNSceneRendererDelegate, ISCNPhysicsContactDelegate
	{
		public const long NSEC_PER_SEC = 1000000000;

		const int SLIDE_COUNT = 10;

		const float TEXT_SCALE = 0.75f;
		const int TEXT_Z_SPACING = 200;

		const float MAX_FIRE = 25.0f;
		const float MAX_SMOKE = 20.0f;


		const int LOGO_SIZE = 70;
		const float TITLE_SIZE = TEXT_SCALE * 0.45f;

		const float MIN_ANGLE = -(float)(Math.PI / 2) * 0.1f;
		const float MAX_ANGLE = (float)Math.PI * 0.8f;

		const double HIT_DELAY = 3.0;

		const float MAX_RY = (float)(Math.PI / 4) * 1.5f;

		const int BOX_W = 8;
		const float FACTOR = 2.2f;

		const int SPRITE_SIZE = 256;

		const int W = 50;
		const int PAINT_FACTOR = 2;

		SKNode NextButton { get; set; }

		SKNode PreviousButton { get; set; }

		SKNode ButtonGroup { get; set; }

		//steps of the demo
		nint IntroductionStep;
		nint Step;

		//scene
		SCNScene Scene;

		SCNView SceneView { get; set; }

		//references to nodes for manipulation
		SCNNode CameraHandle;
		SCNNode CameraOrientation;
		SCNNode CameraNode;
		SCNNode SpotLightParentNode;
		SCNNode SpotLightNode;
		SCNNode AmbientLightNode;
		SCNNode FloorNode;
		SCNNode MainWall;
		SCNNode InvisibleWallForPhysicsSlide;

		//ship
		SCNNode ShipNode;
		SCNNode ShipPivot;
		SCNNode ShipHandle;
		SCNNode IntroNodeGroup;

		//physics slide
		List<SCNNode> Boxes;

		//particles slide
		SCNNode FireTruck;
		SCNNode Collider;
		SCNNode Emitter;
		SCNNode FireContainer;
		SCNNode Handle;
		SCNParticleSystem Fire;
		SCNParticleSystem Smoke;
		SCNParticleSystem Plok;
		bool HitFire;

		//physics fields slide
		SCNNode FieldEmitter;
		SCNNode FieldOwner;
		SCNNode InteractiveField;

		//SpriteKit integration slide
		SCNNode Torus;

		//shaders slide
		SCNNode ShaderGroupNode;
		SCNNode ShadedNode;
		nint ShaderStage;

		//camera manipulation
		SCNVector3 CameraBaseOrientation;
		CGPoint InitialOffset, LastOffset;
		List<SCNMatrix4> CameraHandleTransforms = new List<SCNMatrix4> (SLIDE_COUNT);
		List<SCNMatrix4> CameraOrientationTransforms = new List<SCNMatrix4> (SLIDE_COUNT);

		NSTimer Timer;

		bool PreventNext;

		static SCNMaterial Material;
		static SCNMatrix4 OldSpotTransform;

		static List<SCNNode> PhysicBoxes;
		static int BoxesCount = 0;

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

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			Setup ();
		}

		#region Setup

		private void Setup ()
		{
			SceneView = (SCNView)this.View;

			SceneView.BackgroundColor = NSColor.Black;

			//setup ivars
			Boxes = new List<SCNNode> ();

			//setup the scene
			SetupScene ();

			//present it
			SceneView.Scene = Scene;

			//tweak physics
			SceneView.Scene.PhysicsWorld.Speed = 2.0f;

			//let's be the delegate of the SCNView
			//SceneView.delegate = this;

			SceneView.JitteringEnabled = true;

			//initial point of view
			SceneView.PointOfView = CameraNode;

			//setup overlays
			var overlay = new SpriteKitOverlayScene (SceneView.Bounds.Size);
			SceneView.OverlayScene = overlay;
		}

		private void SetupScene ()
		{
			Scene = SCNScene.Create ();

			SetupEnvironment ();
			SetupSceneElements ();
			SetupIntroEnvironment ();
		}

		private void SetupEnvironment ()
		{
			// |_   cameraHandle
			//   |_   cameraOrientation
			//     |_   cameraNode

			//create a main camera
			CameraNode = SCNNode.Create ();
			CameraNode.Position = new SCNVector3 (0, 0, 120);

			//create a node to manipulate the camera orientation
			CameraHandle = SCNNode.Create ();
			CameraHandle.Position = new SCNVector3 (0, 60, 0);

			CameraOrientation = SCNNode.Create ();

			Scene.RootNode.AddChildNode (CameraHandle);
			CameraHandle.AddChildNode (CameraOrientation);
			CameraOrientation.AddChildNode (CameraNode);

			CameraNode.Camera = SCNCamera.Create ();
			CameraNode.Camera.ZFar = 800;

			#if __IOS__
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				CameraNode.Camera.YFov = 55;
			} else
			#endif 
			{
				CameraNode.Camera.XFov = 75;
			}

			for (int i = 0; i < SLIDE_COUNT; i++) {
				CameraHandleTransforms.Add (new SCNMatrix4 ());
				CameraOrientationTransforms.Add (new SCNMatrix4 ());
			}
			CameraHandleTransforms [0] = CameraNode.Transform;

			// add an ambient light
			AmbientLightNode = SCNNode.Create ();
			AmbientLightNode.Light = SCNLight.Create ();

			AmbientLightNode.Light.LightType = SCNLightType.Ambient;
			AmbientLightNode.Light.Color = NSColor.FromDeviceWhite (0.3f, 1.0f);

			Scene.RootNode.AddChildNode (AmbientLightNode);


			//add a key light to the scene
			SpotLightParentNode = SCNNode.Create ();
			SpotLightParentNode.Position = new SCNVector3 (0, 90, 20);

			SpotLightNode = SCNNode.Create ();
			SpotLightNode.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 4);

			SpotLightNode.Light = SCNLight.Create ();
			SpotLightNode.Light.LightType = SCNLightType.Spot;
			SpotLightNode.Light.Color = NSColor.FromDeviceWhite (1.0f, 1.0f);
			SpotLightNode.Light.CastsShadow = true;
			SpotLightNode.Light.ShadowColor = NSColor.FromDeviceWhite (0, 0.5f);
			SpotLightNode.Light.ZNear = 30;
			SpotLightNode.Light.ZFar = 800;
			SpotLightNode.Light.ShadowRadius = 1.0f;
			SpotLightNode.Light.SpotInnerAngle = 15;
			SpotLightNode.Light.SpotOuterAngle = 70;

			CameraNode.AddChildNode (SpotLightParentNode);
			SpotLightParentNode.AddChildNode (SpotLightNode);


			//floor
			var floor = SCNFloor.Create ();
			floor.ReflectionFalloffEnd = 0;
			floor.Reflectivity = 0;

			FloorNode = SCNNode.Create ();
			FloorNode.Geometry = floor;
			FloorNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/wood", "png"));
			FloorNode.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;
			FloorNode.Geometry.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
			FloorNode.Geometry.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;
			FloorNode.Geometry.FirstMaterial.Diffuse.MipFilter = SCNFilterMode.Linear;

			FloorNode.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
			FloorNode.PhysicsBody.Restitution = 1.0f;

			Scene.RootNode.AddChildNode (FloorNode);
		}

		private void SetupSceneElements ()
		{
			// create the wall geometry
			var wallGeometry = SCNPlane.Create (800, 200);
			wallGeometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/wallPaper", "png"));
			wallGeometry.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.Mult (SCNMatrix4.Scale (8, 2, 1), SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (0, 0, 1), (nfloat)Math.PI / 4));
			wallGeometry.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
			wallGeometry.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;
			wallGeometry.FirstMaterial.DoubleSided = false;
			wallGeometry.FirstMaterial.LocksAmbientWithDiffuse = true;

			var wallWithBaseboardNode = SCNNode.FromGeometry (wallGeometry);
			wallWithBaseboardNode.Position = new SCNVector3 (200, 100, -20);
			wallWithBaseboardNode.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
			wallWithBaseboardNode.PhysicsBody.Restitution = 1.0f;
			wallWithBaseboardNode.CastsShadow = false;

			var baseboardNode = SCNNode.FromGeometry (SCNPlane.Create (800, 8));
			baseboardNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/baseboard", "jpg"));
			baseboardNode.Geometry.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
			baseboardNode.Geometry.FirstMaterial.DoubleSided = false;
			baseboardNode.Geometry.FirstMaterial.LocksAmbientWithDiffuse = true;
			baseboardNode.Position = new SCNVector3 (0, -wallWithBaseboardNode.Position.Y + 4, 0.5f);
			baseboardNode.CastsShadow = false;

			wallWithBaseboardNode.AddChildNode (baseboardNode);

			//front walls
			MainWall = wallWithBaseboardNode;
			Scene.RootNode.AddChildNode (wallWithBaseboardNode);

			//back
			var wallNode = wallWithBaseboardNode.Clone ();
			wallNode.Opacity = 0;
			wallNode.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
			wallNode.PhysicsBody.Restitution = 1.0f;
			wallNode.PhysicsBody.CategoryBitMask = 1 << 2;
			wallNode.CastsShadow = false;

			wallNode.Position = new SCNVector3 (0, 100, 40);
			wallNode.Rotation = new SCNVector4 (0, 1, 0, (nfloat)Math.PI);
			Scene.RootNode.AddChildNode (wallNode);

			//left
			wallNode = wallWithBaseboardNode.Clone ();
			wallNode.Position = new SCNVector3 (-120, 100, 40);
			wallNode.Rotation = new SCNVector4 (0, 1, 0, (nfloat)Math.PI / 2);
			Scene.RootNode.AddChildNode (wallNode);

			//right (an invisible wall to keep the bodies in the visible area when zooming in the Physics slide)
			wallNode = wallNode.Clone ();
			wallNode.Opacity = 0;
			wallNode.Position = new SCNVector3 (120, 100, 40);
			wallNode.Rotation = new SCNVector4 (0, 1, 0, (nfloat)Math.PI / 2);
			InvisibleWallForPhysicsSlide = wallNode;

			//right (the actual wall on the right)
			wallNode = wallWithBaseboardNode.Clone ();
			wallNode.PhysicsBody = null;
			wallNode.Position = new SCNVector3 (600, 100, 40);
			wallNode.Rotation = new SCNVector4 (0, 1, 0, (nfloat)Math.PI / 2);
			Scene.RootNode.AddChildNode (wallNode);

			//top
			wallNode = wallWithBaseboardNode.Clone ();
			wallNode.Geometry = (SCNGeometry)wallNode.Geometry.Copy ();
			wallNode.Geometry.FirstMaterial = SCNMaterial.Create ();
			wallNode.Opacity = 1;
			wallNode.Position = new SCNVector3 (200, 200, 0);
			wallNode.Scale = new SCNVector3 (1, 10, 1);
			wallNode.Rotation = new SCNVector4 (1, 0, 0, (nfloat)Math.PI / 2);
			Scene.RootNode.AddChildNode (wallNode);
		}

		private void SetupIntroEnvironment ()
		{
			IntroductionStep = 1;

			// configure the lighting for the introduction (dark lighting)
			AmbientLightNode.Light.Color = NSColor.Black;
			SpotLightNode.Light.Color = NSColor.Black;
			SpotLightNode.Position = new SCNVector3 (50, 90, -50);
			SpotLightNode.EulerAngles = new SCNVector3 (-(nfloat)(Math.PI / 2) * 0.75f, (nfloat)(Math.PI / 4) * 0.5f, 0);

			//put all texts under this node to remove all at once later
			IntroNodeGroup = SCNNode.Create ();

			//Slide 1
			var sceneKitLogo = SCNNode.FromGeometry (SCNPlane.Create (LOGO_SIZE, LOGO_SIZE));
			sceneKitLogo.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/SceneKit", "png"));
			sceneKitLogo.Geometry.FirstMaterial.Emission.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/SceneKit", "png"));
			sceneKitLogo.Geometry.FirstMaterial.Emission.Intensity = 0;

			IntroNodeGroup.AddChildNode (sceneKitLogo);
			sceneKitLogo.Position = new SCNVector3 (200, LOGO_SIZE / 2, 1050);

			var position = new SCNVector3 (200, 0, 1000);

			CameraNode.Position = new SCNVector3 (200, -20, position.Z + 150);
			CameraNode.EulerAngles = new SCNVector3 (-(nfloat)(Math.PI / 2) * 0.06f, 0, 0);


			//slide 2
			var text = SCNText.Create ("Load\n3D Scenes", 5);
			text.ChamferRadius = 0.3f;
			text.Flatness = 0.1f;
			/*#if TARGET_OS_IPHONE
			text.font = [UIFont fontWithName:@"Avenir-heavy" size:30];
			#else*/
			text.Font = NSFont.FromFontName ("Avenir Heavy", 30);
			text.Materials = new SCNMaterial[] { TextMaterial () };
			var textNode = SCNNode.Create ();
			textNode.Geometry = text;
			textNode.Scale = new SCNVector3 (TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);

			position.Z -= TEXT_Z_SPACING;
			var min = new SCNVector3 ();
			var max = new SCNVector3 ();
			textNode.GetBoundingBox (ref min, ref max);

			position = new SCNVector3 (110, -min.Y * TEXT_SCALE, position.Z);
			textNode.Position = position;
			IntroNodeGroup.AddChildNode (textNode);

			/* hierarchy
		     shipHandle
		     |_ shipXTranslate
		     |_ shipPivot
		     |_ ship */
			var modelScene = SCNScene.FromFile ("assets.scnassets/models/ship");
			ShipNode = modelScene.RootNode.FindChildNode ("Aircraft", true);
			ShipNode.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2); 
			ShipNode.Opacity = 0;

			ShipPivot = SCNNode.Create ();
			var shipXTranslate = SCNNode.Create ();
			ShipHandle = SCNNode.Create ();

			ShipHandle.Position = new SCNVector3 (200, 0, position.Z + 30);
			ShipNode.Position = new SCNVector3 (50, 30, 0);

			ShipPivot.AddChildNode (ShipNode);
			shipXTranslate.AddChildNode (ShipPivot);
			ShipHandle.AddChildNode (shipXTranslate);
			IntroNodeGroup.AddChildNode (ShipHandle);


			//slide 3
			text = SCNText.Create ("Animate & Render", 5);
			text.ChamferRadius = 0.3f;
			text.Flatness = 0.1f;
			#if __IOS__
			text.Font = UIFont.FromFontName ("Avenir Heavy", 30);
			#else
			text.Font = NSFont.FromFontName ("Avenir Heavy", 30);
			#endif
			text.Materials = new SCNMaterial[] { TextMaterial () };
			textNode = SCNNode.Create ();
			textNode.Geometry = text;
			textNode.Scale = new SCNVector3 (TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);

			position.Z -= TEXT_Z_SPACING;
			textNode.GetBoundingBox (ref min, ref max);

			position = new SCNVector3 (100, -min.Y * TEXT_SCALE, position.Z);
			textNode.Position = position;
			IntroNodeGroup.AddChildNode (textNode);


			//slide 4
			text = SCNText.Create ("OS X    iOS", 5);
			text.ChamferRadius = 0.3f;
			text.Flatness = 0.1f;
			#if __IOS__
			text.Font = UIFont.FromFontName ("Avenir Heavy", 50);
			#else
			text.Font = NSFont.FromFontName ("Avenir Heavy", 50);
			#endif
			text.Materials = new SCNMaterial[] { TextMaterial () };
			textNode = SCNNode.Create ();
			textNode.Geometry = text;
			textNode.Scale = new SCNVector3 (TEXT_SCALE, TEXT_SCALE, TEXT_SCALE);

			position.Z -= TEXT_Z_SPACING;
			position.Z -= TEXT_Z_SPACING;
			textNode.GetBoundingBox (ref min, ref max);

			position = new SCNVector3 (200 - (max.X - min.X) * TEXT_SCALE * 0.5f, -min.Y * TEXT_SCALE, position.Z);
			textNode.Position = position;
			IntroNodeGroup.AddChildNode (textNode);

			Scene.RootNode.AddChildNode (IntroNodeGroup);

			//wait, then fade in light
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			SCNTransaction.SetCompletionBlock (() => {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 2.5f;

				SpotLightNode.Light.Color = NSColor.FromDeviceWhite (1, 1);
				sceneKitLogo.Geometry.FirstMaterial.Emission.Intensity = 0.75f;

				SCNTransaction.Commit ();
			});

			SpotLightNode.Light.Color = NSColor.FromDeviceWhite (0.001f, 1);

			SCNTransaction.Commit ();
		}

		// the material to use for text
		private SCNMaterial TextMaterial ()
		{
			if (Material == null) {
				Material = SCNMaterial.Create ();
				Material.Specular.Contents = NSColor.FromDeviceWhite (0.6f, 1);
				Material.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/color_envmap", "png"));
				Material.Shininess = 0.1f;
			}
			return Material;
		}

		// switch to the next introduction step
		private void NextIntroductionStep ()
		{
			IntroductionStep++;

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			SCNTransaction.SetCompletionBlock (() => {
				if (IntroductionStep >= 4) {
					//We did finish introduction step 4
					//free some memory
					if (ShipHandle != null)
						ShipHandle.RemoveFromParentNode ();
					ShipHandle = null;
					ShipPivot = null;
					ShipNode = null;

				}
				if (IntroductionStep == 0) {
					//We did finish the whole introduction
					IntroNodeGroup.RemoveFromParentNode ();
					IntroNodeGroup = null;

					Next ();
				}
			});

			if (IntroductionStep == 2) {
				var popTime = new DispatchTime (DispatchTime.Now, (long)(1 * NSEC_PER_SEC));
				DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 1.0f;
					ShipNode.Opacity = 1.0f;
					ShipNode.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, (nfloat)Math.PI, 0, 1.0)));
					SCNTransaction.Commit ();
				});
			}

			if (IntroductionStep == 3) {
				//animate ship
				ShipNode.RemoveAllActions ();
				ShipNode.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2);
				ShipNode.EulerAngles = new SCNVector3 (ShipNode.EulerAngles.X, ShipNode.EulerAngles.Y, ShipNode.EulerAngles.Z + (float)(Math.PI / 4) * 0.5f);

				//make spotlight relative to the ship
				var newPosition = new SCNVector3 (50, 100, 0);
				var oldTransform = ShipPivot.ConvertTransformFromNode (SCNMatrix4.Identity, SpotLightNode);

				OldSpotTransform = SpotLightNode.Transform;

				SpotLightNode.RemoveFromParentNode ();
				SpotLightNode.Transform = oldTransform;

				ShipPivot.AddChildNode (SpotLightNode);

				SpotLightNode.Position = newPosition; // will animate implicitly
				SpotLightNode.EulerAngles = new SCNVector3 (-(float)Math.PI / 2, 0, 0);
				SpotLightNode.Light.SpotOuterAngle = 120;

				var action = SCNAction.Sequence (new SCNAction[] {
					SCNAction.Wait (1),
					SCNAction.RepeatActionForever (SCNAction.RotateBy (0, (nfloat)Math.PI, 0, 2))
				});
				ShipPivot.RunAction (action);

				var animation = CABasicAnimation.FromKeyPath ("position.x");
				animation.From = new NSNumber (-50);
				animation.To = new NSNumber (50);
				animation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
				animation.AutoReverses = true;
				animation.Duration = 2;
				animation.RepeatCount = float.MaxValue;
				animation.TimeOffset = animation.Duration * 0.5f;
				ShipPivot.ParentNode.AddAnimation (animation, null);

				var emitter = ShipNode.FindChildNode ("emitter", true);
				var ps = SCNParticleSystem.Create ("reactor.scnp", "assets.scnassets/particles");
				emitter.AddParticleSystem (ps);

				ShipHandle.Position = new SCNVector3 (ShipHandle.Position.X, ShipHandle.Position.Y, ShipHandle.Position.Z - TEXT_Z_SPACING - 50);
			}


			if (IntroductionStep == 4) {
				//restore spot light config
				SpotLightNode.Light.SpotOuterAngle = 70;
				var oldTransform = SpotLightParentNode.ConvertTransformFromNode (SCNMatrix4.Identity, SpotLightNode);
				SpotLightNode.RemoveFromParentNode ();
				SpotLightNode.Transform = oldTransform;

				SpotLightParentNode.AddChildNode (SpotLightNode);
				SpotLightNode.Transform = OldSpotTransform;

				CameraNode.Position = new SCNVector3 (CameraNode.Position.X, CameraNode.Position.Y, CameraNode.Position.Z - TEXT_Z_SPACING);
			}


			if (IntroductionStep == 5) {
				IntroductionStep = 0;//introduction is over
				// jiterring is now useless since the screen will never be static again.
				var scnView = (SCNView)this.View;
				scnView.JitteringEnabled = false;

				AmbientLightNode.Light.Color = NSColor.FromDeviceWhite (0.3f, 1.0f);
				SpotLightNode.Position = new SCNVector3 (0, 0, 0);
				SpotLightNode.EulerAngles = new SCNVector3 (-(float)(Math.PI / 4), 0, 0);
				CameraNode.Position = new SCNVector3 (0, 0, 120);
				CameraNode.EulerAngles = new SCNVector3 (0, 0, 0);
			} else {
				CameraNode.Position = new SCNVector3 (CameraNode.Position.X, CameraNode.Position.Y, CameraNode.Position.Z - TEXT_Z_SPACING);
			}

			SCNTransaction.Commit ();
		}

		//restore the default camera orientation and position
		private void RestoreCameraAngle ()
		{
			//reset drag offset
			InitialOffset = new CGPoint (0, 0);
			LastOffset = InitialOffset;

			//restore default camera
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.5f;
			SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			CameraHandle.EulerAngles = new SCNVector3 (0, 0, 0);
			SCNTransaction.Commit ();
		}

		// tilt the camera based on an offset
		private void TiltCamera (CGPoint offset)
		{
			if (IntroductionStep != 0)
				return;

			offset.X += InitialOffset.X;
			offset.Y += InitialOffset.Y;

			CGPoint tr;
			tr.X = offset.X - LastOffset.X;
			tr.Y = offset.Y - LastOffset.Y;

			LastOffset = offset;

			offset.X *= 0.1f;
			offset.Y *= 0.1f;
			var rx = offset.Y;
			var ry = offset.X;

			ry *= 0.05f;
			rx *= 0.05f;

			#if __IOS__
			rx = -rx; //on iOS, invert rotation on the X axis
			#endif

			if (rx > 0.5f) {
				rx = 0.5f;
				InitialOffset.Y -= tr.Y;
				LastOffset.Y -= tr.Y;
			}
			if (rx < -(float)(Math.PI / 2)) {
				rx = -(float)Math.PI / 2;
				InitialOffset.Y -= tr.Y;
				LastOffset.Y -= tr.Y;
			}
			if (ry > MAX_RY) {
				ry = MAX_RY;
				InitialOffset.X -= tr.X;
				LastOffset.X -= tr.X;
			}
			if (ry < -MAX_RY) {
				ry = -MAX_RY;
				InitialOffset.X -= tr.X;
				LastOffset.X -= tr.X;

			}

			ry = -ry;

			CameraHandle.EulerAngles = new SCNVector3 (rx, ry, 0);
		}

		#endregion

		#region Physics

		// return a new physically based box at the specified position
		// sometimes generate a ball instead of a box for more variety
		private SCNNode GetBox (SCNVector3 position)
		{
			if (PhysicBoxes == null) {
				PhysicBoxes = new List<SCNNode> ();

				var box = SCNNode.Create ();
				box.Geometry = SCNBox.Create (BOX_W, BOX_W, BOX_W, 0.1f);
				box.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/WoodCubeA", "jpg"));
				box.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();

				PhysicBoxes.Add (box);

				box = box.Clone ();
				box.Geometry = (SCNGeometry)box.Geometry.Copy ();
				box.Geometry.FirstMaterial = (SCNMaterial)box.Geometry.FirstMaterial.Copy ();
				box.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/WoodCubeB", "jpg"));
				PhysicBoxes.Add (box);

				box = box.Clone ();
				box.Geometry = (SCNGeometry)box.Geometry.Copy ();
				box.Geometry.FirstMaterial = (SCNMaterial)box.Geometry.FirstMaterial.Copy ();
				box.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/WoodCubeC", "jpg"));
				PhysicBoxes.Add (box);

				var ball = SCNNode.Create ();
				var sphere = SCNSphere.Create (BOX_W * 0.75f);
				ball.Geometry = sphere;
				ball.Geometry.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
				ball.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/ball", "jpg"));
				ball.Geometry.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/envmap", "jpg"));
				ball.Geometry.FirstMaterial.FresnelExponent = 1.0f;
				ball.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
				ball.PhysicsBody.Restitution = 0.9f;
				PhysicBoxes.Add (ball);
			}

			BoxesCount++;

			int index = BoxesCount % 3;
			if (BoxesCount == 1 || BoxesCount == 7)
				index = 3;

			var item = PhysicBoxes [index].Clone ();
			item.Position = position;

			return item;
		}

		//apply an explosion force at the specified location to the specified nodes
		//remove from the nodes from the scene graph is removeOnCompletion is set to yes
		private void Explosion (SCNVector3 center, List<SCNNode> nodes, bool removeOnCompletion)
		{
			var c = center;

			foreach (SCNNode node in nodes) {
				var p = node.PresentationNode.Position;

				c.Y = removeOnCompletion ? -20 : -90;
				c.Z = removeOnCompletion ? 0 : 50;
				var direction = SCNVector3.Subtract (p, c);

				c.Y = 0;
				c.Z = 0;
				var dist = SCNVector3.Subtract (p, c);

				var force = removeOnCompletion ? 2000 : 1000 * (1.0 + Math.Abs (c.X) / 100.0);

				var distance = dist.Length;

				if (removeOnCompletion) {
					if (direction.X < 500.0 && direction.X > 0)
						direction.X += 500;
					if (direction.X > -500.0 && direction.X < 0)
						direction.X -= 500;
					node.PhysicsBody.CollisionBitMask = 0x0;
				}

				//normalise
				direction = SCNVector3.Normalize (direction);
				direction = SCNVector3.Multiply (direction, (nfloat)(FACTOR * force / Math.Max (20.0, distance)));

				node.PhysicsBody.ApplyForce (direction, removeOnCompletion ? SCNVector3.Zero : new SCNVector3 (RandFloat (-0.2, 0.2), RandFloat (-0.2, 0.2), RandFloat (-0.2, 0.2)), true);

				if (removeOnCompletion) {
					node.RunAction (SCNAction.Sequence (new SCNAction[] {
						SCNAction.Wait (1.0),
						SCNAction.FadeOut (0.125),
						SCNAction.RemoveFromParentNode ()
					}));
				}
			}
		}

		// present physics slide
		private void ShowPhysicsSlide ()
		{
			var count = 80;
			var spread = 6.0f;

			SCNScene scene = ((SCNView)this.View).Scene;

			//tweak physics
			scene.PhysicsWorld.Gravity = new SCNVector3 (0, -70, 0);

			//add invisible wall
			scene.RootNode.AddChildNode (InvisibleWallForPhysicsSlide);

			// drop rigid bodies cubes
			var intervalTime = 10.0 / count;
			var remainingCount = count;
			var right = false;

			Timer = NSTimer.CreateRepeatingTimer (intervalTime, delegate(NSTimer obj) {

				if (Step > 1) {
					Timer.Invalidate ();
					return;
				}

				SCNTransaction.Begin ();

				var pos = new SCNVector3 (right ? 100 : -100, 50, 0);

				var box = GetBox (pos);

				//add to scene
				Scene.RootNode.AddChildNode (box);

				box.PhysicsBody.Velocity = new SCNVector3 (FACTOR * (right ? -50 : 50), FACTOR * (30 + RandFloat (-spread, spread)), FACTOR * (RandFloat (-spread, spread)));
				box.PhysicsBody.AngularVelocity = new SCNVector4 (RandFloat (-1, 1), RandFloat (-1, 1), RandFloat (-1, 1), RandFloat (-3, 3));
				SCNTransaction.Commit ();

				Boxes.Add (box);

				// ensure we stop firing
				if (--remainingCount < 0)
					Timer.Invalidate ();

				right = right ? false : true;
			});

			NSRunLoop.Current.AddTimer (Timer, NSRunLoopMode.Default);
		}

		//remove physics slide
		private void OrderOutPhysics ()
		{
			//move physics out
			Explosion (new SCNVector3 (0, 0, 0), Boxes, true);
			Boxes = null;

			//add invisible wall
			var scene = ((SCNView)this.View).Scene;
			scene.RootNode.AddChildNode (InvisibleWallForPhysicsSlide);
		}

		#endregion

		#region Particles

		//present particle slide
		private void ShowParticlesSlide ()
		{
			//restore defaults
			((SCNView)this.View).Scene.PhysicsWorld.Gravity = new SCNVector3 (0, -9.8f, 0);

			//add truck
			var fireTruckScene = SCNScene.FromFile ("assets.scnassets/models/firetruck");
			var fireTruck = fireTruckScene.RootNode.FindChildNode ("firetruck", true);
			var emitter = fireTruck.FindChildNode ("emitter", true);
			Handle = fireTruck.FindChildNode ("handle", true);

			fireTruck.Position = new SCNVector3 (120, 10, 0);
			fireTruck.Scale = new SCNVector3 (0.2f, 0.2f, 0.2f);
			fireTruck.Rotation = new SCNVector4 (0, 1, 0, (nfloat)Math.PI / 2); //TODO Remove Rotation

			Scene.RootNode.AddChildNode (fireTruck);

			//add fire container
			var fireContainerScene = SCNScene.FromFile ("assets.scnassets/models/bac");
			FireContainer = fireContainerScene.RootNode.FindChildNode ("box", true);
			FireContainer.Scale = new SCNVector3 (0.5f, 0.25f, 0.25f);
			FireContainer.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2); 
			Scene.RootNode.AddChildNode (FireContainer);

			//preload it to avoid frame drop
			//(SCNView)this.View prepareObject:_scene shouldAbortBlock:nil];

			FireTruck = fireTruck;

			//collider
			var colliderNode = SCNNode.Create ();
			colliderNode.Geometry = SCNBox.Create (50, 2, 25, 0);
			colliderNode.Geometry.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("assets.scnassets/textures/train_wood", "jpg"));
			colliderNode.Position = new SCNVector3 (60, 260, 5);
			Scene.RootNode.AddChildNode (colliderNode);

			var moveIn = SCNAction.MoveBy (0, -215, 0, 1.0);
			moveIn.TimingMode = SCNActionTimingMode.EaseOut;
			colliderNode.RunAction (SCNAction.Sequence (new SCNAction[] { SCNAction.Wait (2), moveIn }));

			var animation = CABasicAnimation.FromKeyPath ("eulerAngles");
			animation.From = NSNumber.FromVector (new SCNVector3 (0, 0, 0));
			animation.To = NSNumber.FromVector (new SCNVector3 (0, 0, 2 * (nfloat)Math.PI));
			animation.BeginTime = CAAnimation.CurrentMediaTime () + 0.5f;
			animation.Duration = 2;
			animation.RepeatCount = float.MaxValue;
			colliderNode.AddAnimation (animation, null);
			Collider = colliderNode;

			SCNParticleSystem ps;

			//add fire
			var fireHolder = SCNNode.Create ();
			Emitter = fireHolder;
			fireHolder.Position = new SCNVector3 (0, 0, 0);
			ps = SCNParticleSystem.Create ("fire.scnp", "assets.scnassets/particles/");
			Smoke = SCNParticleSystem.Create ("smoke.scnp", "assets.scnassets/particles/");
			Smoke.BirthRate = 0;
			fireHolder.AddParticleSystem (ps);

			var smokeEmitter = SCNNode.Create ();
			smokeEmitter.Position = new SCNVector3 (0, 0, 0.5f);
			smokeEmitter.AddParticleSystem (Smoke);
			fireHolder.AddChildNode (smokeEmitter);
			Scene.RootNode.AddChildNode (fireHolder);

			Fire = ps;

			//add water
			ps = SCNParticleSystem.Create ("sparks.scnp", "assets.scnassets/particles/");
			ps.BirthRate = 0;
			ps.SpeedFactor = 3.0f;
			ps.ColliderNodes = new SCNNode[] { FloorNode, colliderNode };
			emitter.AddParticleSystem (ps);

			var tr = SCNAction.MoveBy (new SCNVector3 (60, 0, 0), 1);
			tr.TimingMode = SCNActionTimingMode.EaseInEaseOut;
		}

		//remove particle slide
		private void OrderOutParticles ()
		{
			//remove fire truck
			FireTruck.RemoveFromParentNode ();
			Emitter.RemoveFromParentNode ();
			Collider.RemoveFromParentNode ();
			FireContainer.RemoveFromParentNode ();
			FireContainer = null;
			Collider = null;
			Emitter = null;
			FireTruck = null;
		}

		#endregion

		#region PhysicsFields

		private void MoveEmitter (CGPoint p)
		{
			var scnView = (SCNView)this.View;
			var pTmp = scnView.ProjectPoint (new SCNVector3 (0, 0, 50));
			var p3d = scnView.UnprojectPoint (new SCNVector3 (p.X, p.Y, pTmp.Z));
			p3d.Z = 50;
			p3d.Y = (nfloat)Math.Max (p3d.Y, 5);
			FieldOwner.Position = p3d;
			FieldOwner.PhysicsField.Strength = 200000.0f;
		}

		//present physics field slide
		private void ShowPhysicsFields ()
		{
			nfloat dz = 50;

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			SpotLightNode.Light.Color = NSColor.FromDeviceWhite (0.5f, 1.0f);
			AmbientLightNode.Light.Color = NSColor.Black;
			SCNTransaction.Commit ();

			//remove gravity for this slide
			Scene.PhysicsWorld.Gravity = SCNVector3.Zero;

			//move camera
			var tr = SCNAction.MoveBy (new SCNVector3 (0, 0, dz), 1);
			tr.TimingMode = SCNActionTimingMode.EaseInEaseOut;
			CameraHandle.RunAction (tr);

			//add particles
			FieldEmitter = SCNNode.Create ();
			FieldEmitter.Position = new SCNVector3 (CameraHandle.Position.X, 5, dz);

			var ps = SCNParticleSystem.Create ("bubbles.scnp", "assets.scnassets/particles/");

			ps.ParticleColor = NSColor.FromDeviceRgba (0.8f, 0, 0, 1.0f);
			ps.ParticleColorVariation = new SCNVector4 (0.3f, 0.2f, 0.3f, 0.0f);
			ps.SortingMode = SCNParticleSortingMode.Distance;
			ps.BlendMode = SCNParticleBlendMode.Alpha;
			var right = new NSImage (NSBundle.MainBundle.PathForResource ("images/cubemap/right", "jpg"));
			var left = new NSImage (NSBundle.MainBundle.PathForResource ("images/cubemap/left", "jpg"));
			var top = new NSImage (NSBundle.MainBundle.PathForResource ("images/cubemap/top", "jpg"));
			var bottom = new NSImage (NSBundle.MainBundle.PathForResource ("images/cubemap/bottom", "jpg"));
			var front = new NSImage (NSBundle.MainBundle.PathForResource ("images/cubemap/front", "jpg"));
			var back = new NSImage (NSBundle.MainBundle.PathForResource ("images/cubemap/back", "jpg"));
			var cubMap = new NSMutableArray ();
			cubMap.AddObjects (new NSObject[] { right, left, top, bottom, front, back });
			ps.ParticleImage = cubMap;
			ps.FresnelExponent = 2;
			ps.ColliderNodes = new SCNNode[] { FloorNode, MainWall };

			ps.EmitterShape = SCNBox.Create (200, 0, 100, 0);

			FieldEmitter.AddParticleSystem (ps);
			Scene.RootNode.AddChildNode (FieldEmitter);

			//field
			FieldOwner = SCNNode.Create ();
			FieldOwner.Position = new SCNVector3 (CameraHandle.Position.X, 50, dz + 5);

			var field = SCNPhysicsField.CreateRadialGravityField ();
			field.HalfExtent = new SCNVector3 (100, 100, 100);
			field.MinimumDistance = 20.0f;
			field.FalloffExponent = 0;
			FieldOwner.PhysicsField = field;
			FieldOwner.PhysicsField.Strength = 0.0f;
			Scene.RootNode.AddChildNode (FieldOwner);
		}

		//remove physics field slide
		private void OrderOutPhysicsFields ()
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			SpotLightNode.Light.Color = NSColor.FromDeviceWhite (1.0f, 1.0f);
			AmbientLightNode.Light.Color = NSColor.FromDeviceWhite (0.3f, 1.0f);
			SCNTransaction.Commit ();

			//move camera
			nfloat dz = 50;
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			CameraHandle.Position = new SCNVector3 (CameraHandle.Position.X, CameraHandle.Position.Y, CameraHandle.Position.Z - dz);
			SCNTransaction.Commit ();

			FieldEmitter.RemoveFromParentNode ();
			FieldOwner.RemoveFromParentNode ();
			FieldEmitter = null;
			FieldOwner = null;
		}

		#endregion

		#region SpriteKit

		// add a color "splash" at the specified location in the SKScene used as a material
		private void AddPaint (CGPoint p, NSColor color)
		{
			var skScene = (SKScene)Torus.Geometry.FirstMaterial.Diffuse.Contents;

			if (skScene.GetType () == typeof(SKScene)) {
				//update the contents of skScene by adding a splash of "color" at p (normalized [0, 1])
				p.X *= SPRITE_SIZE;
				p.Y *= SPRITE_SIZE;

				var node = SKSpriteNode.Create ();
				node.Position = p;
				node.XScale = 0.33f;

				var subNode = SKSpriteNode.FromImageNamed (NSBundle.MainBundle.PathForResource ("images/splash", "png"));
				subNode.ZRotation = RandFloat (0.0f, 2.0f * Math.PI);
				subNode.Color = color;
				subNode.ColorBlendFactor = 1;

				node.AddChild (subNode);
				skScene.AddChild (node);

				if (p.X < 16) {
					node = (SKNode)node.Copy ();
					p.X = SPRITE_SIZE + p.X;
					node.Position = p;
					skScene.AddChild (node);
				} else if (p.X > SPRITE_SIZE - 16) {
					node = (SKNode)node.Copy ();
					p.X = (p.X - SPRITE_SIZE);
					node.Position = p;
					skScene.AddChild (node);
				}
			}
		}

		// physics contact delegate
		[Export ("physicsWorld:didBeginContact:")]
		public void DidBeginContact (SCNPhysicsWorld world, SCNPhysicsContact contact)
		{
			SCNNode ball = null;

			if (contact.NodeA.PhysicsBody.Type == SCNPhysicsBodyType.Dynamic) {
				ball = contact.NodeA;
			} else {
				ball = contact.NodeB;
			}

			if (ball != null) {
				DispatchQueue.MainQueue.DispatchAsync (() => {
					ball.RemoveFromParentNode ();
				});

				var plokCopy = (SCNParticleSystem)Plok.Copy ();
				plokCopy.ParticleImage = Plok.ParticleImage; // to workaround an bug in seed #1
				plokCopy.ParticleColor = (NSColor)ball.Geometry.FirstMaterial.Diffuse.Contents;
				Scene.AddParticleSystem (plokCopy, SCNMatrix4.CreateTranslation (contact.ContactPoint.X, contact.ContactPoint.Y, contact.ContactPoint.Z));

				//compute texture coordinate
				var pointA = new SCNVector3 (contact.ContactPoint.X, contact.ContactPoint.Y, contact.ContactPoint.Z + 20);
				var pointB = new SCNVector3 (contact.ContactPoint.X, contact.ContactPoint.Y, contact.ContactPoint.Z - 20);

				var hitTestOptions = new SCNHitTestOptions (NSDictionary.FromObjectAndKey (Torus, SCNHitTest.RootNodeKey));
				var results = SceneView.Scene.RootNode.HitTest (pointA, pointB, hitTestOptions);

				if (results.Length > 0) {
					var hit = results [0];
					AddPaint (hit.GetTextureCoordinatesWithMappingChannel (0), plokCopy.ParticleColor);
				}
			}
		}

		//present spritekit integration slide
		private void ShowSpriteKitSlide ()
		{
			//place camera
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 2;
			CameraHandle.Position = new SCNVector3 (CameraHandle.Position.X + 200, 60, 0);
			SCNTransaction.Commit ();

			//load plok particles
			Plok = SCNParticleSystem.Create ("plok.scnp", "assets.scnassets/particles");

			//create a spinning object
			Torus = SCNNode.Create ();
			Torus.Position = new SCNVector3 (CameraHandle.Position.X, 60, 10);
			Torus.Geometry = SCNTorus.Create (W / 2, W / 6);
			Torus.PhysicsBody = SCNPhysicsBody.CreateStaticBody ();
			Torus.Opacity = 0.0f;

			var material = Torus.Geometry.FirstMaterial;
			material.Specular.Contents = NSColor.FromDeviceWhite (0.5f, 1);
			material.Shininess = 2.0f;

			material.Normal.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/wood-normal", "png"));

			Scene.RootNode.AddChildNode (Torus);
			Torus.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy ((nfloat)Math.PI * 2, new SCNVector3 (0.4f, 1, 0), 8)));

			//preload it to avoid frame drop
			//[(SCNView*)self.view prepareObject:_scene shouldAbortBlock:nil];

			Scene.PhysicsWorld.WeakContactDelegate = this;

			//setup material
			var skScene = SKScene.FromSize (new CGSize (SPRITE_SIZE, SPRITE_SIZE));
			skScene.BackgroundColor = NSColor.White;
			material.Diffuse.Contents = skScene;

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			SCNTransaction.SetCompletionBlock (() => {
				StartLaunchingColors ();
			});

			Torus.Opacity = 1.0f;

			SCNTransaction.Commit ();
		}

		private void StartLaunchingColors ()
		{
			//tweak physics
			((SCNView)this.View).Scene.PhysicsWorld.Gravity = new SCNVector3 (0, -70, 0);

			// drop rigid bodies
			var intervalTime = 0.1;
			var right = false;

			Timer = NSTimer.CreateRepeatingTimer (intervalTime, delegate(NSTimer obj) {

				if (Step != 4) {
					Timer.Invalidate ();
					return;
				}

				var ball = SCNNode.Create ();
				var sphere = SCNSphere.Create (2);
				ball.Geometry = sphere;
				ball.Geometry.FirstMaterial.Diffuse.Contents = NSColor.FromDeviceHsba (RandFloat (0, 1), 1, 1, 1);
				ball.Geometry.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/envmap", "jpg"));
				ball.Geometry.FirstMaterial.FresnelExponent = 1.0f;
				ball.PhysicsBody = SCNPhysicsBody.CreateDynamicBody ();
				ball.PhysicsBody.Restitution = 0.9f;
				ball.PhysicsBody.CategoryBitMask = 0x4;
				ball.PhysicsBody.CollisionBitMask = (nuint)~(0x4);

				SCNTransaction.Begin ();

				ball.Position = new SCNVector3 (CameraHandle.Position.X, 20, 100);

				//add to scene
				Scene.RootNode.AddChildNode (ball);

				ball.PhysicsBody.Velocity = new SCNVector3 (PAINT_FACTOR * RandFloat (-10, 10), 70 + RandFloat (0, 40), PAINT_FACTOR * -30.0f);
				SCNTransaction.Commit ();

				right = right ? false : true;
			});

			NSRunLoop.Current.AddTimer (Timer, NSRunLoopMode.Default);
		}

		private void OrderOutSpriteKit ()
		{
			Torus.RemoveFromParentNode ();
			Scene.PhysicsWorld.ContactDelegate = null;
		}

		#endregion

		#region Shaders

		private void ShowNextShaderStage ()
		{
			ShaderStage++;

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 2;

			//retrieve the node that owns the shader modifiers
			var node = ShadedNode;

			switch (ShaderStage) {
			case 1:
				node.Geometry.SetValueForKey (new NSNumber (3), new NSString ("Amplitude"));
				node.Geometry.SetValueForKey (new NSNumber (0.25f), new NSString ("Frequency"));
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("lightIntensity"));
				break;
			case 2:
				node.Geometry.SetValueForKey (new NSNumber (1), new NSString ("surfIntensity"));
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("Amplitude"));
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("lightIntensity"));
				break;
			case 3:
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("surfIntensity"));
				node.Geometry.SetValueForKey (new NSNumber (1), new NSString ("fragIntensity"));
				node.Geometry.SetValueForKey (new NSNumber (1), new NSString ("lightIntensity"));
				break;
			case 4:
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("surfIntensity"));
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("lightIntensity"));
				node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("fragIntensity"));
				ShaderStage = 0;
				break;

			}

			SCNTransaction.Commit ();
		}

		private void ShowShadersSlide ()
		{
			ShaderStage = 0;

			//move the camera back
			//place camera
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 1;
			CameraHandle.Position = new SCNVector3 (CameraHandle.Position.X + 180, 60, 0);
			CameraHandle.EulerAngles = new SCNVector3 (-(nfloat)(Math.PI / 4) * 0.3f, 0, 0);

			SpotLightNode.Light.SpotOuterAngle = 55;
			SCNTransaction.Commit ();

			ShaderGroupNode = SCNNode.Create ();
			ShaderGroupNode.Position = new SCNVector3 (CameraHandle.Position.X, -5, 20);
			Scene.RootNode.AddChildNode (ShaderGroupNode);

			//add globe stand
			var globe = SCNScene.FromFile ("assets.scnassets/models/globe").RootNode.FindChildNode ("globe", true);
			globe.Rotation = new SCNVector4 (1, 0, 0, -(nfloat)Math.PI / 2); //TODO Remove Rotation

			ShaderGroupNode.AddChildNode (globe);

			//show shader modifiers
			//add spheres
			var sphere = SCNSphere.Create (28);
			sphere.SegmentCount = 48;
			sphere.FirstMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/earth/earth-diffuse", "jpg"));
			sphere.FirstMaterial.Specular.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/earth/earth-specular", "jpg"));
			sphere.FirstMaterial.Specular.Intensity = 0.2f;

			sphere.FirstMaterial.Shininess = 0.1f;
			sphere.FirstMaterial.Reflective.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("images/envmap", "jpg"));
			sphere.FirstMaterial.Reflective.Intensity = 0.5f;
			sphere.FirstMaterial.FresnelExponent = 2;

			//GEOMETRY
			var node = globe.FindChildNode ("globeAttach", true);
			node.Geometry = sphere;
			node.Scale = new SCNVector3 (3, 3, 3);
			node.Rotation = new SCNVector4 (1, 0, 0, (nfloat)Math.PI / 2); //TODO Remove Rotation

			node.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, 0, (nfloat)Math.PI, 4.0))); //TODO Change Rotation to Y axis

			var geoModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("shaders/sm_geom", "shader"));
			var surfaceModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("shaders/sm_surf", "shader"));
			var fragmentModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("shaders/sm_frag", "shader"));
			var lightingModifier = File.ReadAllText (NSBundle.MainBundle.PathForResource ("shaders/sm_light", "shader"));

			node.Geometry.ShaderModifiers = new SCNShaderModifiers {
				EntryPointGeometry = geoModifier,
				EntryPointSurface = surfaceModifier,
				EntryPointFragment = fragmentModifier,
				EntryPointLightingModel = lightingModifier,
			};


			node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("Amplitude"));
			node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("lightIntensity"));
			node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("surfIntensity"));
			node.Geometry.SetValueForKey (new NSNumber (0), new NSString ("fragIntensity"));

			ShadedNode = node;

			//redraw forever
			((SCNView)this.View).Playing = true;
			((SCNView)this.View).Loops = true;
		}

		private void OrderOutShaders ()
		{
			ShaderGroupNode.RunAction (SCNAction.Sequence (new SCNAction[] {
				SCNAction.ScaleTo (0.01f, 1.0),
				SCNAction.RemoveFromParentNode ()
			}));
			ShaderGroupNode = null;
			((SCNView)this.View).Playing = false;
		}

		#endregion

		#region Presentation logic

		private void PresentStep (nint step)
		{
			var overlay = (SpriteKitOverlayScene)((SCNView)this.View).OverlayScene;

			if (CameraHandleTransforms [(int)step].M11 == 0) {
				CameraHandleTransforms [(int)step] = CameraHandle.Transform;
				CameraOrientationTransforms [(int)step] = CameraOrientation.Transform;
			}

			switch (step) {
			case 1:
				overlay.ShowLabel ("Physics");
				overlay.RunAction (SKAction.Sequence (new SKAction[] { SKAction.WaitForDuration (2), SKAction.Run (() => {
						if (Step == 1)
							overlay.ShowLabel (null);
					})
				}));

				var popTime = new DispatchTime (DispatchTime.Now, (long)(0.0 * NSEC_PER_SEC));
				DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
					ShowPhysicsSlide ();
				});
				break;
			case 2:
				overlay.ShowLabel ("Particles");
				overlay.RunAction (SKAction.Sequence (new SKAction[] { SKAction.WaitForDuration (4), SKAction.Run (() => {
						if (Step == 2)
							overlay.ShowLabel (null);
					})
				}));

				ShowParticlesSlide ();
				break;
			case 3:
				overlay.ShowLabel ("Physics Fields");
				overlay.RunAction (SKAction.Sequence (new SKAction[] { SKAction.WaitForDuration (2), SKAction.Run (() => {
						if (Step == 3)
							overlay.ShowLabel (null);
					})
				}));

				ShowPhysicsFields ();
				break;
			case 4:
				overlay.ShowLabel ("SceneKit + SpriteKit");
				overlay.RunAction (SKAction.Sequence (new SKAction[] { SKAction.WaitForDuration (4), SKAction.Run (() => {
						if (Step == 4)
							overlay.ShowLabel (null);
					})
				}));

				ShowSpriteKitSlide ();
				break;
			case 5:
				overlay.ShowLabel ("SceneKit + Shaders");
				ShowShadersSlide ();
				break;
			}
		}

		private void OrderOutStep (nint step)
		{
			switch (step) {
			case 1:
				OrderOutPhysics ();
				break;
			case 2:
				OrderOutParticles ();
				break;
			case 3:
				OrderOutPhysicsFields ();
				break;
			case 4:
				OrderOutSpriteKit ();
				break;
			case 5:
				OrderOutShaders ();
				break;
			}
		}

		private void Next ()
		{
			if (Step >= 5)
				return;

			OrderOutStep (Step);
			Step++;
			PresentStep (Step);
		}

		private void Previous ()
		{
			if (Step <= 1)
				return;

			OrderOutStep (Step);
			Step--;

			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			SCNTransaction.SetCompletionBlock (() => {
				PresentStep (Step);
			});

			CameraHandle.Transform = CameraHandleTransforms [(int)Step];
			CameraOrientation.Transform = CameraOrientationTransforms [(int)Step];

			SCNTransaction.Commit ();
		}

		#endregion

		#region Gestures

		public void GestureDidEnd ()
		{
			if (Step == 3) {
				//bubbles
				FieldOwner.PhysicsField.Strength = 0.0f;
			}
		}

		public void GestureDidBegin ()
		{
			InitialOffset = LastOffset;
		}

		public void HandlePan (CGPoint p)
		{
			var scnView = (SCNView)this.View;

			if (Step == 2) {
				//particles
				var pTmp = scnView.ProjectPoint (new SCNVector3 (0, 0, 0));
				var p3d = scnView.UnprojectPoint (new SCNVector3 (p.X, p.Y, pTmp.Z));
				var handlePos = Handle.WorldTransform;


				var dy = (nfloat)Math.Max (0, p3d.Y - handlePos.M42);
				var dx = handlePos.M41 - p3d.X;
				var angle = (nfloat)Math.Atan2 (dy, dx);


				angle -= 35.0f * (float)Math.PI / 180.0f; //handle is 35 degree by default

				//clamp
				if (angle < MIN_ANGLE)
					angle = MIN_ANGLE;
				if (angle > MAX_ANGLE)
					angle = MAX_ANGLE;


				var popTime = new DispatchTime (DispatchTime.Now, (long)(HIT_DELAY * NSEC_PER_SEC));
				if (angle <= 0.66 && angle >= 0.48) {
					DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
						//hit the fire!
						HitFire = true;
					});
				} else {
					DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
						//hit the fire!
						HitFire = false;
					});
				}

				Handle.Rotation = new SCNVector4 (1, 0, 0, angle);
			}

			if (Step == 3) {
				//bubbles
				MoveEmitter (p);
			}
		}

		public void HandleDoubleTap (CGPoint p)
		{
			RestoreCameraAngle ();
		}

		private void PreventAccidentalNext (nfloat delay)
		{
			PreventNext = true;

			//disable the next button for "delay" seconds to prevent accidental tap
			var overlay = (SpriteKitOverlayScene)((SCNView)this.View).OverlayScene;
			overlay.NextButton.RunAction (SKAction.FadeAlphaBy (-0.5f, 0.5f));
			overlay.PreviousButton.RunAction (SKAction.FadeAlphaBy (-0.5f, 0.5f));
			var popTime = new DispatchTime (DispatchTime.Now, (long)(delay * NSEC_PER_SEC));
			DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
				PreventNext = false;
				overlay.PreviousButton.RunAction (SKAction.FadeAlphaTo (Step > 1 ? 1 : 0, 0.75f));
				overlay.NextButton.RunAction (SKAction.FadeAlphaTo (IntroductionStep == 0 && Step < 5 ? 1 : 0, 0.75f));
			});
		}

		public void HandleTap (CGPoint p)
		{
			//test buttons
			SKScene skScene = ((SCNView)this.View).OverlayScene;
			var p2D = skScene.ConvertPointFromView (p);
			var node = skScene.GetNodeAtPoint (p2D);

			// wait X seconds before enabling the next tap to avoid accidental tap
			var ignoreNext = PreventNext;

			if (IntroductionStep > 0) {
				//next introduction step
				if (!ignoreNext) {
					PreventAccidentalNext (1);
					NextIntroductionStep ();
				}
				return;
			}

			if (ignoreNext == false) {
				if (Step == 0 || node.Name == "next" || node.Name == "back") {
					var shouldGoBack = node.Name == "back";

					if (node.Name == "next") {
						((SKSpriteNode)node).Color = NSColor.FromDeviceRgba (1, 0, 0, 1);
						node.RunAction (SKAction.CustomActionWithDuration (0.7, (SKNode spriteNode, nfloat elapsedTime) => {
							((SKSpriteNode)spriteNode).ColorBlendFactor = 0.7f - elapsedTime;
						}));
					}

					RestoreCameraAngle ();

					PreventAccidentalNext (Step == 1 ? 3 : 1);

					if (shouldGoBack)
						Previous ();
					else
						Next ();

					return;
				}
			}

			if (Step == 1) {
				//bounce physics!
				var scnView = (SCNView)this.View;
				var pTmp = scnView.ProjectPoint (new SCNVector3 (0, 0, -60));
				var p3d = scnView.UnprojectPoint (new SCNVector3 (p.X, p.Y, pTmp.Z));

				p3d.Y = 0;
				p3d.Z = 0;

				Explosion (p3d, Boxes, false);
			}
			if (Step == 3) {
				//bubbles
				MoveEmitter (p);
			}

			if (Step == 5) {
				//shader
				ShowNextShaderStage ();
			}
		}

		#endregion
	}
}