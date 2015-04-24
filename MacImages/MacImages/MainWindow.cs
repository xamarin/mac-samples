using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace MacImages
{
	public partial class MainWindow : NSWindow
	{

		#region Private Variables
		private SubviewType ViewType = SubviewType.None;
		private NSViewController SubviewController = null;
		private NSView Subview = null;
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods
		private void DisplaySubview(NSViewController controller, SubviewType type) {

			// Is this view already displayed?
			if (ViewType == type) return;

			// Is there a view already being displayed?
			if (Subview != null) {
				// Yes, remove it from the view
				Subview.RemoveFromSuperview ();

				// Release memory
				Subview = null;
				SubviewController = null;
			}

			// Save values
			ViewType = type;
			SubviewController = controller;
			Subview = controller.View;

			// Define frame and display
			Subview.Frame = new CGRect (0, 0, ViewContainer.Frame.Width, ViewContainer.Frame.Height);
			ViewContainer.AddSubview (Subview);
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Populate Source List
			SourceList.Initialize ();

			var TableViews = new SourceListItem ("Table Based Views");
			TableViews.AddItem ("Table View", "calendar.png", () => {
				DisplaySubview(new SubviewTableController(), SubviewType.TableView);
			});
			TableViews.AddItem ("Outline View", "calendar.png", () => {
				DisplaySubview(new SubviewOutlineController(), SubviewType.OutlineView);
			});
			SourceList.AddItem (TableViews);

			var ImageViews = new SourceListItem ("Photos");
			ImageViews.AddItem ("First Person", "film-roll.png", () => {
				if (ViewType== SubviewType.ImageView) {
					var Photo = Subview as SubviewImage ;
					Photo.Image = NSImage.ImageNamed("person01.jpg");
				} else {
					var Photo = new SubviewImageController();
					Photo.View.Image = NSImage.ImageNamed("person01.jpg");
					DisplaySubview(Photo, SubviewType.ImageView);
				}
			});
			ImageViews.AddItem ("Second Person", "film-roll.png", () => {
				if (ViewType== SubviewType.ImageView) {
					var Photo = Subview as SubviewImage ;
					Photo.Image = NSImage.ImageNamed("person02.jpg");
				} else {
					var Photo = new SubviewImageController();
					Photo.View.Image = NSImage.ImageNamed("person02.jpg");
					DisplaySubview(Photo, SubviewType.ImageView);
				}
			});
			ImageViews.AddItem ("Third Person", "film-roll.png", () => {
				if (ViewType== SubviewType.ImageView) {
					var Photo = Subview as SubviewImage ;
					Photo.Image = NSImage.ImageNamed("person03.jpg");
				} else {
					var Photo = new SubviewImageController();
					Photo.View.Image = NSImage.ImageNamed("person03.jpg");
					DisplaySubview(Photo, SubviewType.ImageView);
				}
			});
			SourceList.AddItem (ImageViews);

			// Display Source List
			SourceList.ReloadData();
			SourceList.ExpandItem (null, true);
		}
		#endregion
	}
}
