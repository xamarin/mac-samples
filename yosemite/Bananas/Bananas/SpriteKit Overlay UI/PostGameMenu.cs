using System;
using CoreGraphics;
using Foundation;
using SpriteKit;

namespace Bananas
{
	public class PostGameMenu : SKNode
	{
		SKLabelNode myLabel;
		SKLabelNode bananaText;
		SKLabelNode bananaScore;
		SKLabelNode coinText;
		SKLabelNode coinScore;
		SKLabelNode totalText;
		SKLabelNode totalScore;

		IGameUIState GameStateDelegate { get; set; }

		public PostGameMenu (CGSize frameSize, IGameUIState gameStateDelegate)
		{
			GameStateDelegate = gameStateDelegate;
			nfloat menuHeight = frameSize.Height * 0.8f;
			var background = new SKSpriteNode (AppKit.NSColor.Black, new CGSize (frameSize.Width * 0.8f, menuHeight));

			background.ZPosition = -1;
			background.Alpha = 0.5f;
			background.Position = new CGPoint (0, -0.2f * menuHeight);

			myLabel = InGameScene.LabelWithText ("Final Score", 65);
			myLabel.Position = new CGPoint (Frame.GetMidX (), Frame.GetMidY ());

			Position = new CGPoint (frameSize.Width * 0.5f, frameSize.Height * 0.5f);
			UserInteractionEnabled = true;
			myLabel.UserInteractionEnabled = true;
			AddChild (myLabel);
			InGameScene.DropShadowOnLabel (myLabel);

			var bananaLocation = new CGPoint (frameSize.Width * -0.4f, Frame.GetMidY () * -0.4f);
			var	coinLocation = new CGPoint (frameSize.Width * -0.4f, Frame.GetMidY () * -0.6f);
			var totalLocation = new CGPoint (frameSize.Width * -0.4f, Frame.GetMidY () * -0.8f);
			var bananaScoreLocation = new CGPoint (frameSize.Width * +0.4f, Frame.GetMidY () * -0.4f);
			var coinScoreLocation = new CGPoint (frameSize.Width * +0.4f, Frame.GetMidY () * -0.6f);
			var totalScoreLocation = new CGPoint (frameSize.Width * +0.4f, Frame.GetMidY () * -0.8f);

			bananaText = (SKLabelNode)myLabel.Copy ();
			bananaText.Text = "Bananas";
			bananaText.FontSize = 0.1f * menuHeight;
			bananaText.SetScale(0.8f);
			bananaLocation.X += bananaText.CalculateAccumulatedFrame ().Width * 0.5f + frameSize.Width * 0.1f;
			bananaText.Position = new CGPoint (bananaLocation.X, -2000);
			AddChild (bananaText);
			InGameScene.DropShadowOnLabel (bananaText);

			bananaScore = (SKLabelNode)bananaText.Copy ();
			bananaScore.Text = "000";
			bananaScoreLocation.X -= bananaScore.CalculateAccumulatedFrame ().Width * 0.5f + frameSize.Width * 0.1f;
			bananaScore.Position = new CGPoint (bananaScoreLocation.X, -2000);
			AddChild (bananaScore);

			coinText = (SKLabelNode)bananaText.Copy ();
			coinText.Text = "Large Bananas";
			coinLocation.X += coinText.CalculateAccumulatedFrame ().Width * 0.5f + frameSize.Width * 0.1f;
			coinText.Position = new CGPoint (coinLocation.X, -2000f);
			AddChild (coinText);
			InGameScene.DropShadowOnLabel (coinText);

			coinScore = (SKLabelNode)coinText.Copy ();
			coinScore.Text = "000";
			coinScoreLocation.X -= coinScore.CalculateAccumulatedFrame ().Width * 0.5f + frameSize.Width * 0.1f;
			coinScore.Position = new CGPoint (coinScoreLocation.X, -2000f);
			AddChild (coinScore);

			totalText = (SKLabelNode)bananaText.Copy ();
			totalText.Text = "Total";
			totalLocation.X += totalText.CalculateAccumulatedFrame ().Width * 0.5f + frameSize.Width * 0.1f;
			totalText.Position = new CGPoint (totalLocation.X, -2000f);
			AddChild (totalText);
			InGameScene.DropShadowOnLabel (totalText);

			totalScore = (SKLabelNode)totalText.Copy ();
			totalScore.Text = "000";
			totalScoreLocation.X -= totalScore.CalculateAccumulatedFrame ().Width * 0.5f + frameSize.Width * 0.1f;
			totalScore.Position = new CGPoint (totalScoreLocation.X, -2000f);
			AddChild (totalScore);

			SKAction flyup = SKAction.MoveTo (new CGPoint (frameSize.Width * 0.5f, frameSize.Height - 100), 0.25);
			flyup.TimingMode = SKActionTimingMode.EaseInEaseOut;

			SKAction flyupBananas = SKAction.MoveTo (bananaLocation, 0.25);
			flyupBananas.TimingMode = SKActionTimingMode.EaseInEaseOut;

			SKAction flyupBananasScore = SKAction.MoveTo (bananaScoreLocation, 0.25f);
			flyupBananasScore.TimingMode = SKActionTimingMode.EaseInEaseOut;

			SKAction flyupCoins = SKAction.MoveTo (coinLocation, 0.25);
			flyupCoins.TimingMode = SKActionTimingMode.EaseInEaseOut;

			SKAction flyupCoinsScore = SKAction.MoveTo (coinScoreLocation, 0.25);
			flyupCoinsScore.TimingMode = SKActionTimingMode.EaseInEaseOut;

			SKAction flyupTotal = SKAction.MoveTo (totalLocation, 0.25);
			flyupTotal.TimingMode = SKActionTimingMode.EaseInEaseOut;

			SKAction flyupTotalScore = SKAction.MoveTo (totalScoreLocation, 0.25);
			flyupTotalScore.TimingMode = SKActionTimingMode.EaseInEaseOut;

			int bananasCollected = gameStateDelegate.BananasCollected;
			int coinsCollected = gameStateDelegate.CoinsCollected;
			int totalCollected = bananasCollected + (coinsCollected * 100);

			SKAction countUpBananas = SKAction.CustomActionWithDuration (bananasCollected / 100f, ((node, elapsedTime) => {
				if (bananasCollected > 0) {
					SKLabelNode label = (SKLabelNode)node;
					nint total = (nint)(elapsedTime / (bananasCollected / 100.0f) * bananasCollected);
					label.Text = total.ToString ();
						
					if (total % 10 == 0)
						GameSimulation.Sim.PlaySound ("deposit.caf");
				}
			}));

			SKAction countUpCoins = SKAction.CustomActionWithDuration (coinsCollected / 100f, ((node, elapsedTime) => {
				if (coinsCollected > 0) {
					SKLabelNode label = (SKLabelNode)node;
					nint total = (nint)((elapsedTime / (coinsCollected / 100.0f)) * coinsCollected);
					label.Text = total.ToString ();

					if (total % 10 == 0)
						GameSimulation.Sim.PlaySound ("deposit.caf");
				}
			}));

			SKAction countUpTotal = SKAction.CustomActionWithDuration (totalCollected / 500.0f, ((node, elapsedTime) => {
				if (totalCollected > 0) {
					SKLabelNode label = (SKLabelNode)node;
					nint total = (nint)((elapsedTime / (totalCollected / 500.0f)) * totalCollected);
					label.Text = total.ToString ();

					if (total % 25 == 0)
						GameSimulation.Sim.PlaySound ("deposit.caf");
				}
			}));

			RunAction (flyup, () => {
				bananaText.RunAction (flyupBananas);
				bananaScore.RunAction (flyupBananasScore, () => {
					bananaScore.RunAction (countUpBananas, () => {
						bananaScore.Text = bananasCollected.ToString ();
						coinText.RunAction (flyupCoins);
						coinScore.RunAction (flyupCoinsScore, () => {
							coinScore.RunAction (countUpCoins, () => {
								coinScore.Text = coinsCollected.ToString ();
								totalText.RunAction (flyupTotal);
								totalScore.RunAction (flyupTotalScore, () => {
									totalScore.RunAction (countUpTotal, () => {
										totalScore.Text = totalCollected.ToString ();
									});
								});
							});
						});
					});
				});
			});
		}

		public void TouchUpAtPoint (CGPoint location)
		{
			SKNode touchedNode = Scene.GetNodeAtPoint (location);

			if (touchedNode != null) {
				Hidden = true;
				GameSimulation.Sim.SetGameState (GameState.InGame);
			}
		}
	}

}

