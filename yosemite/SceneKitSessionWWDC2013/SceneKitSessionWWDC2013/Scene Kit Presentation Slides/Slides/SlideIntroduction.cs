namespace SceneKitSessionWWDC2013 {
	public class SlideIntroduction : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Kit");
			TextManager.AddBulletAtLevel ("Introduced in Mountain Lion", 0);
			TextManager.AddBulletAtLevel ("Eases the integration of 3D into applications", 0);
		}
	}
}

