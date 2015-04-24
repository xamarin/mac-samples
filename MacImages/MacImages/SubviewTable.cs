using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacImages
{
	public partial class SubviewTable : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public SubviewTable (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTable (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Create the Product Table Data Source and populate it
			var DataSource = new ProductTableDataSource ();
			DataSource.Products.Add (new Product ("Xamarin.iOS", "Allows you to develop native iOS Applications in C#"));
			DataSource.Products.Add (new Product ("Xamarin.Android", "Allows you to develop native Android Applications in C#"));
			DataSource.Products.Add (new Product ("Xamarin.Mac", "Allows you to develop Mac native Applications in C#"));
			DataSource.Sort ("Title", true);

			// Populate the Product Table
			ProductTable.DataSource = DataSource;
			ProductTable.Delegate = new ProductTableDelegate (DataSource);

			// Auto select the first row
			ProductTable.SelectRow (0, false);
		}
		#endregion
	}
}
