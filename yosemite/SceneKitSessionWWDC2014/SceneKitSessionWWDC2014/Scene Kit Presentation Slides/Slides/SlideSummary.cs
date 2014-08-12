using System;

namespace SceneKitSessionWWDC2014
{
	public class SlideSummary : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Summary");
			TextManager.AddBulletAtLevel ("SceneKit available on iOS", 0);
			TextManager.AddBulletAtLevel ("Casual game ready", 0);
			TextManager.AddBulletAtLevel ("Full featured rendering", 0);
			TextManager.AddBulletAtLevel ("Extendable", 0);
		}
	}
}

