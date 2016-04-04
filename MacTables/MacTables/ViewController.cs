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

		#region Public Methods
		public void ReloadTable ()
		{
			ProductTable.ReloadData ();
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
			DataSource.Products.Add (new Product ("Xamarin.tvOS", "Allows you to develop Apple TV native Applications in C#"));
			DataSource.Products.Add (new Product ("Xamarin Studio Community", "A free, full-featured IDE for Mac users to create Android and iOS apps using Xamarin."));
			DataSource.Products.Add (new Product ("Visual Studio Community", "A free, full-featured and extensible IDE for Windows users to create Android and iOS apps with Xamarin, as well as Windows apps, web apps, and cloud services."));
			DataSource.Products.Add (new Product ("Visual Studio Professional", "Professional developer tools and services for individual developers or small teams."));
			DataSource.Products.Add (new Product ("Visual Studio Enterprise", "End-to-end solution for teams of any size with demanding quality and scale needs"));
			DataSource.Products.Add (new Product ("Xamarin Test Cloud", "Automatically test your app on thousands of mobile devices."));
			DataSource.Products.Add (new Product ("HockyApp", "Bring Mobile DevOps to your apps and reliability to your users."));
			DataSource.Products.Add (new Product ("Xamarin University", "Take your mobile strategy and apps to the next level."));
			DataSource.Sort ("Title", true);

			// Populate the Product Table
			ProductTable.DataSource = DataSource;
			ProductTable.Delegate = new ProductTableDelegate (this, DataSource);

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
