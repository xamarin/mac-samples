using System;
using SceneKit;
using Foundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideTechniques : Slide
	{
		private enum TechniqueSteps
		{
			Intro,
			Pass1,
			Passes3,
			Passes3Connected,
			Files,
			Plist,
			Code,
			Sample,
			Count
		}

		private SCNNode PlistGroup { get; set; }

		private SCNNode Pass3 { get; set; }

		private SCNNode Pass2 { get; set; }

		private SCNNode Pass1 { get; set; }

		public override int NumberOfSteps ()
		{
			return (int)TechniqueSteps.Count;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Multi-Pass Effects");
			TextManager.SetSubtitle ("SCNTechnique");

			TextManager.AddBulletAtLevel ("Multi-pass effects", 0);
			TextManager.AddBulletAtLevel ("Post processing", 0);
			TextManager.AddBulletAtLevel ("Chain passes", 0);
			TextManager.AddBulletAtLevel ("Set and animate shader uniforms from C#", 0);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case (int)TechniqueSteps.Intro:
				break;
			case (int)TechniqueSteps.Code:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				PlistGroup.RemoveFromParentNode ();

				TextManager.AddEmptyLine ();
				TextManager.AddCode ("#// Load a technique\nSCNTechnique *technique = [SCNTechnique #techniqueWithDictionary#:aDictionary];\n\n\n"
				+ "// Chain techniques\ntechnique = [SCNTechnique #techniqueBySequencingTechniques#:@[t1, t2 ...];\n\n\n\t\t\t\t\t"
				+ "// Set a technique\naSCNView.#technique# = technique;#");

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);
				break;
			case (int)TechniqueSteps.Files:
				Pass2.RemoveFromParentNode ();

				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddBulletAtLevel ("Load from Plist", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Code);

				PlistGroup = SCNNode.Create ();
				ContentNode.AddChildNode (PlistGroup);

				//add plist icon
				var node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/plist", "png"), 8, true);
				node.Position = new SCNVector3 (0, 3.7f, 10);
				PlistGroup.AddChildNode (node);

				//add plist icon
				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/vsh", "png"), 3, true);
				for (var i = 0; i < 5; i++) {
					node = node.Clone ();
					node.Position = new SCNVector3 (6, 1.4f, 10 - i);
					PlistGroup.AddChildNode (node);
				}

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/fsh", "png"), 3, true);
				for (var i = 0; i < 5; i++) {
					node = node.Clone ();
					node.Position = new SCNVector3 (9, 1.4f, 10 - i);
					PlistGroup.AddChildNode (node);
				}
				break;
			case (int)TechniqueSteps.Plist:
				//add plist icon
				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/technique", "png"), 9, true);
				node.Position = new SCNVector3 (0, 3.5f, 10.1f);
				node.Opacity = 0.0f;
				PlistGroup.AddChildNode (node);
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				node.Position = new SCNVector3 (0, 3.5f, 11);
				node.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case (int)TechniqueSteps.Pass1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/pass1", "png"), 15, true);
				node.Position = new SCNVector3 (0, 3.5f, 10.1f);
				node.Opacity = 0.0f;
				ContentNode.AddChildNode (node);
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				node.Position = new SCNVector3 (0, 3.5f, 11);
				node.Opacity = 1.0f;
				SCNTransaction.Commit ();
				Pass1 = node;
				break;
			case (int)TechniqueSteps.Passes3:
				Pass1.RemoveFromParentNode ();
				Pass2 = SCNNode.Create ();
				Pass2.Opacity = 0.0f;
				Pass2.Position = new SCNVector3 (0, 3.5f, 6);

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/pass2", "png"), 8, true);
				node.Position = new SCNVector3 (-8, 0, 0);
				Pass2.AddChildNode (node);

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/pass3", "png"), 8, true);
				node.Position = new SCNVector3 (0, 0, 0);
				Pass2.AddChildNode (node);

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/pass4", "png"), 8, true);
				node.Position = new SCNVector3 (8, 0, 0);
				Pass2.AddChildNode (node);

				ContentNode.AddChildNode (Pass2);
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				Pass2.Position = new SCNVector3 (0, 3.5f, 9);
				Pass2.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			case (int)TechniqueSteps.Passes3Connected:
				TextManager.AddEmptyLine ();
				TextManager.AddBulletAtLevel ("Connect pass inputs / outputs", 0);

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/link", "png"), 8.75f, true);
				node.Position = new SCNVector3 (0.01f, -2, 0);
				node.Opacity = 0.0f;
				Pass2.AddChildNode (node);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				var n = Pass2.ChildNodes [0];
				n.Position = new SCNVector3 (-7.5f, -0.015f, 0);

				n = Pass2.ChildNodes [2];
				n.Position = new SCNVector3 (7.5f, 0.02f, 0);

				node.Opacity = 1.0f;

				SCNTransaction.Commit ();
				break;
			case (int)TechniqueSteps.Sample:
				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
				TextManager.SetSubtitle ("Example: simple depth of field");
				TextManager.FlipInText (SlideTextManager.TextType.Code);

				Pass3 = SCNNode.Create ();

				node = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/pass5", "png"), 15, true);
				node.Position = new SCNVector3 (-3, 5, 10.1f);
				node.Opacity = 0.0f;
				Pass3.AddChildNode (node);

				var t0 = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/technique0", "png"), 4, false);
				t0.Position = new SCNVector3 (-8.5f, 1.5f, 10.1f);
				t0.Opacity = 0.0f;
				Pass3.AddChildNode (t0);

				var t1 = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/technique1", "png"), 4, false);
				t1.Position = new SCNVector3 (-3.6f, 1.5f, 10.1f);
				t1.Opacity = 0.0f;
				Pass3.AddChildNode (t1);

				var t2 = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/technique2", "png"), 4, false);
				t2.Position = new SCNVector3 (1.4f, 1.5f, 10.1f);
				t2.Opacity = 0.0f;
				Pass3.AddChildNode (t2);

				var t3 = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/technique/technique3", "png"), 8, false);
				t3.Position = new SCNVector3 (8, 5, 10.1f);
				t3.Opacity = 0.0f;
				Pass3.AddChildNode (t3);

				ContentNode.AddChildNode (Pass3);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				node.Opacity = 1.0f;
				t0.Opacity = 1.0f;
				t1.Opacity = 1.0f;
				t2.Opacity = 1.0f;
				t3.Opacity = 1.0f;
				SCNTransaction.Commit ();
				break;
			}
		}
	}
}

