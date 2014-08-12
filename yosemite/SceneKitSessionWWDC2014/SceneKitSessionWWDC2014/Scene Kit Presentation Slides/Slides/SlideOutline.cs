using System;

namespace SceneKitSessionWWDC2014
{
	public class SlideOutline : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Outline");

			TextManager.AddBulletAtLevel ("Scene graph overview", 0);
			TextManager.AddBulletAtLevel ("Getting started", 0);
			TextManager.AddBulletAtLevel ("Animating", 0);
			TextManager.AddBulletAtLevel ("Rendering", 0);
			TextManager.AddBulletAtLevel ("Effects", 0);
		}
	}
}

