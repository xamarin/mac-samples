using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideManipulation : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Per-Frame Updates");
			TextManager.SetSubtitle ("Game Loop");

			var gameLoop = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/gameLoop", "png"), 17, false);
			gameLoop.Position = new SCNVector3 (0, 6, 10);
			GroundNode.AddChildNode (gameLoop);
		}
	}
}

