using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideLabs : Slide
	{
		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			// Set the slide's title
			TextManager.SetTitle ("Labs");

			var relatedImage = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/labs", "png"), 35, false);
			relatedImage.Position = new SCNVector3 (0, 30, 0);
			relatedImage.CastsShadow = false;
			ContentNode.AddChildNode (relatedImage);
		}
	}
}

