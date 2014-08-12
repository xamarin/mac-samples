using System;

namespace SceneKitSessionWWDC2014
{
	public class SlideAnimationOutline : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Animating a Scene");
			TextManager.SetSubtitle ("Outline");

			TextManager.AddBulletAtLevel ("Per-Frame Updates", 0);
			TextManager.AddBulletAtLevel ("Animations", 0);
			TextManager.AddBulletAtLevel ("Actions", 0);
			TextManager.AddBulletAtLevel ("Physics", 0);
			TextManager.AddBulletAtLevel ("Constraints", 0);
		}
	}
}

