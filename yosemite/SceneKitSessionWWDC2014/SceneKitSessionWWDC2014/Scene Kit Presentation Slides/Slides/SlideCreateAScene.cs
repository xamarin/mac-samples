using System;

namespace SceneKitSessionWWDC2014
{
	public class SlideCreateAScene : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Creating a Scene");

			TextManager.AddBulletAtLevel ("Creating programmatically", 0);
			TextManager.AddBulletAtLevel ("Loading a scene from a file", 0);
		}
	}
}

