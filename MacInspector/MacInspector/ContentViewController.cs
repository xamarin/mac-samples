using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Controls the Content View displayed in the Left side of the <see cref="T:MacInspector.MainSplitViewController"/>.
	/// </summary>
	/// <remarks>
	/// This class uses key-value coding and data binding to set the properties. Please see:
	/// https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/
	/// </remarks>
	public partial class ContentViewController : NSViewController
	{
		#region Computed Properties
		/// <summary>
		/// Gets or sets the split view controller.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.MainSplitViewController"/>.</value>
		public MainSplitViewController SplitViewController { get; set;}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.ContentViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ContentViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called before the View is presented to the user
		/// </summary>
		/// <returns>The will appear.</returns>
		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			// Wireup events
			BoxOne.BoxClicked += (box) => {
				SplitViewController.ShowBoxInspector (BoxOne);
			};

			BoxTwo.BoxClicked += (box) => {
				SplitViewController.ShowBoxInspector (BoxTwo);
			};

			BoxThree.BoxClicked += (box) => {
				SplitViewController.ShowBoxInspector (BoxThree);
			};

			BoxFour.BoxClicked += (box) => {
				SplitViewController.ShowBoxInspector (BoxFour);
			};

			// Observe changes in the document background color
			// Please see: https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/#Observing_Value_Changes
			SplitViewController.DocProperties.AddObserver ("BackgroundColor", NSKeyValueObservingOptions.New, (obj) => {
				// Update document background when the background color changes
				BackgroundBox.FillColor = SplitViewController.DocProperties.BackgroundColor;
			});
		}
		#endregion
	}
}
