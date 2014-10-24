using System;
using CoreGraphics;
using Foundation;
using SpriteKit;

namespace Bananas
{
	public class PauseMenu : SKNode
	{
		SKLabelNode myLabel;

		public PauseMenu (CGSize frameSize)
		{
			myLabel = InGameScene.LabelWithText ("Resume", 65);
			myLabel.Position = new CGPoint (Frame.GetMidX (), Frame.GetMidY ());

			Position = new CGPoint (frameSize.Width * 0.5f, frameSize.Height * 0.5f);
			AddChild (myLabel);
			InGameScene.DropShadowOnLabel (myLabel);
		}

		public void TouchUpAtPoint (CGPoint location)
		{
			SKNode touchedNode = Scene.GetNodeAtPoint (location);

			if (touchedNode == myLabel) {
				Hidden = true;
				GameSimulation.Sim.SetGameState (GameState.InGame);
			}
		}
	}

}

