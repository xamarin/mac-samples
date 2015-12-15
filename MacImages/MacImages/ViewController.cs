using System;

using AppKit;
using Foundation;

namespace MacDatabinding
{
	public partial class ViewController : NSViewController
	{
		#region Private Variables
		private NSImage Picture = null;
		#endregion

		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		public NSView Content {
			get { return ContentView; }
		}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Populate Source List
			SourceList.Initialize ();

			var TableViews = new SourceListItem ("Table Based Views");
			TableViews.AddItem ("Table View", "calendar.png", () => {
				PerformSegue("TableSegue", this);
			});
			TableViews.AddItem ("Outline View", "calendar.png", () => {
				PerformSegue("OutlineSegue", this);
			});
			SourceList.AddItem (TableViews);

			var ImageViews = new SourceListItem ("Photos");
			ImageViews.AddItem ("First Person", "film-roll.png", () => {
				Picture = NSImage.ImageNamed("person01.jpg");
				PerformSegue("PictureSegue", this);
			});
			ImageViews.AddItem ("Second Person", "film-roll.png", () => {
				Picture = NSImage.ImageNamed("person02.jpg");
				PerformSegue("PictureSegue", this);
			});
			ImageViews.AddItem ("Third Person", "film-roll.png", () => {
				Picture = NSImage.ImageNamed("person03.jpg");
				PerformSegue("PictureSegue", this);
			});
			SourceList.AddItem (ImageViews);

			// Display Source List
			SourceList.ReloadData();
			SourceList.ExpandItem (null, true);
		}

		public override void PrepareForSegue (NSStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Take action based on segue type
			switch (segue.Identifier) {
			case "PictureSegue":
				var pictureView = segue.DestinationController as PictureViewController;
				pictureView.Picture = Picture;
				break;
			}
		}
		#endregion
	}
}
