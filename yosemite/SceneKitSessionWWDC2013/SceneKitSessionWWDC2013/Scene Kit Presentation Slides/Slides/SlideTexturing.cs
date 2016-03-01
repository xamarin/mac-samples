namespace SceneKitSessionWWDC2013 {
	public class SlideTexturing : Slide {
		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtile
				TextManager.SetTitle ("Performance");
				TextManager.SetSubtitle ("Texturing");

				// From now on, every text we add will fade in
				TextManager.FadesIn = true;
				break;
			case 1:
				TextManager.AddBulletAtLevel ("Avoid unnecessarily large images", 0);
				break;
			case 2:
				TextManager.AddBulletAtLevel ("Lock ambient and diffuse", 0);
				TextManager.AddCode ("#aMaterial.#LocksAmbientWithDiffuse# = true;#");
				break;
			case 3:
				TextManager.AddBulletAtLevel ("Use mipmaps", 0);
				TextManager.AddCode ("#aMaterial.Diffuse.#MipFilter# = #SCNFilterMode.Linear#;#");
				break;
			}
		}
	}
}

