using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideReferTo2013 : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Related Sessions");

			// load the "related.png" image and show it mapped on a plane
			var relatedImage = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/related", "png"), 35, false);
			relatedImage.Position = new SCNVector3 (0, 10, 0);
			relatedImage.CastsShadow = false;
			GroundNode.AddChildNode (relatedImage);
		}
	}
}