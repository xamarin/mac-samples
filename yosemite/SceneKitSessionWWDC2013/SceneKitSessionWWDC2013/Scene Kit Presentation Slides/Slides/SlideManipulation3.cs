namespace SceneKitSessionWWDC2013 {
	public class SlideManipulation3 : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Manipulation");
			TextManager.SetSubtitle ("Code example");

			TextManager.AddCode ("#// Move a node to another position. \n"
			+ "aNode.#Position# = new SCNVector3 (0, 0, 0); \n"
			+ "aNode.#Scale#    = new SCNVector3 (2, 2, 2); \n"
			+ "aNode.#Rotation# = new SCNVector4 (x, y, z, angle); \n"
			+ "aNode.#Opacity#  = 0.5f;#");
		}
	}
}

