namespace SceneKitSessionWWDC2013 {
	public class SlideRenderAScene : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Rendering a Scene");

			TextManager.AddBulletAtLevel ("Assign the scene to the renderer", 0);
			TextManager.AddBulletAtLevel ("Modifications of the scene graph are automatically reflected", 0);

			TextManager.AddCode ("#// Assign the scene \nSCNView.#Scene# = aScene;#");
		}
	}
}

