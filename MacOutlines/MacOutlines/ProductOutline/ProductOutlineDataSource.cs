using System;
using AppKit;
using CoreGraphics;
using Foundation;
using System.Collections;
using System.Collections.Generic;

namespace MacOutlines
{
	public class ProductOutlineDataSource : NSOutlineViewDataSource
	{
		#region Public Variables
		public List<Product> Products = new List<Product>();
		#endregion

		#region Constructors
		public ProductOutlineDataSource ()
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
			}
		}
		#endregion

		#region Override Methods
		public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			if (item == null) {
				return Products.Count;
			} else {
				return ((Product)item).Products.Count;
			}

		}

		public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
		{
			if (item == null) {
				return Products [(int)childIndex];
			} else {
				return ((Product)item).Products [(int)childIndex];
			}
				
		}

		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
			if (item == null) {
				return Products [0].IsProductGroup;
			} else {
				return ((Product)item).IsProductGroup;
			}
		
		}

		public override void SortDescriptorsChanged (NSOutlineView outlineView, NSSortDescriptor[] oldDescriptors)
		{
			// Sort the data
			Sort (oldDescriptors [0].Key, oldDescriptors [0].Ascending);
			outlineView.ReloadData ();
		}
		#endregion
	}
}

