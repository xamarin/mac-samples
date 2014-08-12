using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using AppKit;
using SceneKit;
using Foundation;
using CoreGraphics;
using CoreAnimation;
using CoreFoundation;
using Newtonsoft.Json;

namespace SceneKitSessionWWDC2014
{
	public delegate void PresentationDelegate ();

	public class PresentationViewController : NSViewController
	{
		public enum Light
		{
			Main = 0,
			Front,
			Spot,
			Left,
			Right,
			Ambient,
			Count
		}

		public class Slides
		{
			public string Class { get; set; }

			public Slide Slide { get; set; }
		}

		public class Settings
		{
			public string Name { get; set; }

			public Slides[] Slides { get; set; }
		}

		public IPresentation PresentationDelegate { get; set; }

		// Keeping track of the current slide
		private int CurrentSlideIndex { get; set; }

		private int CurrentSlideStep { get; set; }

		// The scene used for this presentation
		private SCNScene Scene { get; set; }

		// Light nodes
		private SCNNode[] Lights = new SCNNode [(int)Light.Count];

		// Nodes used to control the position and orientation of the main camera
		public SCNNode CameraHandle { get; private set; }

		public SCNNode CameraPitch { get; private set; }
		// child of 'cameraHandle'
		public SCNNode CameraNode { get; private set; }
		// child of 'cameraPitch'

		// Managing the floor
		private SCNFloor Floor { get; set; }

		private NSImage FloorImage { get; set; }

		// Presentation settings and slides
		private Settings SlideSettings { get; set; }

		private Dictionary <string, Slide> SlideCache { get; set; }

		// Managing the "New" badge
		private SCNNode NewBadgeNode { get; set; }

		private CAAnimation NewBadgeAnimation { get; set; }

		public override NSView View {
			get {
				return (SCNView)base.View;
			}
			set {
				base.View = value;
			}
		}

		public PresentationViewController (string path)
		{
			// Load the presentation settings from the plist file
			var settingsPath = NSBundle.MainBundle.PathForResource (path, "xml");
			SlideSettings = JsonConvert.DeserializeObject<Settings> (File.ReadAllText (settingsPath));

			SlideCache = new Dictionary <string, Slide> ();

			// Create a new empty scene
			Scene = new SCNScene ();

			// Create and add a camera to the scene
			// We create three separate nodes to ease the manipulation of the global position, pitch (ie. orientation around the x axis) and relative position
			// - cameraHandle is used to control the global position in world space
			// - cameraPitch  is used to rotate the position around the x axis
			// - cameraNode   is sometimes manipulated by slides to move the camera relatively to the global position (cameraHandle). But this node is supposed to always be repositioned at (0, 0, 0) in the end of a slide.

			CameraHandle = SCNNode.Create ();
			CameraHandle.Name = "cameraHandle";
			Scene.RootNode.AddChildNode (CameraHandle);

			CameraPitch = SCNNode.Create ();
			CameraPitch.Name = "cameraPitch";
			CameraHandle.AddChildNode (CameraPitch);

			CameraNode = SCNNode.Create ();
			CameraNode.Name = "cameraNode";
			CameraNode.Camera = new SCNCamera ();

			// Set the default field of view to 70 degrees (a relatively strong perspective)
			CameraNode.Camera.XFov = 70.0;
			CameraNode.Camera.YFov = 42.0;
			CameraPitch.AddChildNode (CameraNode);

			// Setup the different lights
			InitLighting ();

			// Create and add a reflective floor to the scene
			var floorMaterial = new SCNMaterial ();
			floorMaterial.Ambient.Contents = NSColor.Black;
			floorMaterial.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/floor", "png"));
			floorMaterial.Diffuse.ContentsTransform = SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (0, 0, 1), (float)Math.PI / 4);
			floorMaterial.Specular.WrapS = 
				floorMaterial.Specular.WrapT = 
					floorMaterial.Diffuse.WrapS = 
						floorMaterial.Diffuse.WrapT = SCNWrapMode.Mirror;

