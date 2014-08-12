using System;

namespace SceneKitSessionWWDC2013
{
	public class SlideManipulation : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Manipulation");
			TextManager.SetSubtitle ("Basics");

			TextManager.AddBulletAtLevel ("Retrieve nodes", 0);
			TextManager.AddBulletAtLevel ("Move, scale and rotate", 0);
			TextManager.AddBulletAtLevel ("Animate", 0);
			TextManager.AddBulletAtLevel ("Change colors and images", 0);
			TextManager.AddBulletAtLevel ("Change lighting", 0);
			TextManager.AddBulletAtLevel ("Clone", 0);
		}
	}
}

