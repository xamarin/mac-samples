namespace SceneKitSessionWWDC2013 {
	public class SlideMoreInfo : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("More Information");

			TextManager.AddTextAtLevel ("Allan Schaffer", 0);
			TextManager.AddTextAtLevel ("Graphics and Game Technologies Evangelist", 1);
			TextManager.AddTextAtLevel ("aschaffer@apple.com", 2);
			TextManager.AddEmptyLine ();

			TextManager.AddTextAtLevel ("Documentation", 0);
			TextManager.AddTextAtLevel ("Scene Kit Programming Guide", 1);
			TextManager.AddTextAtLevel ("http://developer.apple.com", 2);
			TextManager.AddEmptyLine ();

			TextManager.AddTextAtLevel ("Apple Developer Forums", 0);
			TextManager.AddTextAtLevel ("http://devforums.apple.com", 2);
			TextManager.AddEmptyLine ();
		}
	}
}

