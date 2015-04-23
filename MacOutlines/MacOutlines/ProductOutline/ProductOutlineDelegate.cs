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
			NSTextField view = (NSTextField)outlineView.MakeView (tableColumn.Title, this);
			if (view == null) {
				view = new NSTextField ();
				view.Identifier = tableColumn.Title;
				view.Bordered = false;
				view.Selectable = false;
				view.Editable = !product.IsProductGroup;
			}

			// Tag view
			view.Tag = outlineView.RowForItem (item);

			// Allow for edit
			view.EditingEnded += (sender, e) => {

				// Grab product
				var prod = outlineView.ItemAtRow(view.Tag) as Product;

				// Take action based on type
				switch(view.Identifier) {
				case "Product":
					prod.Title = view.StringValue;
					break;
				case "Details":
					prod.Description = view.StringValue;
					break; 
				}
			};

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Product":
				view.StringValue = product.Title;
				break;
			case "Details":
				view.StringValue = product.Description;
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

