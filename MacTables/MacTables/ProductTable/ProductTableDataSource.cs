using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MacTables
{
	public class ProductTableDataSource : NSTableViewDataSource
	{
		#region Public Variables
		public List<Product> Products = new List<Product>();
		#endregion

		#region Constructors
		public ProductTableDataSource ()
		{
		}
		#endregion

		#region Public Methods
		public void Sort(string key, bool ascending) {

			// Take action based on key
			switch (key) {
			case "Title":
				if (ascending) {
					Products.Sort ((x, y) => x.Title.CompareTo (y.Title));
				} else {
					Products.Sort ((x, y) => -1 * x.Title.CompareTo (y.Title));
				}
				break;
			case "Description":
				if (ascending) {
					Products.Sort ((x, y) => x.Description.CompareTo (y.Description));
				} else {
					Products.Sort ((x, y) => -1 * x.Description.CompareTo (y.Description));
				}
				break;
			}

		}
		#endregion

		#region Override Methods
		public override nint GetRowCount (NSTableView tableView)
		{
			return Products.Count;
		}

		public override void SortDescriptorsChanged (NSTableView tableView, NSSortDescriptor[] oldDescriptors)
		{
			// Sort the data
			Sort (oldDescriptors [0].Key, oldDescriptors [0].Ascending);
			tableView.ReloadData ();
		}
		#endregion
	}
}

