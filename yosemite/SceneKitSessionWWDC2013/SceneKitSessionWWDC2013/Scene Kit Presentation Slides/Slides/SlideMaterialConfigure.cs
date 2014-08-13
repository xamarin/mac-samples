using System;
using SceneKit;

namespace SceneKitSessionWWDC2013
{
	public class SlideMaterialConfigure : Slide
	{
		public override int NumberOfSteps ()
		{
			return 4;
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.5f;

			switch (index) {
			case 0:
				TextManager.SetTitle ("Materials");
				TextManager.SetSubtitle ("Code example");

				TextManager.AddCode ("#// Access the geometry attribute of a node \n"
				+ "var geometry = node.#Geometry#; \n\n"
				+ "// Create a new \"red\" material \n"
				+ "var aMaterial = #SCNMaterial.Create ()#; \n"
				+ "aMaterial.#Diffuse#.Contents = NSColor.Red; \n\n"
				+ "// Set this material to our geometry \n"
				+ "geometry.#FirstMaterial# = aMaterial;#");

				TextManager.HighlightCodeChunks (null);
				break;
			case 1:
				TextManager.HighlightCodeChunks (new int[] { 0 });
				break;
			case 2:
				TextManager.HighlightCodeChunks (new int[] { 1, 2 });
				break;
			case 3:
				TextManager.HighlightCodeChunks (new int[] { 3 });
				break;
			}

			SCNTransaction.Commit ();
		}
	}
}

