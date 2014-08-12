using System;
using Foundation;
using SceneKit;

namespace SceneKitSessionWWDC2014
{
	public class SlideSampleLoadingDae : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Loading a DAE");
			TextManager.SetSubtitle ("Sample code");

			TextManager.AddCode ("#// Load a DAE"
			+ "\n"
			+ "var scene = SCNScene.#FromFile# (\"yourPath\");#");

			var image = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/daeAsResource", "png"), 9, false);
			image.Position = new SCNVector3 (0, 3.2f, 7);
			GroundNode.AddChildNode (image);
		}
	}
}

