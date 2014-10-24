using System;
using CoreGraphics;
using Foundation;
using SpriteKit;

namespace Bananas
{
	public class MainMenu : SKNode
	{
		SKSpriteNode gameLogo;

		public MainMenu (CGSize frameSize)
		{
			Position = new CGPoint (frameSize.Width * 0.5f, frameSize.Height * 0.15f);
			UserInteractionEnabled = true;

			gameLogo = new SKSpriteNode ("art.scnassets/level/interface/logo_bananas.png");

			// resize logo to fit the screen
			CGSize size = gameLogo.Size;
			nfloat factor = frameSize.Width / size.Width;
			size.Width *= factor;
			size.Height *= factor;
			gameLogo.Size = size;

			gameLogo.AnchorPoint = new CGPoint (1f, 0f);
			gameLogo.Position = new CGPoint (Frame.GetMidX (), Frame.GetMidY ());
			AddChild (gameLogo);
		}

		public void TouchUpAtPoint (CGPoint location)
		{
			Hidden = true;
			GameSimulation.Sim.SetGameState (GameState.InGame);
		}
	}

}

