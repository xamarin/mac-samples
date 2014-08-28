using System;
using SceneKit;
using CoreImage;
using Foundation;
using CoreGraphics;

namespace SceneKitSessionWWDC2014
{
	public class SlideCoreImage : Slide
	{
		private CGSize ViewportSize { get; set; }

		public override int NumberOfSteps ()
		{
			return 1;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Setup the image grid here to benefit from the preloading mechanism
			ViewportSize = presentationViewController.View.ConvertSizeToBacking (presentationViewController.View.Frame.Size);
		}

		public override void DidOrderIn (PresentationViewController presentationViewController)
		{
			var banana = Utils.SCAddChildNode (ContentNode, "banana", "Scenes.scnassets/banana/banana", 5);
			banana.Rotation = new SCNVector4 (1, 0, 0, -(float)(Math.PI / 2));

			banana.RunAction (SCNAction.RepeatActionForever (SCNAction.RotateBy (0, NMath.PI * 2, 0, 1.5f)));
			banana.Position = new SCNVector3 (2.5f, 5, 10);
			var gaussianBlurFilter = new CIGaussianBlur () { Radius = 10 };
			gaussianBlurFilter.SetDefaults ();
			banana.Filters = new CIFilter[] { gaussianBlurFilter };

			banana = (SCNNode)banana.Copy ();
			ContentNode.AddChildNode (banana);
			banana.Position = new SCNVector3 (6, 5, 10);
			var pixellateFilter = new CIPixellate ();
			pixellateFilter.SetDefaults ();
			banana.Filters = new CIFilter[] { pixellateFilter };

			banana = (SCNNode)banana.Copy ();
			ContentNode.AddChildNode (banana);
			banana.Position = new SCNVector3 (9.5f, 5, 10);
			var filter = CIFilter.FromName ("CIEdgeWork");
			filter.SetDefaults ();
			banana.Filters = new CIFilter[] { filter };
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtitle and add some text
				TextManager.SetTitle ("Core Image");
				TextManager.SetSubtitle ("CI Filters");

				TextManager.AddBulletAtLevel ("Screen-space effects", 0);
				TextManager.AddBulletAtLevel ("Applies to a node hierarchy", 0);
				TextManager.AddBulletAtLevel ("Filter parameters are animatable", 0);
				TextManager.AddCode ("#aNode.#filters# = @[filter1, filter2];#");
				break;
			}
		}

	}
}

