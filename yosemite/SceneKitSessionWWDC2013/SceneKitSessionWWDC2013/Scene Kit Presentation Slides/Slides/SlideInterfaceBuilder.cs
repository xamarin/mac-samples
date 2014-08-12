using System;
using MonoMac.SceneKit;
using MonoMac.Foundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideInterfaceBuilder : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Add some text
			TextManager.SetTitle ("Renderers");
			TextManager.SetSubtitle ("View creation in Interface Builder");

			TextManager.AddBulletAtLevel ("Drag an SCNView from the library", 0);

			// And an image
			var imageNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/renderers/Interface Builder", "png"), 8.3f, false);
			imageNode.Position = new SCNVector3 (0.0f, 3.2f, 11.0f);
			ContentNode.AddChildNode (imageNode);
		}
	}
}

