using System;

namespace SceneKitSessionWWDC2014
{
	public class SlideJS : Slide
	{
		public override int NumberOfSteps ()
		{
			return 3;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			// Set the slide's title and subtitle and add some code
			TextManager.SetTitle ("Scriptability");

			TextManager.AddBulletAtLevel ("Javascript bridge", 0);
			TextManager.AddCode ("#// setup a JSContext for SceneKit\n"
			+ "#SCNJavaScript.ExportModule# (aJSContext);\n\n"
			+ "// reference a SceneKit object from JS\n"
			+ "aJSContext.#GlobalObject# = aNode;\n\n"
			+ "// execute a script\n"
			+ "aJSContext.#EvaluateScript# (\"aNode.scale = {x:2, y:2, z:2};\");#");
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 0:
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddEmptyLine ();
				TextManager.AddBulletAtLevel ("Javascript code example", 0);
				TextManager.AddCode ("#\n#//allocate a node#\n"
				+ "var aNode = SCNNode.Create ();\n\n"
				+ "#//change opacity#\n"
				+ "aNode.Opacity = 0.5f;\n\n"
				+ "#//remove from parent#\n"
				+ "aNode.RemoveFromParentNode ();\n\n"
				+ "#//animate implicitly#\n"
				+ "SCNTransaction.Begin ();\n"
				+ "SCNTransaction.AnimationDuration = 1.0f;\n"
				+ "aNode.Scale = new SCNVector3 (2, 2, 2);\n"
				+ "SCNTransaction.Commit ();#");

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Code);

				break;
			case 2:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Code);
				TextManager.AddBulletAtLevel ("Tools", 0);
				TextManager.AddBulletAtLevel ("Debugging", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				break;
			}
		}
	}
}