			Floor = SCNFloor.Create ();
			Floor.ReflectionFalloffEnd = 3.0f;
			Floor.FirstMaterial = floorMaterial;

			var floorNode = SCNNode.Create ();
			floorNode.Geometry = Floor;
			Scene.RootNode.AddChildNode (floorNode);

			floorNode.PhysicsBody = SCNPhysicsBody.CreateStaticBody (); //make floor dynamic for physics slides
			Scene.PhysicsWorld.Speed = 0; //pause physics to avoid continuous drawing

			// Use a shader modifier to support a secondary texture for some slides
			var shaderFile = NSBundle.MainBundle.PathForResource ("Shaders/floor", "shader");
			var surfaceModifier = File.ReadAllText (shaderFile);
			floorMaterial.ShaderModifiers = new SCNShaderModifiers { EntryPointSurface = surfaceModifier };

			// Set the scene to the view
			View = new SCNView (CGRect.Empty);
			((SCNView)View).Scene = Scene;
			((SCNView)View).BackgroundColor = NSColor.Black;

			// black fog
			Scene.FogColor = NSColor.FromCalibratedWhite (0, 1);
			Scene.FogEndDistance = 45;
			Scene.FogStartDistance = 40;

			// Turn on jittering for better anti-aliasing when the scene is still
			((SCNView)View).JitteringEnabled = true;

			// Start the presentation
			GoToSlide (0);
		}

		public int NumberOfSlides {
			get {
				return SlideSettings.Slides.Count ();
			}
		}

		public Type ClassOfSlide (int slideIndex)
		{
			var className = SlideSettings.Slides [slideIndex].Class;
			return Type.GetType ("SceneKitSessionWWDC2014." + className);
		}


		// This method creates and initializes the slide at the specified index and returns it.
		// The new slide is cached in the _slides array.
		private Slide GetSlide (int slideIndex, bool loadIfNeeded)
		{
			if (slideIndex < 0 || slideIndex >= SlideSettings.Slides.Count ())
				return null;

			// Look into the cache first
			if (SlideCache.ContainsKey (slideIndex.ToString ()))
				return SlideCache [slideIndex.ToString ()];

			if (!loadIfNeeded)
				return null;

			// Create the new slide
			Type slideClass = ClassOfSlide (slideIndex);
			var slide = (Slide)Activator.CreateInstance (slideClass);

			// Update its parameters
			var slideSettings = SlideSettings.Slides [slideIndex].Slide;
			if (slideSettings != null)
				slide.Fetch (slideSettings);

			SlideCache [slideIndex.ToString ()] = slide;

			if (slide == null)
				return null;

			// Setup the slide
			slide.SetupSlide (this);

			return slide;
		}

		// Preload the next slide
		private void PrepareSlide (int slideIndex)
		{
			// Retrieve the slide to preload
			var slide = GetSlide (slideIndex, true);

			if (slide != null) {
				SCNTransaction.Flush (); // make sure that all pending transactions are flushed otherwise objects not added yet to the scene graph would not be preloaded

				// Preload the node tree
				// ((SCNView)View).Prepare (slide.contentNode, null);

				// Preload the floor image if any
				if (slide.FloorImageName != null) {
					NSImage image;
					try {
						image = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/" + slide.FloorImageName, "png"));
					} catch (Exception) {
						image = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/" + slide.FloorImageName, "jpg"));
					}

					// Create a container for this image to be able to preload it
					var material = SCNMaterial.Create ();
					material.Diffuse.Contents = image;
					material.Diffuse.MipFilter = SCNFilterMode.Linear; // we also want to preload mipmaps

					SCNTransaction.Flush (); //make this material ready before warming up

					// Preload
					//((SCNView)View).Prepare (material, null);

					// Don't release the material now, otherwise we will loose what we just preloaded
					slide.FloorWarmupMaterial = material;
				}
			}
		}

		public void GoToNextSlideStep ()
		{
			var slide = GetSlide (CurrentSlideIndex, false);
			if (CurrentSlideStep + 1 >= slide.NumberOfSteps ())
				GoToSlide (CurrentSlideIndex + 1);
			else
				GoToSlideStep (CurrentSlideStep + 1);
		}

