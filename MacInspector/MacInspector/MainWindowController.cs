using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Controls the <see cref="T:MacInspector.MainWindow"/> displayed for the app and manages
	/// the toolbar.
	/// </summary>
	/// <remarks>
	/// Please see:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-toolbars/
	/// </remarks>
	public partial class MainWindowController : NSWindowController
	{
		#region Computed Properties
		/// <summary>
		/// Gets the split view controller.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.MainSplitViewController"/> contained in the 
		/// <see cref="T:MacInspector.MainWindow"/>.</value>
		public MainSplitViewController SplitViewController {
			get { return Window.ContentViewController as MainSplitViewController; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.MainWindowController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Actions
		/// <summary>
		/// Handles the user clicking on the Format Segment Control in the Toolbar.
		/// </summary>
		/// <returns>The selected.</returns>
		/// <param name="sender">Sender.</param>
		partial void FormatSelected (Foundation.NSObject sender)
		{
			// Take action based on the selected segment
			switch (FormatSegment.SelectedSegment) {
			case 0:
				// Display the general format inspector
				FormatItem.Label = "Format";
				SplitViewController.ClearInspector ();
				break;
			case 1:
				// Display the document format inspector
				FormatItem.Label = "Document";
				SplitViewController.ShowDocumentInspector ();
				break;
			}
		}
		#endregion
	}
}
