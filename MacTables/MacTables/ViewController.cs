using System;

using AppKit;
using Foundation;

namespace MacTables
{
	public partial class ViewController : NSViewController
	{
		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}
		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

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

		#region Menu Handlers
		[Export("selectAll:")]
		public void SelectAll(NSObject sender)
		{
			ProductTable.SelectAll (this);
		}

		[Export("deselectAll:")]
		public void DeselectAll(NSObject sender)
		{
			ProductTable.DeselectAll (this);
		}
		#endregion
	}
}
