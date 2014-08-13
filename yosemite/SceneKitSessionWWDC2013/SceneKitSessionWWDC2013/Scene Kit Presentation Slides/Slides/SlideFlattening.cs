using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideFlattening : Slide
	{
		public override int NumberOfSteps ()
		{
			return 2;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text.
				TextManager.SetTitle ("Performance");
				TextManager.SetSubtitle ("Flattening");

				TextManager.AddBulletAtLevel ("Flatten node tree into single node", 0);
				TextManager.AddBulletAtLevel ("Minimize draw calls", 0);

				TextManager.AddCode ("#// Flatten node hierarchy \n"
				+ "var flattenedNode = aNode.#FlattenedClone# ();#");

				break;
			case 1:
				// Discard the text and show a 2D image.
				// Animate the image's position when it appears.

				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				var imageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/flattening", "png"), 20, false);
				imageNode.Position = new SCNVector3 (0, 4.8f, 16);
				GroundNode.AddChildNode (imageNode);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				imageNode.Position = new SCNVector3 (0, 4.8f, 8);
				SCNTransaction.Commit ();
				break;
			}
		}
	}
}

