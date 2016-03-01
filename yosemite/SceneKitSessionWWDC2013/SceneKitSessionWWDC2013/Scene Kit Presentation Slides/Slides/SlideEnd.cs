using SceneKit;

namespace SceneKitSessionWWDC2013 {
	public class SlideEnd : Slide {
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			var labelNode = Utils.SCLabelNode ("XAMARIN 2014", Utils.LabelSize.Normal, false);
			labelNode.Position = new SCNVector3 (-3.6f, 29.5f, 20);

			// Reducing the text's flatness makes it smoother, less tesselated
			var text = (SCNText)labelNode.Geometry;
			text.Flatness = 0.1f;

			ContentNode.AddChildNode (labelNode);
		}
	}
}

