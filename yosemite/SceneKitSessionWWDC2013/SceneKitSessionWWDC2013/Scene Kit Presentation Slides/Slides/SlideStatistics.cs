using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013 {
	public class SlideStatistics : Slide {
		SCNNode FpsNode { get; set; }

		SCNNode PanelNode { get; set; }

		SCNNode ButtonNode { get; set; }

		SCNNode WindowNode { get; set; }

		public override int NumberOfSteps ()
		{
			return 6;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				// Set the slide's title and subtile and add some code
				TextManager.SetTitle ("Performance");
				TextManager.SetSubtitle ("Statistics");

				TextManager.AddCode ("#// Show statistics \n"
				+ "aSCNView.#ShowsStatistics# = true;");
				break;
			case 1:
				// Place a screenshot in the scene and animate it
				WindowNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/statistics/statistics", "png"), 20, true);
				ContentNode.AddChildNode (WindowNode);

				WindowNode.Opacity = 0.0f;
				WindowNode.Position = new SCNVector3 (20, 5.2f, 9);
				WindowNode.Rotation = new SCNVector4 (0, 1, 0, -(float)(Math.PI / 4));

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 1;
				WindowNode.Opacity = 1.0f;
				WindowNode.Position = new SCNVector3 (0, 5.2f, 7);
				WindowNode.Rotation = new SCNVector4 (0, 1, 0, 0);
				SCNTransaction.Commit ();

				// The screenshot contains transparent areas so we need to make sure it is rendered
				// after the text (which also sets its rendering order)
				WindowNode.RenderingOrder = 2;

				break;
			case 2:
				FpsNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/statistics/statistics-fps", "png"), 7, false);
				WindowNode.AddChildNode (FpsNode);

				FpsNode.Scale = new SCNVector3 (0.75f, 0.75f, 0.75f);
				FpsNode.Opacity = 0.0f;
				FpsNode.Position = new SCNVector3 (-6, -3, 0.5f);
				FpsNode.RenderingOrder = 4;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				FpsNode.Scale = new SCNVector3 (1.0f, 1.0f, 1.0f);
				FpsNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case 3:
				ButtonNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/statistics/statistics-button", "png"), 4, false);
				WindowNode.AddChildNode (ButtonNode);

				ButtonNode.Scale = new SCNVector3 (0.75f, 0.75f, 0.75f);
				ButtonNode.Opacity = 0.0f;
				ButtonNode.Position = new SCNVector3 (-7.5f, -2.75f, 0.5f);
				ButtonNode.RenderingOrder = 5;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				FpsNode.Opacity = 0.0f;
				ButtonNode.Scale = new SCNVector3 (1.0f, 1.0f, 1.0f);
				ButtonNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case 4:
				PanelNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/statistics/control-panel", "png"), 10, false);
				WindowNode.AddChildNode (PanelNode);

				PanelNode.Scale = new SCNVector3 (0.75f, 0.75f, 0.75f);
				PanelNode.Opacity = 0.0f;
				PanelNode.Position = new SCNVector3 (3.5f, -0.5f, 1.5f);
				PanelNode.RenderingOrder = 6;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				PanelNode.Scale = new SCNVector3 (1.0f, 1.0f, 1.0f);
				PanelNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case 5:
				var detailsNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/statistics/statistics-detail", "png"), 9, false);
				WindowNode.AddChildNode (detailsNode);

				detailsNode.Scale = new SCNVector3 (0.75f, 0.75f, 0.75f);
				detailsNode.Opacity = 0.0f;
				detailsNode.Position = new SCNVector3 (5, -2.75f, 1.5f);
				detailsNode.RenderingOrder = 7;

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.5f;
				PanelNode.Opacity = 0.0f;
				ButtonNode.Opacity = 0.0f;
				detailsNode.Scale = new SCNVector3 (1.0f, 1.0f, 1.0f);
				detailsNode.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			}
		}
	}
}