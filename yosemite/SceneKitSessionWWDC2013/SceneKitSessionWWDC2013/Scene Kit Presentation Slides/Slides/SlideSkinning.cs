using System;
using System.IO;

using SceneKit;
using Foundation;
using ObjCRuntime;
using CoreAnimation;
using CoreFoundation;

namespace SceneKitSessionWWDC2013 {
	public class SlideSkinning : Slide {
		CAAnimationGroup IdleAnimationGroup { get; set; }

		CAAnimationGroup AnimationGroup1 { get; set; }

		CAAnimationGroup AnimationGroup2 { get; set; }

		SCNNode CharacterNode { get; set; }

		SCNNode SkeletonNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 5;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Using a scene source allows us to retrieve the animations using their identifier
			var path = NSBundle.MainBundle.PathForResource ("Scenes/skinning/skinning", "dae");
			var sceneURL = NSUrl.FromFilename (path);
			var sceneSource = SCNSceneSource.FromUrl (sceneURL, (NSDictionary)null);

			// Place the character in the scene
			var error = new NSError ();
			var scene = sceneSource.SceneWithOption ((SCNSceneLoadingOptions)null, out error);
			CharacterNode = scene.RootNode.FindChildNode ("avatar_attach", true);
			CharacterNode.Scale = new SCNVector3 (0.004f, 0.004f, 0.004f);
			CharacterNode.Position = new SCNVector3 (5, 0, 12);
			CharacterNode.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 8));
			CharacterNode.Hidden = true;
			GroundNode.AddChildNode (CharacterNode);

			SkeletonNode = CharacterNode.FindChildNode ("skeleton", true);

			// Prepare the other resources
			//TODO LoadGhostEffect ();
			ExtractAnimation (sceneSource);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();

			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Skinning");

				TextManager.AddBulletAtLevel ("Animate characters", 0);
				TextManager.AddBulletAtLevel ("Deform geometries with a skeleton", 0);
				TextManager.AddBulletAtLevel ("Joints and bones", 0);

				// Animate the character
				CharacterNode.AddAnimation (IdleAnimationGroup, new NSString ("idleAnimation"));

				// The character is hidden. Wait a little longer before showing it
				// otherwise it may slow down the transition from the previous slide
				var delayInSeconds = 1.5;
				var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * Utils.NSEC_PER_SEC));
				DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 0;
					CharacterNode.Hidden = false;
					CharacterNode.Opacity = 0;
					SCNTransaction.Commit ();

					SCNTransaction.Begin ();
					SCNTransaction.AnimationDuration = 1.5;
					SCNTransaction.AnimationTimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseIn);
					CharacterNode.Opacity = 1;
					SCNTransaction.Commit ();
				});
				break;
			case 1:
				SCNTransaction.AnimationDuration = 1.5f;
				//TODO SetShowsBones (true);
				break;
			case 2:
				CharacterNode.AddAnimation (AnimationGroup1, new NSString ("animation"));
				break;
			case 3:
				SCNTransaction.AnimationDuration = 1.5f;
				//TODO SetShowsBones (false);
				break;
			case 4:
				CharacterNode.AddAnimation (AnimationGroup1, new NSString ("animation"));
				break;
			}
			SCNTransaction.Commit ();
		}

		private void ExtractAnimation (SCNSceneSource sceneSource)
		{
			// In this scene objects are animated separately using long animations
			// playing 3 successive animations. We will group these long animations
			// and then split the group in 3 different animation groups.
			// We could also have used three DAEs (one per animation).

			var animationIDs = sceneSource.GetIdentifiersOfEntries (new Class ("CAAnimation"));

			var animationCount = animationIDs.Length;
			var longAnimations = new CAAnimation [animationCount];

			var maxDuration = 0.0;

			for (var index = 0; index < animationCount; index++) {
				var animation = (CAAnimation)sceneSource.GetEntryWithIdentifier (animationIDs [index].ToString (), new Class ("CAAnimation"));
				if (animation != null) {
					maxDuration = Math.Max (maxDuration, animation.Duration);
					longAnimations [index] = animation;
				}
			}

			var longAnimationsGroup = new CAAnimationGroup ();
			longAnimationsGroup.Animations = longAnimations;
			longAnimationsGroup.Duration = maxDuration;

			var idleAnimationGroup = (CAAnimationGroup)longAnimationsGroup.Copy ();
			idleAnimationGroup.TimeOffset = 6.45833333333333f;
			IdleAnimationGroup = CAAnimationGroup.CreateAnimation ();
			IdleAnimationGroup.Animations = new CAAnimation[] { idleAnimationGroup };
			IdleAnimationGroup.Duration = 24.71f - 6.45833333333333f;
			IdleAnimationGroup.RepeatCount = float.MaxValue;
			IdleAnimationGroup.AutoReverses = true;

			var animationGroup1 = (CAAnimationGroup)longAnimationsGroup.Copy ();
			AnimationGroup1 = CAAnimationGroup.CreateAnimation ();
			AnimationGroup1.Animations = new CAAnimation[] { animationGroup1 };
			AnimationGroup1.Duration = 1.4f;
			AnimationGroup1.FadeInDuration = 0.1f;
			AnimationGroup1.FadeOutDuration = 0.5f;

			var animationGroup2 = (CAAnimationGroup)longAnimationsGroup.Copy ();
			animationGroup2.TimeOffset = 3.666666666666667f;
			AnimationGroup2 = CAAnimationGroup.CreateAnimation ();
			AnimationGroup2.Animations = new CAAnimation[] { animationGroup2 };
			AnimationGroup2.Duration = 6.416666666666667f - 3.666666666666667f;
			AnimationGroup2.FadeInDuration = 0.1f;
			AnimationGroup2.FadeOutDuration = 0.5f;
		}

		private void LoadGhostEffect ()
		{
			var shaderFile = NSBundle.MainBundle.PathForResource ("Shaders/character", "shader");
			var fragmentModifier = File.ReadAllText (shaderFile);
			SetShaderModifiers (new SCNShaderModifiers { EntryPointFragment = fragmentModifier }, CharacterNode);
		}

		private void ApplyGhostEffect (bool show, SCNNode node)
		{
			// Uniforms in your GLSL shaders can be set using KVC
			// The following line will set the 'ghostFactor' uniform found in the 'character.shader' file
			node.Geometry.SetValueForKey (new NSNumber (show), new NSString ("ghostFactor"));

			foreach (var child in node.ChildNodes)
				ApplyGhostEffect (show, child);
		}

		private void SetShaderModifiers (SCNShaderModifiers modifiers, SCNNode node)
		{
			node.Geometry.ShaderModifiers = modifiers;

			foreach (var childNode in node.ChildNodes)
				SetShaderModifiers (modifiers, childNode);
		}

		private void SetShowsBones (bool show)
		{
			VisualizeBones (show, SkeletonNode, 1);
			ApplyGhostEffect (show, CharacterNode);
		}

		private void VisualizeBones (bool show, SCNNode node, nfloat scale)
		{
			// We propagate an inherited scale so that the boxes
			// representing the bones will be of the same size
			scale *= node.Scale.X;

			if (show) {
				if (node.Geometry == null)
					node.Geometry = SCNBox.Create (6.0f / scale, 6.0f / scale, 6.0f / scale, 0.5f);
			} else {
				if (node.Geometry.GetType () == typeof(SCNBox))
					node.Geometry = null;
			}

			//foreach (SCNNode child in node.ChildNodes)
			//VisualizeBones (show, child, scale);
		}
	}
}

