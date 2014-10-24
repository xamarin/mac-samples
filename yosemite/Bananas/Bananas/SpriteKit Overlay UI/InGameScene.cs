using System;
using CoreGraphics;
using Foundation;
using SpriteKit;

namespace Bananas
{
	public enum GameState
	{
		PreGame = 0,
		InGame,
		Paused,
		PostGame,
		Count
	}

	public class InGameScene : SKScene
	{
		SKLabelNode scoreLabel;
		SKLabelNode scoreLabelValue;
		SKLabelNode scoreLabelShadow;
		SKLabelNode scoreLabelValueShadow;

		SKLabelNode timeLabel;
		SKLabelNode timeLabelValue;
		SKLabelNode timeLabelShadow;
		SKLabelNode timeLabelValueShadow;

		MainMenu menuNode;
		PauseMenu pauseNode;
		PostGameMenu postGameNode;

		GameState gameState;

		public IGameUIState GameStateDelegate { get; set; }

		public InGameScene (CGSize size) : base (size)
		{
			BackgroundColor = AppKit.NSColor.FromCalibratedRgba (0.15f, 0.15f, 0.3f, 1f);

			timeLabel = LabelWithText ("Time", 24);
			AddChild (timeLabel);

			CGRect af = timeLabel.CalculateAccumulatedFrame ();
			timeLabel.Position = new CGPoint (Frame.Size.Width - af.Size.Width, Frame.Size.Height - (af.Size.Height));

			timeLabelValue = LabelWithText ("102:00", 20);
			AddChild (timeLabelValue);
			CGRect timeLabelValueSize = timeLabelValue.CalculateAccumulatedFrame ();
			timeLabelValue.Position = new CGPoint (Frame.Size.Width - af.Size.Width - timeLabelValueSize.Size.Width - 10, Frame.Size.Height - af.Size.Height);

			scoreLabel = LabelWithText ("Score", 24);
			AddChild (scoreLabel);
			af = scoreLabel.CalculateAccumulatedFrame ();
			scoreLabel.Position = new CGPoint (af.Size.Width * 0.5f, Frame.Size.Height - af.Size.Height);

			scoreLabelValue = LabelWithText ("0", 24);
			AddChild (scoreLabelValue);
			scoreLabelValue.Position = new CGPoint (af.Size.Width * 0.75f + (timeLabelValueSize.Size.Width), Frame.Size.Height - af.Size.Height);

			// Add drop shadows to each label above.
			timeLabelValueShadow = DropShadowOnLabel (timeLabelValue);
			timeLabelShadow = DropShadowOnLabel (timeLabel);

			scoreLabelShadow = DropShadowOnLabel (scoreLabel);
			scoreLabelValueShadow = DropShadowOnLabel (scoreLabelValue);
		}

		public void SetGameState (GameState state)
		{
			if (menuNode != null)
				menuNode.RemoveFromParent ();
			if (pauseNode != null)
				pauseNode.RemoveFromParent ();
			if (postGameNode != null)
				postGameNode.RemoveFromParent ();

			if (state == GameState.PreGame) {
				menuNode = new MainMenu (Frame.Size);
				AddChild (menuNode);
			} else if (state == GameState.InGame) {
				HideInGameUI (false);
			} else if (state == GameState.Paused) {
				pauseNode = new PauseMenu (Frame.Size);
				AddChild (pauseNode);
			} else if (state == GameState.PostGame) {
				InvokeOnMainThread (() => {
					postGameNode = new PostGameMenu (Frame.Size, GameStateDelegate);
					AddChild (postGameNode);
					HideInGameUI (true);
				});
			}

			gameState = state;
		}

		public void HideInGameUI (bool hide)
		{
			scoreLabel.Hidden = hide;
			scoreLabelValue.Hidden = hide;
			scoreLabelShadow.Hidden = hide;
			scoreLabelValueShadow.Hidden = hide;

			timeLabel.Hidden = hide;
			timeLabelValue.Hidden = hide;
			timeLabelShadow.Hidden = hide;
			timeLabelValueShadow.Hidden = hide;
		}

		public override void Update (double currentTime)
		{
			GameStateDelegate.ScoreLabelLocation = scoreLabelValue.Position;
			scoreLabelValue.Text = GameStateDelegate.Score.ToString ();
			scoreLabelValueShadow.Text = scoreLabelValue.Text;

			if (GameStateDelegate.SecondsRemaining > 60) {
				int minutes = (int)(GameStateDelegate.SecondsRemaining / 60.0f);
				int seconds = (int)Math.Floor (GameStateDelegate.SecondsRemaining % 60.0);
				timeLabelValue.Text = string.Format ("{0}:{1}", minutes, seconds);
				timeLabelValueShadow.Text = timeLabelValue.Text;
			} else {
				int seconds = (int)Math.Floor (GameStateDelegate.SecondsRemaining % 60.0);
				timeLabelValue.Text = string.Format ("0:{0}", seconds);
				timeLabelValueShadow.Text = timeLabelValue.Text;
			}
		}

		public void TouchUpAtPoint (CGPoint location)
		{
			if (gameState == GameState.Paused) {
				pauseNode.TouchUpAtPoint (location);
			} else if (gameState == GameState.PostGame) {
				postGameNode.TouchUpAtPoint (location);
			} else if (gameState == GameState.PreGame) {
				menuNode.TouchUpAtPoint (location);
			} else if (gameState == GameState.InGame) {
				SKNode touchedNode = Scene.GetNodeAtPoint (location);

				if (touchedNode == timeLabelValue)
					GameSimulation.Sim.SetGameState (GameState.Paused);
			}
		}

		public static SKLabelNode DropShadowOnLabel (SKLabelNode frontLabel)
		{
			SKLabelNode myLabelBackground = (SKLabelNode)frontLabel.Copy ();
			myLabelBackground.UserInteractionEnabled = false;
			myLabelBackground.FontColor = AppKit.NSColor.Black;
			myLabelBackground.Position = new CGPoint (2 + frontLabel.Position.X, -2 + frontLabel.Position.Y);
			myLabelBackground.ZPosition = frontLabel.ZPosition - 1;
			frontLabel.Parent.AddChild (myLabelBackground);
			return myLabelBackground;
		}

		public static SKLabelNode LabelWithText (string text, int textSize)
		{
			string fontName = "Optima-ExtraBlack";
			SKLabelNode myLabel = new SKLabelNode (fontName);

			myLabel.Text = text;
			myLabel.FontSize = textSize;
			myLabel.FontColor = AppKit.NSColor.Yellow;

			return myLabel;
		}
	}
}

