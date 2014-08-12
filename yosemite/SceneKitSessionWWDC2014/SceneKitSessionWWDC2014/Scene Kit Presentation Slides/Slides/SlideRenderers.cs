using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideRenderers : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Displaying the Scene");

			// Add labels
			var node = Utils.SCLabelNode ("SCNView", Utils.LabelSize.Normal, false);
			node.Position = new SCNVector3 (-14, 8, 0);
			ContentNode.AddChildNode (node);

			node = Utils.SCLabelNode (" SCNLayer\n(OS X only)", Utils.LabelSize.Normal, false);
			node.Position = new SCNVector3 (-2.2f, 7, 0);
			ContentNode.AddChildNode (node);

			node = Utils.SCLabelNode ("SCNRenderer", Utils.LabelSize.Normal, false);
			node.Position = new SCNVector3 (9.5f, 8, 0);
			ContentNode.AddChildNode (node);

			// Add images - SCNView
			var box = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/renderer-window", "png"), 8, true);
			box.Position = new SCNVector3 (-10, 3, 5);
			ContentNode.AddChildNode (box);

			box = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/teapot", "tiff"), 6, true);
			box.Position = new SCNVector3 (-10, 3, 5.1f);
			ContentNode.AddChildNode (box);

			// Add images - SCNLayer
			box = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/renderer-layer", "png"), 7.4f, true);
			box.Position = new SCNVector3 (0, 3.5f, 5);
			box.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI / 20));
			ContentNode.AddChildNode (box);

			box = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/teapot", "tiff"), 6, true);
			box.Position = new SCNVector3 (0, 3.5f, 5.1f);
			box.Rotation = new SCNVector4 (0, 0, 1, (float)(Math.PI / 20));
			ContentNode.AddChildNode (box);

			// Add images - SCNRenderer
			box = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/renderer-framebuffer", "png"), 8, true);
			box.Position = new SCNVector3 (10, 3.2f, 5);
			ContentNode.AddChildNode (box);

			box = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/teapot", "tiff"), 6, true);
			box.Position = new SCNVector3 (10, 3, 5.1f);
			ContentNode.AddChildNode (box);
		}
	}
}

