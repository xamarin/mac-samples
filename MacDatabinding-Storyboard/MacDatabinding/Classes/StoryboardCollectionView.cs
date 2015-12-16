using System;
using AppKit;
using Foundation;

namespace MacDatabinding
{
	[Register("StoryboardCollectionView")]
	public class StoryboardCollectionView : NSCollectionView
	{
		#region Computed Properties
		public override NSCollectionViewItem ItemPrototype {
			get {
				var storyboard = NSStoryboard.FromName ("Main", null);
				var prototype = storyboard.InstantiateControllerWithIdentifier ("CollectionItem") as CollectionItemController;
				return prototype;
			}
			set {
				// Ignore for now
			}
		}
		#endregion

		#region Constructors
		public StoryboardCollectionView (IntPtr handle) : base (handle)
		{
			// TODO: Update this once Xcode 7 is fixed to handle Collection Views inside of 
			// storyboards.
			Console.WriteLine ("WARNING! Due to a bug in Xcode 7, Collection Views ARE NOT supported in Storyboards. \n" +
			"Please continue to use .xib files if Collection Views are required.");
		}
		#endregion
	}
}