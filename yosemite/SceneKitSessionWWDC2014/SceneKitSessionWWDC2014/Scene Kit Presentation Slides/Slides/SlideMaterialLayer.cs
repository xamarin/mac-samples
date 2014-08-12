using System;
using AppKit;
using SceneKit;
using CoreMedia;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using CoreGraphics;
using CoreAnimation;

namespace SceneKitSessionWWDC2014
{
	public class SlideMaterialLayer : Slide
	{
		private SlideMaterialLayer MaterialLayerSlideReference { get; set; }

		private AVPlayerLayer PlayerLayer1 { get; set; }

		private SCNMaterial Material { get; set; }

		private SCNNode Object { get; set; }

		private const int W = 8;

		public override int NumberOfSteps ()
		{
			return 5;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtitle and add some text
			TextManager.SetTitle ("Materials");
			TextManager.SetSubtitle ("Property contents");

			TextManager.AddBulletAtLevel ("Color", 0);
			TextManager.AddBulletAtLevel ("CGColorRef / NSColor / UIColor", 0);

			var node = SCNNode.Create ();
			node.Name = "material-cube";
			node.Geometry = SCNBox.Create (W, W, W, W * 0.02f);

			Material = node.Geometry.FirstMaterial;
			Material.Diffuse.Contents = NSColor.Red;

			Object = node;

			node.Position = new SCNVector3 (8, 11, 0);
			ContentNode.AddChildNode (node);
			node.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy ((float)Math.PI * 2, new SCNVector3 (0.4f, 1, 0), 4)));

			MaterialLayerSlideReference = this;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Code);

				TextManager.AddBulletAtLevel ("Image", 0);
				TextManager.AddBulletAtLevel ("Name / Path / URL", 1);
				TextManager.AddBulletAtLevel ("NSImage / UIImage / NSData", 1);
				TextManager.AddBulletAtLevel ("SKTexture", 1);

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);


				var code = TextManager.AddCode ("#material.diffuse.contents = #@\"slate.jpg\"#;#");
				code.Position = new SCNVector3 (code.Position.X + 6, code.Position.Y - 6.5f, code.Position.Z);

				Material.Diffuse.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/slate", "jpg"));
				Material.Normal.Contents = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/slate-bump", "png"));
				Material.Normal.Intensity = 0;
				break;
			case 2:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				Material.Normal.Intensity = 5.0f;
				Material.Specular.Contents = NSColor.Gray;
				SCNTransaction.Commit ();

				code = TextManager.AddCode ("#material.normal.contents = #[SKTexture textureByGeneratingNormalMap]#;#");
				code.Position = new SCNVector3 (code.Position.X + 2, code.Position.Y - 6.5f, code.Position.Z);
				break;
			case 3:
				TextManager.FadeOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddBulletAtLevel ("Live contents", 0);
				TextManager.AddBulletAtLevel ("CALayer tree", 1);
				TextManager.AddBulletAtLevel ("SKScene (new)", 1);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1.0f;
				Material.Normal.Intensity = 2.0f;
				SCNTransaction.Commit ();

				PlayerLayer1 = ConfigurePlayer (NSBundle.MainBundle.PathForResource ("Movies/movie1", "mov"), "material-cube");
				break;
			case 4:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddEmptyLine ();

				TextManager.AddBulletAtLevel ("Cube map", 0);
				TextManager.AddBulletAtLevel ("NSArray of 6 items", 1);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

				code = TextManager.AddCode ("#material.reflective.contents = #@[aright.png, left.png ... front.png]#;#");
				code.Position = new SCNVector3 (code.Position.X, code.Position.Y - 9.5f, code.Position.Z);

				var image = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/cubemap", "png"), 12, false);
				image.Position = new SCNVector3 (-10, 9, 0);
				image.Opacity = 0;
				ContentNode.AddChildNode (image);

				Object.Geometry = SCNTorus.Create (W * 0.5f, W * 0.2f);
				Material = Object.Geometry.FirstMaterial;
				Object.Rotation = new SCNVector4 (1, 0, 0, (float)Math.PI / 2);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				var right = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/right", "tga"));
				var left = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/left", "tga"));
				var top = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/top", "tga"));
				var bottom = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/bottom", "tga"));
				var back = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/back", "tga"));
				var front = new NSImage (NSBundle.MainBundle.PathForResource ("SharedTextures/front", "tga"));
				var materialContents = new NSMutableArray ();
				materialContents.AddObjects (new NSObject[] { right, left, top, bottom, back, front });
				Material.Reflective.Contents = materialContents;
				Material.Diffuse.Contents = NSColor.Red;
				image.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			}
		}

		// Load movies and display movie layers
		private AVPlayerLayer ConfigurePlayer (string movieName, string hostingNodeName)
		{
			var player = AVPlayer.FromUrl (NSUrl.FromFilename (movieName));
			player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None; // loop

			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector ("PlayerItemDidReachEnd"), AVPlayerItem.DidPlayToEndTimeNotification, player.CurrentItem);

			player.Play ();

			// Set an arbitrary frame. This frame will be the size of our movie texture so if it is too small it will appear scaled up and blurry, and if it is too big it will be slow
			var playerLayer = new AVPlayerLayer ();
			playerLayer.Player = player;
			playerLayer.ContentsGravity = AVPlayerLayer.GravityResizeAspectFill;
			playerLayer.Frame = new CGRect (0, 0, 600, 800);

			// Use a parent layer with a background color set to black
			// That way if the movie is stil loading and the frame is transparent, we won't see holes in the model
			var backgroundLayer = CALayer.Create ();
			backgroundLayer.BackgroundColor = NSColor.Black.CGColor; 
			backgroundLayer.Frame = new CGRect (0, 0, 600, 800);
			backgroundLayer.AddSublayer (playerLayer);

			var frameNode = ContentNode.FindChildNode (hostingNodeName, true);
			var material = frameNode.Geometry.FirstMaterial;
			material.Diffuse.Contents = backgroundLayer;

			return playerLayer;
		}

		[Export ("PlayerItemDidReachEnd")] 
		private void PlayerItemDidReachEnd (NSNotification notification)
		{
			var playerItem = (AVPlayerItem)notification.Object;
			playerItem.Seek (CMTime.Zero);
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			if (PlayerLayer1 != null) {
				NSNotificationCenter.DefaultCenter.RemoveObserver (this, AVPlayerItem.DidPlayToEndTimeNotification, PlayerLayer1.Player.CurrentItem);
				PlayerLayer1.Player.Pause ();
			}

			// Stop playing scene animations, restore the original point of view and restore the default spot light mode
			((SCNView)presentationViewController.View).Playing = false;
			((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
			presentationViewController.NarrowSpotlight (false);
		}
	}
}