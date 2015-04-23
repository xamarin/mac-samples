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
			NSTextField view = (NSTextField)tableView.MakeView (tableColumn.Title, this);
			if (view == null) {
				view = new NSTextField ();
				view.Identifier = tableColumn.Title;
				view.Bordered = false;
				view.Selectable = false;
				view.Editable = true;

				view.EditingEnded += (sender, e) => {
					
					// Take action based on type
					switch(view.Identifier) {
					case "Product":
						DataSource.Products [(int)view.Tag].Title = view.StringValue;
						break;
					case "Details":
						DataSource.Products [(int)view.Tag].Description = view.StringValue;
						break; 
					}
				};
			}

			// Tag view
			view.Tag = row;

			// Setup view based on the column selected
			switch (tableColumn.Title) {
			case "Product":
				view.StringValue = DataSource.Products [(int)row].Title;
				break;
			case "Details":
				view.StringValue = DataSource.Products [(int)row].Description;
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

