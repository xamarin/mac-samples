using System;
using System.Collections.Generic;

using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideLoadingDae : Slide
	{
		private List <SCNNode> NodesToDim { get; set; }

		private SCNNode DaeIcon { get; set; }

		private SCNNode AbcIcon { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Add some text
			TextManager.SetTitle ("Loading a 3D Scene");
			TextManager.SetSubtitle ("DAE Documents");

			NodesToDim = new List <SCNNode> ();

			TextManager.AddBulletAtLevel ("Geometries", 0);
			TextManager.AddBulletAtLevel ("Animations", 0);
			NodesToDim.Add (TextManager.AddBulletAtLevel ("Textures", 0));
			NodesToDim.Add (TextManager.AddBulletAtLevel ("Lighting", 0));
			NodesToDim.Add (TextManager.AddBulletAtLevel ("Cameras", 0));
			NodesToDim.Add (TextManager.AddBulletAtLevel ("Skinning", 0));
			NodesToDim.Add (TextManager.AddBulletAtLevel ("Morphing", 0));

			// And an image resting on the ground
			DaeIcon = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/dae file icon", "png"), 10, false);
			DaeIcon.Position = new SCNVector3 (6, 4.5f, 1);
			GroundNode.AddChildNode (DaeIcon);

			AbcIcon = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/abc file icon", "png"), 10, false);
			AbcIcon.Position = new SCNVector3 (6, 4.5f, 30);
			GroundNode.AddChildNode (AbcIcon);
		}

		public override int NumberOfSteps ()
		{
			return 2;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			if (index == 1) {
				presentationViewController.ShowsNewInSceneKitBadge (true);

				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);

				TextManager.SetSubtitle ("ABC Documents");

				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				foreach (var node in NodesToDim)
					node.Opacity = 0.5f;
				DaeIcon.Position = new SCNVector3 (6, 4.5f, -30);
				DaeIcon.Opacity = 0;
				AbcIcon.Position = new SCNVector3 (6, 4.5f, 1);
				SCNTransaction.Commit ();
			}
		}
	}
}

