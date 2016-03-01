namespace SceneKitSessionWWDC2013 {
	public class SlideSampleLoadingDae : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Loading a DAE");
			TextManager.SetSubtitle ("Sample code");

			TextManager.AddCode ("#// Load a DAE"
			+ "\n"
			+ "var scene = SCNScene.#FromFile# (\"yourPath\");#");
		}
	}
}

