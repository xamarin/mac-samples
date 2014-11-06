using System;

using AppKit;
using SceneKit;
using CoreMedia;
using Foundation;
using ObjCRuntime;
using AVFoundation;
using CoreGraphics;
using CoreAnimation;

namespace SceneKitSessionWWDC2013
{
	public class SlideMaterialLayer : Slide
	{
		private AVPlayerLayer PlayerLayer1 { get; set; }

		private AVPlayerLayer PlayerLayer2 { get; set; }

		public override int NumberOfSteps ()
		{
			return 2;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Materials");
				TextManager.SetSubtitle ("CALayer as texture");

				TextManager.AddCode ("#// Map a layer tree on a 3D object. \n"
				+ "aNode.Geometry.FirstMaterial.Diffuse.#Contents# = #aLayerTree#;#");

				// Add the model
				var intermediateNode = SCNNode.Create ();
				intermediateNode.Position = new SCNVector3 (0, 3.9f, 8);
				intermediateNode.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));
				GroundNode.AddChildNode (intermediateNode);
				Utils.SCAddChildNode (intermediateNode, "frames", "Scenes/frames/frames", 8);

				presentationViewController.NarrowSpotlight (true);
				break;
			case 1:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
						
				// Change the point of view to "frameCamera" (a camera defined in the "frames" scene)
				var frameCamera = ContentNode.FindChildNode ("frameCamera", true);
				((SCNView)presentationViewController.View).PointOfView = frameCamera;

				// The "frames" scene contains animations, update the end time of our main scene and start to play the animations
				((SCNView)presentationViewController.View).Scene.SetAttribute (new NSNumber (7.33f), SCNScene.EndTimeAttributeKey);
				((SCNView)presentationViewController.View).CurrentTime = 0;
				((SCNView)presentationViewController.View).Playing = true;
				((SCNView)presentationViewController.View).Loops = true;

				PlayerLayer1 = ConfigurePlayer (NSBundle.MainBundle.PathForResource ("Movies/movie1", "mov"), "PhotoFrame-Vertical");
				PlayerLayer2 = ConfigurePlayer (NSBundle.MainBundle.PathForResource ("Movies/movie2", "mov"), "PhotoFrame-Horizontal");

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

			var frameNode = GroundNode.FindChildNode (hostingNodeName, true);
			var material = frameNode.Geometry.Materials [1];
			material.Diffuse.Contents = backgroundLayer;

			return playerLayer;
		}

		[Export ("PlayerItemDidReachEnd:")]
		private void PlayerItemDidReachEnd (NSNotification notification)
		{
			var playerItem = (AVPlayerItem)notification.Object;
			playerItem.Seek (CMTime.Zero);
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, AVPlayerItem.DidPlayToEndTimeNotification, PlayerLayer1.Player.CurrentItem);
			NSNotificationCenter.DefaultCenter.RemoveObserver (this, AVPlayerItem.DidPlayToEndTimeNotification, PlayerLayer2.Player.CurrentItem);

			PlayerLayer1.Player.Pause ();
			PlayerLayer2.Player.Pause ();

			//playerLayer1.Player = null;
			//playerLayer2.Player = null;

			// Stop playing scene animations, restore the original point of view and restore the default spot light mode
			((SCNView)presentationViewController.View).Playing = false;
			((SCNView)presentationViewController.View).PointOfView = presentationViewController.CameraNode;
			presentationViewController.NarrowSpotlight (false);
		}
	}
}