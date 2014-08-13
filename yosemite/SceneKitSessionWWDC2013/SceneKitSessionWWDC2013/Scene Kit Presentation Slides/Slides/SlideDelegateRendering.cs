using System;
using System.Drawing;
using AppKit;
using OpenGL;
using SceneKit;

namespace SceneKitSessionWWDC2013
{
	public class SlideDelegateRendering : Slide, ISCNSceneRendererDelegate
	{
		private float FadeFactor { get; set; }

		private float FadeFactorDelta { get; set; }

		public override int NumberOfSteps ()
		{
			return 3;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Extending Scene Kit with OpenGL");
			TextManager.SetSubtitle ("Scene delegate rendering");

			TextManager.AddBulletAtLevel ("Custom GL code, free of constraints", 0);
			TextManager.AddBulletAtLevel ("Before and/or after scene rendering", 0);
			TextManager.AddBulletAtLevel ("Works with SCNView, SCNLayer and SCNRenderer", 0);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			switch (index) {
			case 1:
				FadeFactor = 0; // tunnel is not visible
				FadeFactorDelta = 0.05f; // fade in

				// Set self as the scene renderer's delegate and make the view redraw for ever
				//((SCNView)presentationViewController.View).SceneRendererDelegate = this;
				((SCNView)presentationViewController.View).Playing = true;
				((SCNView)presentationViewController.View).Loops = true;
				break;
			case 2:
				FadeFactorDelta *= -1; // fade out
				break;
			}
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			((SCNView)presentationViewController.View).SceneRendererDelegate = null;
			((SCNView)presentationViewController.View).Playing = false;
		}
	}
}

