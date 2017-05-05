using System;
using System.Collections.Generic;
using AppKit;
using Foundation;

namespace MacCollectionNew
{
	/// <summary>
	/// Collection view data source provides the data for the collection view.
	/// </summary>
	public class CollectionViewDataSource : NSCollectionViewDataSource
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the parent collection view.
		/// </summary>
		/// <value>The parent collection view.</value>
		public NSCollectionView ParentCollectionView { get; set; }

		/// <summary>
		/// Gets or sets the data that will be displayed in the collection.
		/// </summary>
		/// <value>A collection of PersonModel objects.</value>
		public List<PersonModel> Data { get; set; } = new List<PersonModel>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.CollectionViewDataSource"/> class.
		/// </summary>
		/// <param name="parent">The parent collection that this datasource will provide data for.</param>
		public CollectionViewDataSource(NSCollectionView parent)
		{
			// Initialize
			ParentCollectionView = parent;

			// Attach to collection view
			parent.DataSource = this;

		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Gets the number of sections.
		/// </summary>
		/// <returns>The number of sections.</returns>
		/// <param name="collectionView">The parent Collection view.</param>
		public override nint GetNumberOfSections(NSCollectionView collectionView)
		{
			// There is only one section in this view
			return 1;
		}

		/// <summary>
		/// Gets the number of items in the given section.
		/// </summary>
		/// <returns>The number of items.</returns>
		/// <param name="collectionView">The parent Collection view.</param>
		/// <param name="section">The Section number to count items for.</param>
		public override nint GetNumberofItems(NSCollectionView collectionView, nint section)
		{
			// Return the number of items
			return Data.Count;
		}

		/// <summary>
		/// Gets the item for the give section and item index.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="collectionView">The parent Collection view.</param>
		/// <param name="indexPath">Index path specifying the section and index.</param>
		public override NSCollectionViewItem GetItem(NSCollectionView collectionView, NSIndexPath indexPath)
		{
			var item = collectionView.MakeItem("EmployeeCell", indexPath) as EmployeeItemController;
			item.Person = Data[(int)indexPath.Item];

			return item;
		}
		#endregion
	}
}
