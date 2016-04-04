using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MacTables
{
	public class ProductTableDelegate: NSTableViewDelegate
	{
		#region Constants 
		private const string CellIdentifier = "ProdCell";
		#endregion

		#region Private Variables
		private ProductTableDataSource DataSource;
		private ViewController Controller;
		#endregion

		#region Constructors
		public ProductTableDelegate (ViewController controller, ProductTableDataSource datasource)
		{
			this.Controller = controller;
			this.DataSource = datasource;
		}
		#endregion

		#region Private Methods
		private void ConfigureTextField (NSTableCellView view, nint row)
		{
			// Add to view
			view.TextField.AutoresizingMask = NSViewResizingMask.WidthSizable;
			view.AddSubview (view.TextField);

			// Configure
			view.TextField.BackgroundColor = NSColor.Clear;
			view.TextField.Bordered = false;
			view.TextField.Selectable = false;
			view.TextField.Editable = true;

			// Wireup events
			view.TextField.EditingEnded += (sender, e) => {

				// Take action based on type
				switch (view.Identifier) {
				case "Product":
					DataSource.Products [(int)view.TextField.Tag].Title = view.TextField.StringValue;
					break;
				case "Details":
					DataSource.Products [(int)view.TextField.Tag].Description = view.TextField.StringValue;
					break;
				}
			};

			// Tag view
			view.TextField.Tag = row;
		}
		#endregion

		#region Override Methods
		public override NSView GetViewForItem (NSTableView tableView, NSTableColumn tableColumn, nint row)
		{

			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTableCellView view = (NSTableCellView)tableView.MakeView (tableColumn.Title, this);
			if (view == null) {
				view = new NSTableCellView ();

				// Configure the view
				view.Identifier = tableColumn.Title;

				// Take action based on title
				switch (tableColumn.Title) {
				case "Product":
					view.ImageView = new NSImageView (new CGRect (0, 0, 16, 16));
					view.AddSubview (view.ImageView);
					view.TextField = new NSTextField (new CGRect (20, 0, 400, 16));
					ConfigureTextField (view, row);
					break;
				case "Details":
					view.TextField = new NSTextField (new CGRect (0, 0, 400, 16));
					ConfigureTextField (view, row);
					break;
				case "Action":
					// Create new button
					var button = new NSButton (new CGRect (0, 0, 81, 16));
					button.SetButtonType (NSButtonType.MomentaryPushIn);
					button.Title = "Delete";
					button.Tag = row;

					// Wireup events
					button.Activated += (sender, e) => {
						// Get button and product
						var btn = sender as NSButton;
						var product = DataSource.Products [(int)btn.Tag];

						// Configure alert
						var alert = new NSAlert () {
							AlertStyle = NSAlertStyle.Informational,
							InformativeText = $"Are you sure you want to delete {product.Title}? This operation cannot be undone.",
							MessageText = $"Delete {product.Title}?",
						};
						alert.AddButton ("Cancel");
						alert.AddButton ("Delete");
						alert.BeginSheetForResponse (Controller.View.Window, (result) => {
							// Should we delete the requested row?
							if (result == 1001) {
								// Remove the given row from the dataset
								DataSource.Products.RemoveAt((int)btn.Tag);
								Controller.ReloadTable ();
							}
						});
					};

					// Add to view
					view.AddSubview (button);
					break;
				}

			}

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Product":
				view.ImageView.Image = NSImage.ImageNamed ("tag.png");
				view.TextField.StringValue = DataSource.Products [(int)row].Title;
				view.TextField.Tag = row;
				break;
			case "Details":
				view.TextField.StringValue = DataSource.Products [(int)row].Description;
				view.TextField.Tag = row;
				break;
			case "Action":
				foreach (NSView subview in view.Subviews) {
					var btn = subview as NSButton;
					if (btn != null) {
						btn.Tag = row;
					}
				}
				break;
			}

			return view;
		}

		public override bool ShouldSelectRow (NSTableView tableView, nint row)
		{
			return true;
		}

		public override nint GetNextTypeSelectMatch (NSTableView tableView, nint startRow, nint endRow, string searchString)
		{
			nint row = 0;
			foreach(Product product in DataSource.Products) {
				if (product.Title.Contains(searchString)) return row;

				// Increment row counter
				++row;
			}

			// If not found select the first row
			return 0;
		}

		public override bool ShouldReorder (NSTableView tableView, nint columnIndex, nint newColumnIndex)
		{
			return true;
		}
		#endregion


	}
}

