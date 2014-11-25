using System;
using System.Collections.Generic;

using Foundation;
using AppKit;
using System.Linq;
using MobileCoreServices;
using CoreGraphics;

namespace FileCards
{
	public partial class AppDelegate : NSApplicationDelegate, INSPageControllerDelegate
	{
		const string kNibName = "FileCard";
		const string kImageNibname = "ImageCard";

		NSFileManager DefaultManager {
			get {
				return NSFileManager.DefaultManager;
			}
		}

		NSUrl documentDirectory;

		NSUrl DocumentDirectory {
			get {
				NSError error;
				documentDirectory = documentDirectory ?? DefaultManager.GetUrl (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User, null, false, out error);
				return documentDirectory;
			}
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			// load all the file card URLs by enumerating through the user's Document folder
			IEnumerable<NSUrl> fileUrls = GetFileUrls (DocumentDirectory);
			FileObject[] data = fileUrls.Where (url => !url.IsDir ()).Select (url => new FileObject (url)).ToArray ();

			// set the first card in our list
			if (data.Length > 0) {
				PageController.ArrangedObjects = data;
				TableView.SelectRows (NSIndexSet.FromIndex (0), false);
			}
		}

		IEnumerable<NSUrl> GetFileUrls (NSUrl dirUrl)
		{
			var keys = new NSString[] {
				NSUrl.LocalizedLabelKey,
				NSUrl.EffectiveIconKey,
				NSUrl.IsDirectoryKey,
				NSUrl.TypeIdentifierKey
			};

			var options = NSDirectoryEnumerationOptions.SkipsHiddenFiles
			              | NSDirectoryEnumerationOptions.SkipsPackageDescendants
			              | NSDirectoryEnumerationOptions.SkipsSubdirectoryDescendants;

			NSDirectoryEnumerator itr = DefaultManager.GetEnumerator (dirUrl, keys, options, null);
			DirectoryEnumerable enumerableDir = new DirectoryEnumerable (itr);

			return enumerableDir;
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

		[Export ("takeTransitionStyleFrom:")]
		void TakeTransitionStyleFrom (NSButton sender)
		{
			PageController.TransitionStyle = (NSPageControllerTransitionStyle)(int)sender.SelectedTag;
		}

		[Export ("tableViewSelectionDidChange:")]
		void tableViewSelectionDidChange (NSNotification aNotification)
		{
			nint selectedIndex = TableView.SelectedRow;
			if (selectedIndex < 0 || selectedIndex == PageController.SelectedIndex)
				return;

			// TODO: change pageControllerDidEndLiveTransition
			// The selection of the table view changed. We want to animate to the new selection.
			// However, since we are manually performing the animation,
			// -pageControllerDidEndLiveTransition: will not be called. We are required to
			// PageController.CompleteTransition() when the animation completes.
			NSAnimationContext.RunAnimation ((NSAnimationContext context) => {
				((NSPageController)PageController.Animator).SelectedIndex = selectedIndex;
			}, PageController.CompleteTransition);
		}

		// Required method for BookUI mode of NSPageController
		// We have different cards for image files and everything else.
		// Therefore, we have different identifiers
		// TODO: https://trello.com/c/kUzddDwf
		[Export ("pageController:identifierForObject:")]
		string GetIdentifier (NSPageController pv, NSObject obj)
		{
			var fileObj = (FileObject)obj;

			return UTType.ConformsTo (fileObj.UtiType, UTType.Image) ? kImageNibname : kNibName;
		}

		// Required method for BookUI mode of NSPageController
		// TODO: https://trello.com/c/kUzddDwf
		[Export ("pageController:viewControllerForIdentifier:")]
		NSViewController GetViewControllerForIdentifier (NSPageController pageController, string identifier)
		{
			return new NSViewController (identifier, null);
		}

		// Optional delegate method. This method is used to inset the card a little bit from it's parent view
		// TODO: https://trello.com/c/kUzddDwf
		[Export ("pageController:frameForObject:")]
		CGRect GetFrameForObject (NSPageController pageController, NSObject obj)
		{
			return pageController.View.Bounds.Inset (5, 5);
		}

		[Export ("pageControllerDidEndLiveTransition:")]
		void DidEndLiveTransition (NSPageController pageController)
		{
			// Update the NSTableView selection
			TableView.SelectRows (NSIndexSet.FromIndex (PageController.SelectedIndex), false);

			// tell page controller to complete the transition and display the updated file card
			PageController.CompleteTransition ();
		}
	}
}