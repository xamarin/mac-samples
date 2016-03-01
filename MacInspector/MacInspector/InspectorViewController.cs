using System;
using Foundation;
using AppKit;

namespace MacInspector
{
	/// <summary>
	/// Controls the Inspector Panel displayed in the right hand side of the 
	/// <see cref="T:MacInspector.MainSplitViewController"/>. This controller is responsible for
	/// switching out Inspector Panels as the context changes inside of the app.
	/// </summary>
	/// <remarks>
	/// Please see:
	/// https://developer.xamarin.com/guides/mac/user-interface/working-with-windows/#Inspectors
	/// https://developer.xamarin.com/guides/mac/platform-features/storyboards/quickstart/
	/// </remarks>
	public partial class InspectorViewController : NSViewController
	{
		#region Private Variables
		/// <summary>
		/// The backing store for the properties currently being displayed and edited.
		/// </summary>
		private NSObject _inspectedProperties = null;

		/// <summary>
		/// The backing store for the currently loaded Inspector Panel.
		/// </summary>
		private NSViewController _inspectorPanel = null;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the split view controller.
		/// </summary>
		/// <value>The <see cref="T:MacInspector.MainSplitViewController"/> this controller is attached to.</value>
		public MainSplitViewController SplitViewController { get; set;}

		/// <summary>
		/// Gets or sets the inspected properties currently being displayed and edited.
		/// </summary>
		/// <value>The inspected properties.</value>
		[Export("InspectedProperties")]
		public NSObject InspectedProperties {
			get { return _inspectedProperties; }
			set {
				WillChangeValue ("InspectedProperties");
				_inspectedProperties = value;
				DidChangeValue ("InspectedProperties");
			}
		}

		/// <summary>
		/// Gets or sets the inspector panel currently being displayed.
		/// </summary>
		/// <value>The inspector panel.</value>
		[Export("InspectorPanel")]
		public NSViewController InspectorPanel {
			get { return _inspectorPanel; }
			set {
				WillChangeValue ("InspectorPanel");
				_inspectorPanel = value;
				DidChangeValue ("InspectorPanel");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacInspector.InspectorViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public InspectorViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called before a segue is launched to allow you to configure the new view controller
		/// that will be displayed.
		/// </summary>
		/// <param name="segue">The Segue that is about to be launched.</param>
		/// <param name="sender">The View Controller that is launching the segue.</param>
		public override void PrepareForSegue (NSStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			// Take action based on the segue type
			switch (segue.Identifier) {
			case "DocPrefsSegue":
				// Configure the Document Preferences Inspector Panel
				var docController = segue.DestinationController as DocPrefsViewController;
				docController.Properties = InspectedProperties as DocumentProperties;
				break;
			case "BoxPrefsSegue":
				// Configure the Box Preferences Inspector Panel
				var boxController = segue.DestinationController as BoxPrefsViewController;
				boxController.Box = InspectedProperties as CustomBox;
				break;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Sets the inspector panel.
		/// </summary>
		/// <param name="panel">The new Inspector Panel (<c>View Controller</c>) to display.</param>
		/// <remarks>If the <c>panel</c> is <c>null</c>, the current panel is removed from the 
		/// screen.</remarks>
		public void SetInspectorPanel (NSViewController panel)
		{
			// Remove existing panel
			if (InspectorPanel != null) {
				InspectorPanel.View.RemoveFromSuperview ();
				InspectorPanel.RemoveFromParentViewController ();
			}

			// Save new panel
			InspectorPanel = panel;

			// Insert new panel
			if (panel != null) {
				AddChildViewController (panel);
				View.AddSubview (panel.View);
			}
		}

		/// <summary>
		/// Removes the current Inspector Panel (<c>View Controller</c>) from the screen.
		/// </summary>
		public void ClearInspectorPanel ()
		{
			// Send a null panel to the set method
			// to remove the current panel
			SetInspectorPanel (null);
		}

		/// <summary>
		/// Shows the document inspector.
		/// </summary>
		/// <param name="properties">The <see cref="T:MacInspector.DocumentProperties"/> being displayed/edited.</param>
		public void ShowDocumentInspector (DocumentProperties properties)
		{

			// Save new properties and display document preference
			// editor
			InspectedProperties = properties;
			PerformSegue ("DocPrefsSegue", this);
		}

		/// <summary>
		/// Shows the box inspector.
		/// </summary>
		/// <param name="box">The <see cref="T:MacInspector.CustomBox"/> being displayed/edited.</param>
		public void ShowBoxInspector (CustomBox box)
		{
			// Save new properties and display box preference
			// editor
			InspectedProperties = box;
			PerformSegue ("BoxPrefsSegue", this);
		}
		#endregion
	}
}
