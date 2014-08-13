using System;
using System.Drawing;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace SceneKitSessionWWDC2013
{
	public partial class AppDelegate : NSApplicationDelegate, IPresentation
	{
		PresentationViewController PresentationViewController { get; set; }
		bool Hidden { get; set; }

		public override void WillFinishLaunching (NSNotification notification)
		{
			// Create a presentation from an xml file
			PresentationViewController = new PresentationViewController ("Scene Kit Presentation");
			PresentationViewController.PresentationDelegate = this;

			// Populate the 'Go' menu for direct access to slides
			PopulateGoMenu ();

			// Start the presentation
			MainWindow.ContentView.AddSubview (PresentationViewController.View);
			PresentationViewController.View.Frame = MainWindow.ContentView.Bounds;
			PresentationViewController.View.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
		}

		public void WillPresentSlide (int slideIndex, int step)
		{
			// Update the window's title depending on the current slide
			if (step == 0) {
				MainWindow.Title = "SceneKit WWDC 2013 - slide " + slideIndex;
			} else {
				MainWindow.Title = "SceneKit WWDC 2013 - slide " + slideIndex + " step " + step;
			}
		}

		public void PopulateGoMenu ()
		{
			for (int i = 0; i < PresentationViewController.NumberOfSlides; i++) {
				var slideName = PresentationViewController.ClassOfSlide (i).ToString();
				var splitedString = slideName.Split ('.');
				var title = i + " " + splitedString[1].Substring(5);
				var item = new NSMenuItem (title, GoTo);
				item.RepresentedObject = new NSNumber (i);
				GoMenu.AddItem (item);
			}
		}

		partial void PreviousSlide (NSObject sender)
		{
			PresentationViewController.GoToPreviousSlide ();
		}

		partial void NextSlide (NSObject sender)
		{
			PresentationViewController.GoToNextSlideStep ();
		}

		public void GoTo (object sender, EventArgs e)
		{
			var item = sender as NSMenuItem;
			var index = (NSNumber)(item.RepresentedObject);
			PresentationViewController.GoToSlide (index.Int32Value);
		}

		partial void ToogleCursor (NSObject sender)
		{
			Hidden = false;
			if (Hidden) {
				NSCursor.Unhide();
				Hidden = false;
			} else {
				NSCursor.Hide();
				Hidden = true;
			}
		}
	}
}