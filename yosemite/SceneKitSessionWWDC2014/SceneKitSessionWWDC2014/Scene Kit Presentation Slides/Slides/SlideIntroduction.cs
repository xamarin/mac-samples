using System;
using System.Collections.Generic;

using AppKit;
using SceneKit;
using Foundation;
using CoreGraphics;
using CoreFoundation;

namespace SceneKitSessionWWDC2014
{
	public class SlideIntroduction : Slide
	{
		private List<SCNNode> Boxes { get; set; }

		private SCNNode Icon1 { get; set; }

		private SCNNode Icon2 { get; set; }

		public override int NumberOfSteps ()
		{
			return 3;
		}

		public override void SetupSlide (PresentationViewController presentationViewController)
		{
			TextManager.SetTitle ("Scene Kit");
			TextManager.SetSubtitle ("Introduction");
			TextManager.AddBulletAtLevel ("High level API for 3D integration", 0);
			TextManager.AddBulletAtLevel ("Data visualization", 1);
			TextManager.AddBulletAtLevel ("User Interface", 1);
			TextManager.AddBulletAtLevel ("Casual Games", 1);

			var redColor = NSColor.FromDeviceRgba (168.0f / 255.0f, 21.0f / 255.0f, 1.0f / 255.0f, 1);
			var grayColor = NSColor.Gray;
			var greenColor = NSColor.FromDeviceRgba (105.0f / 255.0f, 145.0f / 255.0f, 14.0f / 255.0f, 1);
			var orangeColor = NSColor.Orange;
			var purpleColor = NSColor.FromDeviceRgba (152.0f / 255.0f, 57.0f / 255.0f, 189.0f / 255.0f, 1);

			Boxes = new List<SCNNode> ();

			AddBox ("Cocoa", new CGRect (0, 0, 500, 70), 3, grayColor);
			AddBox ("Core Image", new CGRect (0, 0, 100, 70), 2, greenColor);
			AddBox ("Core Animation", new CGRect (390, 0, 110, 70), 2, greenColor);
			AddBox ("Sprite Kit", new CGRect (250, 0, 135, 70), 2, greenColor);
			AddBox ("Scene Kit", new CGRect (105, 0, 140, 70), 2, orangeColor);
			AddBox ("OpenGL / OpenGL ES", new CGRect (0, 0, 500, 70), 1, purpleColor);
			AddBox ("Graphics Hardware", new CGRect (0, 0, 500, 70), 0, redColor);
		}

		public override void PresentStep (int index, PresentationViewController presentationViewController)
		{
			float delay = 0;

			switch (index) {
			case 0:
				break;
			case 1:
				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.AddEmptyLine ();
				TextManager.AddBulletAtLevel ("Available on OS X 10.8+ and iOS 8.0", 0);
				TextManager.FlipInText (SlideTextManager.TextType.Bullet);

					//show some nice icons
				Icon1 = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/Badge_X", "png"), 7, false);
				Icon1.Position = new SCNVector3 (-20, 3.5f, 5);
				GroundNode.AddChildNode (Icon1);

				Icon2 = Utils.SCPlaneNode (NSBundle.MainBundle.PathForResource ("Images/Badge_iOS", "png"), 7, false);
				Icon2.Position = new SCNVector3 (20, 3.5f, 5);
				GroundNode.AddChildNode (Icon2);

				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				Icon1.Position = new SCNVector3 (-6, 3.5f, 5);
				Icon2.Position = new SCNVector3 (6, 3.5f, 5);
				SCNTransaction.Commit ();
				break;
			case 2:
				SCNTransaction.Begin ();
				SCNTransaction.AnimationDuration = 0.75f;
				Icon1.Position = new SCNVector3 (-6, 3.5f, -5);
				Icon2.Position = new SCNVector3 (6, 3.5f, -5);
				Icon1.Opacity = 0.0f;
				Icon2.Opacity = 0.0f;
				SCNTransaction.Commit ();

				foreach (var node in Boxes) {
					var popTime = new DispatchTime (DispatchTime.Now, (Int64)(delay * Utils.NSEC_PER_SEC));
					DispatchQueue.MainQueue.DispatchAfter (popTime, () => {
						SCNTransaction.Begin ();
						SCNTransaction.AnimationDuration = 0.5f;

						node.Rotation = new SCNVector4 (1, 0, 0, 0);
						node.Scale = new SCNVector3 (0.02f, 0.02f, 0.02f);
						node.Opacity = 1.0f;

						SCNTransaction.Commit ();
					});

					delay += 0.05f;
				}


				TextManager.FlipOutText (SlideTextManager.TextType.Bullet);
				TextManager.FlipOutText (SlideTextManager.TextType.Subtitle);
			
				TextManager.SetSubtitle ("Graphic Frameworks");

				TextManager.FlipInText (SlideTextManager.TextType.Bullet);
				TextManager.FlipInText (SlideTextManager.TextType.Subtitle);

				break;
			}
		}

		private void AddBox (string title, CGRect frame, int level, NSColor color)
		{
			var node = Utils.SCBoxNode (title, frame, color, 2.0f, true);
			node.Pivot = SCNMatrix4.CreateTranslation (0, frame.Size.Height / 2, 0);
			node.Scale = new SCNVector3 (0.02f, 0.02f, 0.02f);
			node.Position = new SCNVector3 (-5, (0.02f * frame.Size.Height / 2) + (1.5f * level), 10.0f);
			node.Rotation = new SCNVector4 (1, 0, 0, (float)(Math.PI / 2));
			node.Opacity = 0.0f;
			ContentNode.AddChildNode (node);

			Boxes.Add (node);
		}
	}
}

