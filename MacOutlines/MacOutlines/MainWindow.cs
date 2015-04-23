using System;

using Foundation;
using AppKit;

namespace MacOutlines
{
	public partial class MainWindow : NSWindow
	{
		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Create data source and populate
			var DataSource = new ProductOutlineDataSource ();

			var Vegetables = new Product ("Vegetables", "Greens and Other Produce");
			Vegetables.Products.Add (new Product ("Cabbage", "Brassica oleracea - Leaves, axillary buds, stems, flowerheads"));
			Vegetables.Products.Add (new Product ("Turnip", "Brassica rapa - Tubers, leaves"));
			Vegetables.Products.Add (new Product ("Radish", "Raphanus sativus - Roots, leaves, seed pods, seed oil, sprouting"));
			Vegetables.Products.Add (new Product ("Carrot", "Daucus carota - Root tubers"));
			DataSource.Products.Add (Vegetables);

			var Fruits = new Product ("Fruits", "Fruit is a part of a flowering plant that derives from specific tissues of the flower");
			Fruits.Products.Add (new Product ("Grape", "True Berry"));
			Fruits.Products.Add (new Product ("Cucumber", "Pepo"));
			Fruits.Products.Add (new Product ("Orange", "Hesperidium"));
			Fruits.Products.Add (new Product ("Blackberry", "Aggregate fruit"));
			DataSource.Products.Add (Fruits);

			var Meats = new Product ("Meats", "Lean Cuts");
			Meats.Products.Add (new Product ("Beef", "Cow"));
			Meats.Products.Add (new Product ("Pork", "Pig"));
			Meats.Products.Add (new Product ("Veal", "Young Cow"));
			DataSource.Products.Add (Meats);

			// Initially sort the datasource
			DataSource.Sort ("Title", true);

			// Populate the outline
			ProductOutline.DataSource = DataSource;
			ProductOutline.Delegate = new ProductOutlineDelegate (DataSource);

		}
		#endregion
	}
}
