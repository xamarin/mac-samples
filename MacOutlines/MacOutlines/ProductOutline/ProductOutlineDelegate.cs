using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MacOutlines
{
	public class ProductOutlineDelegate : NSOutlineViewDelegate
	{
		#region Constants 
		private const string CellIdentifier = "ProdCell";
		#endregion

		#region Private Variables
		private ProductOutlineDataSource DataSource;
		#endregion

		#region Constructors
		public ProductOutlineDelegate (ProductOutlineDataSource datasource)
		{
			this.DataSource = datasource;
		}
		#endregion

		#region Override Methods
		public override NSView GetView (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item) {
			// Cast item
			var product = item as Product;

			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTableCellView view = (NSTableCellView)outlineView.MakeView (tableColumn.Title, this);
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
				view.TextField.Editable = !product.IsProductGroup;
			}

			// Tag view
			view.TextField.Tag = outlineView.RowForItem (item);

			// Allow for edit
			view.TextField.EditingEnded += (sender, e) => {

				// Grab product
				var prod = outlineView.ItemAtRow(view.TextField.Tag) as Product;

				// Take action based on type
				switch(view.Identifier) {
				case "Product":
					prod.Title = view.TextField.StringValue;
					break;
				case "Details":
					prod.Description = view.TextField.StringValue;
					break; 
				}
			};

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Product":
				view.ImageView.Image = NSImage.ImageNamed (product.IsProductGroup ? "tags.png" : "tag.png");
				view.TextField.StringValue = product.Title;
				break;
			case "Details":
				view.TextField.StringValue = product.Description;
				break;
			}

			return view;
		}

		public override bool ShouldSelectItem (NSOutlineView outlineView, NSObject item)
		{
			// Don't select product groups
			return !((Product)item).IsProductGroup;
		}

		public override NSObject GetNextTypeSelectMatch (NSOutlineView outlineView, NSObject startItem, NSObject endItem, string searchString)
		{
			foreach(Product product in DataSource.Products) {
				if (product.Title.Contains (searchString)) {
					return product;
				}
			}

			// Not found
			return null;
		}

		public override bool ShouldReorder (NSOutlineView outlineView, nint columnIndex, nint newColumnIndex)
		{
			return true;
		}
		#endregion
	}
}