		public void GoToPreviousSlide ()
		{
			GoToSlide (CurrentSlideIndex - 1);
		}

		public void GoToSlide (int slideIndex)
		{
			int oldIndex = CurrentSlideIndex;

			// Load the slide at the specified index
			var slide = GetSlide (slideIndex, true);

			if (slide == null)
				return;

			// Compute the playback direction (did the user select next or previous?)
			var direction = slideIndex >= CurrentSlideIndex ? 1 : -1;

			// Update badge
			ShowsNewInSceneKitBadge (slide.IsNewIn10_10);

			// If we are playing backward, we need to use the slide we come from to play the correct transition (backward)
			int transitionSlideIndex = direction == 1 ? slideIndex : CurrentSlideIndex;
			var transitionSlide = GetSlide (transitionSlideIndex, true);

			// Make sure that the next operations are synchronized by using a transaction
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0;

			var rootNode = slide.ContentNode;
			var textContainer = slide.TextManager.TextNode;

			var offset = new SCNVector3 (transitionSlide.TransitionOffsetX, 0.0f, transitionSlide.TransitionOffsetZ);
			offset.X *= direction;
			offset.Z *= direction;

			// Rotate offset based on current yaw
			var cosa = Math.Cos (-CameraHandle.Rotation.W);
			var sina = Math.Sin (-CameraHandle.Rotation.W);

			var tmpX = offset.X * cosa - offset.Z * sina;
			offset.Z = (float)(offset.X * sina + offset.Z * cosa);
			offset.X = (float)tmpX;

			// If we don't move, fade in
			if (offset.X == 0 && offset.Y == 0 && offset.Z == 0 && transitionSlide.TransitionRotation == 0)
				rootNode.Opacity = 0;

			// Don't animate the first slide
			bool shouldAnimate = !(slideIndex == 0 && CurrentSlideIndex == 0);

			// Update current slide index
			CurrentSlideIndex = slideIndex;

			// Go to step 0
			GoToSlideStep (0);

			// Add the slide to the scene graph
			((SCNView)View).Scene.RootNode.AddChildNode (rootNode);

			// Fade in, update paramters and notify on completion
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = (shouldAnimate ? slide.TransitionDuration : 0);
			SCNTransaction.SetCompletionBlock (() => DidOrderInSlide (slideIndex));
			rootNode.Opacity = 1;

			CameraHandle.Position = new SCNVector3 (CameraHandle.Position.X + offset.X, slide.Altitude, CameraHandle.Position.Z + offset.Z);
			CameraHandle.Rotation = new SCNVector4 (0, 1, 0, (float)(CameraHandle.Rotation.W + transitionSlide.TransitionRotation * (Math.PI / 180.0f) * direction));
			CameraPitch.Rotation = new SCNVector4 (1, 0, 0, (float)(slide.Pitch * (Math.PI / 180.0f)));

			UpdateLightingForSlide (slideIndex);

			Floor.Reflectivity = slide.FloorReflectivity;
			Floor.ReflectionFalloffEnd = slide.FloorFalloff;
			SCNTransaction.Commit ();

			// Compute the position of the text (in world space, relative to the camera)
			var textWorldTransform = SCNMatrix4.Mult (SCNMatrix4.CreateTranslation (0, -3.3f, -28), CameraNode.WorldTransform);

			// Place the rest of the slide
			rootNode.Transform = textWorldTransform;
			rootNode.Position = new SCNVector3 (rootNode.Position.X, 0, rootNode.Position.Z); // clear altitude
			rootNode.Rotation = new SCNVector4 (0, 1, 0, CameraHandle.Rotation.W); // use same rotation as the camera to simplify the placement of the elements in slides

			// Place the text
			textContainer.Transform = textContainer.ParentNode.ConvertTransformFromNode (textWorldTransform, null);

			// Place the ground node
			var localPosition = new SCNVector3 (0, 0, 0);
			var worldPosition = slide.GroundNode.ParentNode.ConvertPositionToNode (localPosition, null);
			worldPosition.Y = 0; // make it touch the ground

			localPosition = slide.GroundNode.ParentNode.ConvertPositionFromNode (worldPosition, null);
			slide.GroundNode.Position = localPosition;

			// Update the floor image if needed
			string floorImagePath = null;
			NSImage floorImage = null;
			if (slide.FloorImageName != null) {
				floorImagePath = NSBundle.MainBundle.PathForResource ("SharedTextures/" + slide.FloorImageName, slide.FloorImageExtension);
				floorImage = new NSImage (floorImagePath);
			}
			UpdateFloorImage (floorImage, slide);

			SCNTransaction.Commit ();

			// Preload the next slide after some delay
			var delayInSeconds = 1.5;
			var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * Utils.NSEC_PER_SEC));
			DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
				PrepareSlide (slideIndex + 1);
			});

			// Order out previous slide if any
			if (oldIndex != CurrentSlideIndex)
				WillOrderOutSlide (oldIndex);
		}

		private void GoToSlideStep (int index)
		{
			CurrentSlideStep = index;

			var slide = GetSlide (CurrentSlideIndex, true);
			if (slide == null)
				return;

			if (PresentationDelegate != null)
				PresentationDelegate.WillPresentSlide (CurrentSlideIndex, CurrentSlideStep);

			slide.PresentStep (CurrentSlideStep, this);
		}

		private void DidOrderInSlide (int slideIndex)
		{
			var slide = GetSlide (slideIndex, false);
			if (slide != null)
				slide.DidOrderIn (this);
		}

		private void WillOrderOutSlide (int slideIndex)
		{
			var slide = GetSlide (slideIndex, false);
			if (slide != null) {
				var node = slide.ContentNode;

				// Fade out and remove on completion
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				SCNTransaction.SetCompletionBlock (() => node.RemoveFromParentNode ());
				node.Opacity = 0.0f;
				SCNTransaction.Commit ();

				slide.WillOrderOut (this);

				SlideCache.Remove (slideIndex.ToString ());
			}
		}

		public void ShowsNewInSceneKitBadge (bool showsBadge)
		{
			if (NewBadgeNode != null && showsBadge)
				return; // already visible

			if (NewBadgeNode == null && !showsBadge)
				return; // already invisible

			// Load the model and the animation
			if (NewBadgeNode == null) {
				NewBadgeNode = SCNNode.Create ();

				var badgeNode = Utils.SCAddChildNode (NewBadgeNode, "newBadge", "Scenes.scnassets/newBadge", 1);
				NewBadgeNode.Scale = new SCNVector3 (0.03f, 0.03f, 0.03f);
				NewBadgeNode.Opacity = 0;
				NewBadgeNode.Position = new SCNVector3 (50, 20, -10);
				NewBadgeNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));

				var imageNode = NewBadgeNode.FindChildNode ("badgeImage", true);
				imageNode.Geometry.FirstMaterial.Emission.Intensity = 0.0f;

				CameraPitch.AddChildNode (NewBadgeNode);

				NewBadgeAnimation = badgeNode.GetAnimation (badgeNode.GetAnimationKeys () [0]);
				badgeNode.RemoveAllAnimations ();

				NewBadgeAnimation.Speed = 1.5f;
				NewBadgeAnimation.FillMode = CAFillMode.Both;
				NewBadgeAnimation.UsesSceneTimeBase = false;
				NewBadgeAnimation.RemovedOnCompletion = false;
				NewBadgeAnimation.RepeatCount = 0;
			}

			// Play
			if (showsBadge) {
				NewBadgeNode.AddAnimation (NewBadgeAnimation, new NSString ("badgeAnimation"));

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 2;
				NewBadgeNode.Position = new SCNVector3 (14, 8, -20);

				SCNTransaction.SetCompletionBlock (() => {
					SCNTransaction.Begin (); 
					SCNTransaction.AnimationDuration = 3;
					if (NewBadgeNode != null) {
						SCNNode ropeNode = NewBadgeNode.FindChildNode ("rope02", true);
						ropeNode.Opacity = 0.0f;
					}
					SCNTransaction.Commit ();
				});

				NewBadgeNode.Opacity = 1.0f;
				SCNNode imageNode = NewBadgeNode.FindChildNode ("badgeImage", true);
				imageNode.Geometry.FirstMaterial.Emission.Intensity = 0.4f;
				SCNTransaction.Commit ();
			}

			// Or hide
			else {
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.5f;
				SCNTransaction.SetCompletionBlock (() => {
					if (NewBadgeNode != null) {
						NewBadgeNode.RemoveFromParentNode ();
						NewBadgeNode = null;
					}
				});
				NewBadgeNode.Position = new SCNVector3 (14, 50, -20);
				NewBadgeNode.Opacity = 0.0f;
				SCNTransaction.Commit ();
			}
		}

		public void InitLighting ()
		{
			// Omni light (main light of the scene)
			Lights [(int)Light.Main] = new SCNNode { 
				Name = "omni", 
				Position = new SCNVector3 (0, 3, -13) 
			};

			Lights [(int)Light.Main].Light = new SCNLight {
				LightType = SCNLightType.Omni,
				AttenuationStartDistance = 10,
				AttenuationEndDistance = 50,
				Color = NSColor.Black
			};

			CameraHandle.AddChildNode (Lights [(int)Light.Main]); //make all lights relative to the camera node


			// Front light
			Lights [(int)Light.Front] = new SCNNode {
				Name = "front light",
				Position = new SCNVector3 (0, 0, 0)
			};

			Lights [(int)Light.Front].Light = new SCNLight {
				LightType = SCNLightType.Directional,
				Color = NSColor.Black
			};

			CameraHandle.AddChildNode (Lights [(int)Light.Front]);


			// Spot light
			Lights [(int)Light.Spot] = new SCNNode {
				Name = "spot light",
				Transform = SCNMatrix4.Mult (SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (1, 0, 0), -(nfloat)(Math.PI / 2) * 0.8f), SCNMatrix4.CreateFromAxisAngle (new SCNVector3 (0, 0, 1), -0.3f)),
				Position = new SCNVector3 (0, 30, -19),
				Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2))
			};

			Lights [(int)Light.Spot].Light = new SCNLight {
				LightType = SCNLightType.Spot,
				ShadowRadius = 3,
				ZNear = 20,
				ZFar = 100,
				Color = NSColor.Black,
				CastsShadow = true
			};

			NarrowSpotlight (false);
			CameraHandle.AddChildNode (Lights [(int)Light.Spot]);


			// Left light
			Lights [(int)Light.Left] = new SCNNode {
				Name = "left light",
				Position = new SCNVector3 (-20, 10, -20),
				Rotation = new SCNVector4 (0, 1, 0, (float)(Math.PI / 2))
			};

			Lights [(int)Light.Left].Light = new SCNLight {
				LightType = SCNLightType.Omni,
				AttenuationStartDistance = 30,
				AttenuationEndDistance = 80,
				Color = NSColor.Black
			};

			CameraHandle.AddChildNode (Lights [(int)Light.Left]);


			// Right light
			Lights [(int)Light.Right] = new SCNNode {
				Name = "right light",
				Position = new SCNVector3 (20, 10, -20)
			};

			Lights [(int)Light.Right].Light = new SCNLight {
				LightType = SCNLightType.Omni,
				AttenuationStartDistance = 30,
				AttenuationEndDistance = 80,
				Color = NSColor.Black
			};

			CameraHandle.AddChildNode (Lights [(int)Light.Right]);


			// Ambient light
			Lights [(int)Light.Ambient] = new SCNNode {
				Name = "ambient light"
			};

			Lights [(int)Light.Ambient].Light = new SCNLight {
				LightType = SCNLightType.Ambient,
				Color = NSColor.FromCalibratedWhite (0.0f, 1.0f)
			};

			Scene.RootNode.AddChildNode (Lights [(int)Light.Ambient]);
		}

		private void UpdateLightingForSlide (int slideIndex)
		{
			var slide = GetSlide (slideIndex, true);

			Lights [(int)Light.Main].Position = slide.MainLightPosition;

			UpdateLightingWithIntensities (slide.LightIntensities);
		}

		public void UpdateLightingWithIntensities (float[] intensities)
		{
			for (int i = 0; i < (int)Light.Count; i++) {
				float intensity = intensities.Length > i ? intensities [i] : 0;
				Lights [i].Light.Color = NSColor.FromDeviceHsba (GetLightHueForSlide (i), GetLightSaturationForSlide (i), intensity, 1);
			}
		}

		public void NarrowSpotlight (bool narrow)
		{
			if (narrow) {
				Lights [(int)Light.Spot].Light.SetAttribute (new NSNumber (20), SCNLightAttribute.SpotInnerAngleKey);
				Lights [(int)Light.Spot].Light.SetAttribute (new NSNumber (30), SCNLightAttribute.SpotOuterAngleKey);
			} else {
				Lights [(int)Light.Spot].Light.SetAttribute (new NSNumber (10), SCNLightAttribute.SpotInnerAngleKey);
				Lights [(int)Light.Spot].Light.SetAttribute (new NSNumber (45), SCNLightAttribute.SpotOuterAngleKey);
			}
		}

		public void RiseMainLight (bool rise)
		{
			if (rise) {
				Lights [(int)Light.Main].Light.SetAttribute (new NSNumber (90), SCNLightAttribute.AttenuationStartKey);
				Lights [(int)Light.Main].Light.SetAttribute (new NSNumber (250), SCNLightAttribute.AttenuationEndKey);
				Lights [(int)Light.Main].Position = new SCNVector3 (0, 10, -10);
			} else {
				Lights [(int)Light.Main].Light.SetAttribute (new NSNumber (10), SCNLightAttribute.AttenuationStartKey);
				Lights [(int)Light.Main].Light.SetAttribute (new NSNumber (50), SCNLightAttribute.AttenuationEndKey);
				Lights [(int)Light.Main].Position = new SCNVector3 (0, 3, -13);
			}
		}

		public SCNNode SpotLight {
			get { 
				return Lights [(int)Light.Spot];
			}
		}

		public SCNNode MainLight {
			get {
				return Lights [(int)Light.Main];
			}
		}

		// Updates the secondary image of the floor if needed
		private void UpdateFloorImage (NSImage image, Slide slide)
		{
			// We don't want to animate if we replace the secondary image by a new one
			// Otherwise we want to translate the secondary image to the new location
			var disableAction = false;

			if (FloorImage != image) {
				FloorImage = image;
				disableAction = true;

				if (image != null) {
					// Set a new material property with this image to the "floorMap" custom property of the floor
					var property = SCNMaterialProperty.Create (image);
					property.WrapS = SCNWrapMode.Repeat;
					property.WrapT = SCNWrapMode.Repeat;
					property.MipFilter = SCNFilterMode.Linear;

					Floor.FirstMaterial.SetValueForKey (property, new NSString ("floorMap"));
				}
			}

			if (image != null) {
				var slidePosition = slide.GroundNode.ConvertPositionToNode (new SCNVector3 (0, 0, 10), null);

				if (disableAction) {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 0;
					Floor.FirstMaterial.SetValueForKey (NSValue.FromVector (slidePosition), new NSString ("floorImageNamePosition"));
					SCNTransaction.Commit ();
				} else
					Floor.FirstMaterial.SetValueForKey (NSValue.FromVector (slidePosition), new NSString ("floorImageNamePosition"));
			}
		}

		private float GetLightSaturationForSlide (int index)
		{
			if (index >= 4)
				return 0.1f; // colored
			return 0; // black and white
		}

		private float GetLightHueForSlide (int index)
		{
			if (index == 4)
				return 0; // red
			if (index == 5)
				return (float)(200 / 360.0f); // blue
			return 0; // black and white
		}
	}
}

