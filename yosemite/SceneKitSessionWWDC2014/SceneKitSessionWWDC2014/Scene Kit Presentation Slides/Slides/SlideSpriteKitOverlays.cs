using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideSpriteKitOverlays : Slide
	{
		public override int NumberOfSteps ()
		{
			return 2;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("SpriteKit Overlays");

			TextManager.AddBulletAtLevel ("Game score, gauges, time, menus...", 0);
			TextManager.AddBulletAtLevel ("Event handling", 0);

			var node = TextManager.AddCode ("#scnView.#overlaySKScene# = aSKScene;#");
			node.Position = new SCNVector3 (9, 0, 0);

			var gameLoop = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/overlays", "png"), 10, false);
			gameLoop.Position = new SCNVector3 (0, 2.9f, 13);
			GroundNode.AddChildNode (gameLoop);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch(index){
			case 0:
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddEmptyLine ();
				TextManager.AddBulletAtLevel ("Portability", 0);
				TextManager.AddBulletAtLevel ("Performance", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			}
		}
	}
}

