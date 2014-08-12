using System;

namespace SceneKitSessionWWDC2013
{
	public class SlideOutline : Slide
	{
		public override int NumberOfSteps ()
		{
			return 6;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title
				// And make the bullets we will add later to fade in
				TextManager.SetTitle ("Outline");
				TextManager.FadesIn = true;
				break;
			case 1:
				TextManager.AddBulletAtLevel ("Scene graph", 0);
				break;
			case 2:
				TextManager.AddBulletAtLevel ("Build an application with Scene Kit", 0);
				break;
			case 3:
				TextManager.AddBulletAtLevel ("Extending with OpenGL", 0);
				break;
			case 4:
				TextManager.AddBulletAtLevel ("What’s new in OS X 10.9", 0);
				break;
			case 5:
				TextManager.AddBulletAtLevel ("Performance notes", 0);
				break;
			}
		}
	}
}

