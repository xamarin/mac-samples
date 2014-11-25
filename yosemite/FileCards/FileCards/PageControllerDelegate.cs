using System;
using AppKit;
using Foundation;
using MobileCoreServices;
using CoreGraphics;

namespace FileCards
{
	public class PageControllerDelegate : NSPageControllerDelegate
	{
		const string kNibName = "FileCard";
		const string kImageNibname = "ImageCard";

		readonly AppDelegate appDelegate;

		public PageControllerDelegate(AppDelegate appDelegate)
		{
			this.appDelegate = appDelegate;
		}

		// Required method for BookUI mode of NSPageController
		// We have different cards for image files and everything else.
		// Therefore, we have different identifiers
		public override string GetIdentifier (NSPageController pv, NSObject obj)
		{
			var fileObj = (FileObject)obj;

			return UTType.ConformsTo (fileObj.UtiType, UTType.Image) ? kImageNibname : kNibName;
		}

		// Required method for BookUI mode of NSPageController
		public override NSViewController GetViewController (NSPageController pageController, string identifier)
		{
			return new NSViewController (identifier, null);
		}

		// Optional delegate method. This method is used to inset the card a little bit from it's parent view
		public override CGRect GetFrame (NSPageController pageController, NSObject targetObject)
		{
			return pageController.View.Bounds.Inset (5, 5);
		}

		public override void DidEndLiveTransition (NSPageController pageController)
		{
			// Update the NSTableView selection
			appDelegate.TableView.SelectRows (NSIndexSet.FromIndex (appDelegate.PageController.SelectedIndex), false);

			// tell page controller to complete the transition and display the updated file card
			appDelegate.PageController.CompleteTransition ();
		}
	}
}

