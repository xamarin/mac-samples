using System;

using Foundation;
using AppKit;
using CoreGraphics;

namespace MacDatabinding
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

			// Take action on type
			switch (type) {
			case SubviewType.TableBinding:
				AddButton.Active = true;
				EditButton.Active = true;
				DeleteButton.Active = true;
				Search.Enabled = true;
				break;
			case SubviewType.CollectionView:
				AddButton.Active = true;
				EditButton.Active = true;
				DeleteButton.Active = true;
				Search.Enabled = true;
				break;
			default:
				AddButton.Active = false;
				EditButton.Active = false;
				DeleteButton.Active = false;
				Search.Enabled = false;
				break;
			}
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Wire-up controls
			AddButton.Active = false;
			AddButton.Activated += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					var sheet = new PersonEditorSheetController(new PersonModel("Unknown","Unknown"), true);

					// Wire-up
					sheet.PersonModified += (person) => {
						controller.AddPerson(person);
					};

					// Display sheet
					sheet.ShowSheet(this);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					var collectionSheet = new PersonEditorSheetController(new PersonModel("Unknown","Unknown"), true);

					// Wire-up
					collectionSheet.PersonModified += (person) => {
						collection.AddPerson(person);
					};

					// Display sheet
					collectionSheet.ShowSheet(this);
					break;
				}
			};

			EditButton.Active = false;
			EditButton.Activated += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					controller.EditPerson(this);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					collection.EditPerson(this);
					break;
				}
			};

			DeleteButton.Active = false;
			DeleteButton.Activated += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					controller.DeletePerson(this);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					collection.DeletePerson(this);
					break;
				}
			};

			Search.Enabled = false;
			Search.EditingEnded += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					controller.FindPerson(Search.StringValue);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					collection.FindPerson(Search.StringValue);
					break;
				}
			};

			// Populate Source List
			SourceList.Initialize ();

			var TableViews = new SourceListItem ("Data Binding Type");
			TableViews.AddItem ("Simple Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewSimpleBindingController(), SubviewType.SimpleBinding);
			});
			TableViews.AddItem ("Table Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewTableBindingController(), SubviewType.TableBinding);
			});
			TableViews.AddItem ("Outline Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewOutlineBindingController(), SubviewType.OutlineBinging);
			});
			TableViews.AddItem ("Collection View", "shoebox.png", () => {
				DisplaySubview(new SubviewCollectionViewController(), SubviewType.CollectionView);
			});
			SourceList.AddItem (TableViews);

			// Display Source List
			SourceList.ReloadData();
			SourceList.ExpandItem (null, true);
		}
		#endregion
	}
}
