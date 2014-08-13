using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2013
{
	public class SlideDaeOnOSX : Slide
	{
		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Add some text
			TextManager.SetTitle ("Working with DAE Files");
			TextManager.SetSubtitle ("DAE Files on OS X");

			// DAE icon
			var daeIconNode = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/dae file icon", "png"), 5, false);
			daeIconNode.Position = new SCNVector3 (0, 2.3f, 0);
			GroundNode.AddChildNode (daeIconNode);

			// Preview icon and text
			var previewIconNode = Utils.SCPlaneNodeWithImage (Utils.SCImageFromApplication ("Preview"), 3, false);
			previewIconNode.Position = new SCNVector3 (-5, 1.3f, 11);
			GroundNode.AddChildNode (previewIconNode);

			var previewTextNode = Utils.SCLabelNode ("Preview", Utils.LabelSize.Small, false);
			previewTextNode.Position = new SCNVector3 (-5.5f, 0, 13);
			GroundNode.AddChildNode (previewTextNode);

			// Quicklook icon and text
			var qlIconNode = Utils.SCPlaneNodeWithImage (Utils.SCImageFromApplication ("Finder"), 3, false);
			qlIconNode.Position = new SCNVector3 (0, 1.3f, 11);
			GroundNode.AddChildNode (qlIconNode);

			var qlTextNode = Utils.SCLabelNode ("QuickLook", Utils.LabelSize.Small, false);
			qlTextNode.Position = new SCNVector3 (-1.11f, 0, 13);
			GroundNode.AddChildNode (qlTextNode);

			// Xcode icon and text
			var xcodeIconNode = Utils.SCPlaneNodeWithImage (Utils.SCImageFromApplication ("Xcode"), 3, false);
			xcodeIconNode.Position = new SCNVector3 (5, 1.3f, 11);
			GroundNode.AddChildNode (xcodeIconNode);

			var xcodeTextNode = Utils.SCLabelNode ("Xcode", Utils.LabelSize.Small, false);
			xcodeTextNode.Position = new SCNVector3 (3.8f, 0, 13);
			GroundNode.AddChildNode (xcodeTextNode);
		}
	}
}

