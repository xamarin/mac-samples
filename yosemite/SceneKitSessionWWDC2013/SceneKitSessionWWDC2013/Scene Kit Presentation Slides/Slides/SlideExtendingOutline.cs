using System;

namespace SceneKitSessionWWDC2013
{
	public class SlideExtendingOutline : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Extending Scene Kit with OpenGL");

			TextManager.AddBulletAtLevel ("Scene delegate rendering", 0);
			TextManager.AddBulletAtLevel ("Node delegate rendering", 0);
			TextManager.AddBulletAtLevel ("Material custom program", 0);
			TextManager.AddBulletAtLevel ("Shader modifiers", 0);
		}
	}
}

