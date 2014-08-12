using System;
using SceneKit;

namespace SceneKitSessionWWDC2014
{
	public class SlideDemo1 : Slide
	{
		private SCNNode ChapterNode { get; set; }

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			ChapterNode = TextManager.SetChapterTitle ("Car Toy Demo");
		}

		public override void WillOrderOut (PresentationViewController presentationViewController)
		{
			SCNTransaction.Begin ();
			SCNTransaction.AnimationDuration = 0.75f;
			ChapterNode.Position = new SCNVector3 (ChapterNode.Position.X-30, ChapterNode.Position.Y, ChapterNode.Position.Z);
			SCNTransaction.Commit ();
		}
	}
}

