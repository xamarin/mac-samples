using System;
using MonoMac.CoreFoundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideCreateAScene2 : Slide
	{
		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Creating a Scene");

			TextManager.AddBulletAtLevel ("Creating programmatically", 0);
			TextManager.AddBulletAtLevel ("Loading a scene from a file", 0);

			var delayInSeconds = 1.0;
			var popTime = new DispatchTime (DispatchTime.Now, (long)(delayInSeconds * Utils.NSEC_PER_SEC));
			DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
				TextManager.HighlightBullet (1);
			});
		}
	}
}

