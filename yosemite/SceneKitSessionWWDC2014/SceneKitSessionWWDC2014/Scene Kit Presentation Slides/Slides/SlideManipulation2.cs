using System;

namespace SceneKitSessionWWDC2014
{
	public class SlideManipulation2 : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Manipulation");
			TextManager.SetSubtitle ("Animations");

			TextManager.AddBulletAtLevel ("Properties are animatable", 0);
			TextManager.AddBulletAtLevel ("Implicit and explicit animations", 0);
			TextManager.AddBulletAtLevel ("Same programming model as Core Animation", 0);
		}
	}
}

