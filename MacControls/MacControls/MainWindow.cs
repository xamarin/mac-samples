using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace MacControls
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

			var TableViews = new SourceListItem ("Control Type");
			TableViews.AddItem ("Buttons", "tag.png", () => {
				DisplaySubview(new SubviewButtonsController(), SubviewType.Buttons);
			});
			TableViews.AddItem ("Checkboxes & Radio Buttons", "tag.png", () => {
				DisplaySubview(new SubviewChecksRadioController(), SubviewType.CheckAndRadioButtons);
			});
			TableViews.AddItem ("Menu Controls", "tag.png", () => {
				DisplaySubview(new SubviewMenuControlsController(), SubviewType.MenuControls);
			});
			TableViews.AddItem ("Selection Controls", "tag.png", () => {
				DisplaySubview(new SubviewSelectionControlsController(), SubviewType.SelectionControls);
			});
			TableViews.AddItem ("Indicator Controls", "tag.png", () => {
				DisplaySubview(new SubviewIndicatorControlsController(), SubviewType.IndicatorControls);
			});
			TableViews.AddItem ("Text Controls", "tag.png", () => {
				DisplaySubview(new SubviewTextControlsController(), SubviewType.IndicatorControls);
			});
			TableViews.AddItem ("Content Views", "tag.png", () => {
				DisplaySubview(new SubviewContentViewsController(), SubviewType.ContentViews);
			});
			SourceList.AddItem (TableViews);


			// Display Source List
			SourceList.ReloadData();
			SourceList.ExpandItem (null, true);
		}
		#endregion
	}
}
