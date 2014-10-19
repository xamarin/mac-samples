using System;

#if __IOS__
using UIKit;
#else
using AppKit;
#endif
using SpriteKit;
using Foundation;
using CoreGraphics;

namespace SceneKitReel
{
	public class SpriteKitOverlayScene : SKScene
	{
		public SKNode NextButton { get; set; }

		public SKNode PreviousButton { get; set; }

		public SKNode ButtonGroup { get; set; }

		SKLabelNode Label { get; set; }

		public SpriteKitOverlayScene (IntPtr handle) : base (handle) {}

		public SpriteKitOverlayScene (CGSize size)
		{
			Size = size;

			AnchorPoint = new CGPoint (0.5f, 0.5f);
			ScaleMode = SKSceneScaleMode.ResizeFill;

			//buttons
			NextButton = SKSpriteNode.FromImageNamed (NSBundle.MainBundle.PathForResource ("images/next", "png"));

			var marginY = 60.0f;
			var maringX = -60.0f;
			#if __IOS__
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				marginY = 30;
				marginY = 30;
			}
			#endif

			NextButton.Position = new CGPoint (size.Width * 0.5f + maringX, -size.Height * 0.5f + marginY);
			NextButton.Name = "next";
			NextButton.Alpha = 0.01f;
			#if __IOS__
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
				NextButton.XScale = NextButton.YScale = 0.5f;
			}
			#endif
			AddChild (NextButton);

			#if __IOS__
			PreviousButton = SKSpriteNode.FromColor (UIColor.Clear, NextButton.Frame.Size);
			#else
			PreviousButton = SKSpriteNode.FromColor (NSColor.Clear, NextButton.Frame.Size);
			#endif
			PreviousButton.Position = new CGPoint (-(size.Width * 0.5f + maringX), -size.Height * 0.5f + marginY);
			PreviousButton.Name = "back";
			PreviousButton.Alpha = 0.01f;
			AddChild (PreviousButton);
		}

		public void ShowLabel (string label)
		{
			if (Label == null) {
				Label = SKLabelNode.FromFont ("Myriad Set");
				if (Label == null)
					Label = SKLabelNode.FromFont ("Avenir-Heavy");
				Label.FontSize = 140;
				Label.Position = new CGPoint (0, 0);

				AddChild (Label);
			} else {
				if (label != null)
					Label.Position = new CGPoint (0, Size.Height * 0.25f);
			}

			if (label == null) {
				Label.RunAction (SKAction.FadeOutWithDuration (0.5));
			} else { 
				#if __IOS__
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
					Label.FontSize = label.Length > 10 ? 50 : 80;
				else
				#endif
				Label.FontSize = label.Length > 10 ? 100 : 140;

				Label.Text = label;
				Label.Alpha = 0.0f;
				Label.RunAction (SKAction.Sequence (new SKAction [] {
					SKAction.WaitForDuration (0.5),
					SKAction.FadeInWithDuration (0.5)
				}));
			}
		}
	}
}

