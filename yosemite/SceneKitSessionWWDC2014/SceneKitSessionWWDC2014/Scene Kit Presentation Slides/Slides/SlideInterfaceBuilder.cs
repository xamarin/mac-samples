using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideInterfaceBuilder : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Add some text
			TextManager.SetTitle ("Displaying the Scene");
			TextManager.SetSubtitle ("Game Template");

			TextManager.AddBulletAtLevel ("Start with the Xcode game template", 0);
			TextManager.AddBulletAtLevel ("Or drag an SCNView from the library", 0);

			// And an image
			var imageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/Interface Builder", "png"), 8.3f, false);
			imageNode.Position = new SCNVector3 (-4.0f, 3.2f, 11.0f);
			ContentNode.AddChildNode (imageNode);

			imageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/game_big", "png"), 7, false);
			imageNode.Position = new SCNVector3 (5.0f, 3.5f, 11.0f);
			imageNode.Geometry.FirstMaterial.Diffuse.MagnificationFilter = SCNFilterMode.Nearest;
			ContentNode.AddChildNode (imageNode);
		}
	}
}

