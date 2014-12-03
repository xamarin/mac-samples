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
		PageControllerDelegate pageDelegate;

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
			pageDelegate = new PageControllerDelegate (this);
			PageController.Delegate = pageDelegate;

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
	}
}