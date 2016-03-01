using System;
using AppKit;
using Foundation;

namespace MacInspector
{
	/// <summary>
	/// A custom <c>NSSplitViewController</c> that handles the <see cref="T:MacInspector.ContentViewController"/> displayed
	/// on the left side of the split and the Inspector (handled by a <see cref="T:MacInspector.InspectorViewController"/>)
	/// on the right side.
	/// </summary>
	/// <remarks>
	/// This class uses key-value coding and data binding to set the properties. Please see:
	/// https://developer.xamarin.com/guides/mac/application_fundamentals/databinding/
	/// </remarks>
	[Register("MainSplitViewController")]
	public partial class MainSplitViewController : NSSplitViewController
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the <see cref="T:MacInspector.DocumentProperties"/>.
		/// </summary>
		private DocumentProperties _docProperties = new DocumentProperties();

		/// <summary>
		/// The backing store for the Inspector Panel currently being displayed.
		/// </summary>
		private int _inspectorPanelID = 0;
		#endregion 

		#region Computed Properties
		/// <summary>
		/// Gets the content controller in the left side of the split view.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.ContentViewController"/>.</value>
		public ContentViewController ContentController {
			get { return SplitViewItems [0].ViewController as ContentViewController; }
		}

		/// <summary>
		/// Gets the inspector controller in the right side of the split view.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.InspectorViewController"/>.</value>
		public InspectorViewController InspectorController {
			get { return SplitViewItems [1].ViewController as InspectorViewController; }
		}

		/// <summary>
		/// Gets or sets the document properties.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.DocumentProperties"/> for this Split View.</value>
		[Export("DocProperties")]
		public DocumentProperties DocProperties {
			get { return _docProperties; }
			set {
				WillChangeValue ("DocProperties");
				_docProperties = value;
				DidChangeValue ("DocProperties");
			}
		}

		/// <summary>
		/// Gets or sets the inspector panel identifier for the panel currently being displayed.
		/// </summary>
		/// <value>The inspector panel identifier.</value>
		/// <remarks>
		/// This is the ID of the item currently selected in the Format Segment View: 0 - Format,
		/// 1 - Document.
		/// </remarks>
		[Export("InspectorPanelID")]
		public int InspectorPanelID {
			get { return _inspectorPanelID; }
			set {
				WillChangeValue ("InspectorPanelID");
				_inspectorPanelID = value;
				DidChangeValue ("InspectorPanelID");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.MainSplitViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MainSplitViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called before the view is displayed so it can be configured.
		/// </summary>
		/// <returns>The will appear.</returns>
		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			// Attach to sub view controllers
			ContentController.SplitViewController = this;
			InspectorController.SplitViewController = this;

			// Update GUI
			ShowDocumentInspector ();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Removes the panel currently being displayed in the Inspector.
		/// </summary>
		public void ClearInspector ()
		{
			// Remove current inspector panel
			InspectorController.ClearInspectorPanel ();
			InspectorPanelID = 0;
		}

		/// <summary>
		/// Shows the box inspector.
		/// </summary>
		/// <param name="box">The <see cref="T:MacInspector.CustomBox"/> being displayed/edited.</param>
		public void ShowBoxInspector (CustomBox box)
		{
			// Are we on the Format inspector?
			if (InspectorPanelID != 0) return;

			// Show Box Inspector
			InspectorController.ShowBoxInspector (box);
		}

		/// <summary>
		/// Shows the document inspector.
		/// </summary>
		public void ShowDocumentInspector ()
		{
			// Are we already showing the Document Inspector?
			if (InspectorPanelID == 1) return;

			// Show Document Inspector
			InspectorController.ShowDocumentInspector (DocProperties);
			InspectorPanelID = 1;
		}
		#endregion
	}
}

