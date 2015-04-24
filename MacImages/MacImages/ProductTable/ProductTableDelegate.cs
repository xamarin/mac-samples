using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MacImages
{
	public class ProductTableDelegate: NSTableViewDelegate
	{
		#region Constants 
		private const string CellIdentifier = "ProdCell";
		#endregion

		#region Private Variables
		private ProductTableDataSource DataSource;
		#endregion

		#region Constructors
		public ProductTableDelegate (ProductTableDataSource datasource)
		{
			this.DataSource = datasource;
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
				if (tableColumn.Title == "Product") {
					view.ImageView = new NSImageView (new CGRect (0, 0, 16, 16));
					view.AddSubview (view.ImageView);
					view.TextField = new NSTextField (new CGRect (20, 0, 400, 16));
				} else {
					view.TextField = new NSTextField (new CGRect (0, 0, 400, 16));
				}
				view.TextField.AutoresizingMask = NSViewResizingMask.WidthSizable;
				view.AddSubview (view.TextField);
				view.Identifier = tableColumn.Title;
				view.TextField.BackgroundColor = NSColor.Clear;
				view.TextField.Bordered = false;
				view.TextField.Selectable = false;
				view.TextField.Editable = true;

				view.TextField.EditingEnded += (sender, e) => {
					
					// Take action based on type
					switch(view.Identifier) {
					case "Product":
						DataSource.Products [(int)view.TextField.Tag].Title = view.TextField.StringValue;
						break;
					case "Details":
						DataSource.Products [(int)view.TextField.Tag].Description = view.TextField.StringValue;
						break; 
					}
				};
			}

			// Tag view
			view.TextField.Tag = row;

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Product":
				view.ImageView.Image = NSImage.ImageNamed ("tags.png");
				view.TextField.StringValue = DataSource.Products [(int)row].Title;
				break;
			case "Details":
				view.TextField.StringValue = DataSource.Products [(int)row].Description;
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

