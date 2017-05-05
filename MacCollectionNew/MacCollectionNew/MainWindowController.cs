using System;
using Foundation;
using AppKit;

namespace MacCollectionNew
{
	/// <summary>
	/// Main window controller.
	/// </summary>
	public partial class MainWindowController : NSWindowController
	{
		#region Computed Properties
		/// <summary>
		/// Gets the collection view controller that is be displayed in the
		/// Window.
		/// </summary>
		/// <value>The collection view controller.</value>
		public ViewController CollectionViewController
		{
			get { return ContentViewController as ViewController; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.MainWindowController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public MainWindowController(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Updates the user interface based on an item in the collection view being selected or not.
		/// </summary>
		private void UpdateUI()
		{

			// Set UI state
			EditToolbarItem.ShouldBeEnabled = (CollectionViewController.PersonSelected != null);
			DeleteToolbarItem.ShouldBeEnabled = EditToolbarItem.ShouldBeEnabled;
			InfoToolbarItem.ShouldBeEnabled = EditToolbarItem.ShouldBeEnabled;
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called when the window has finished being loaded from the Storyboard and allows the
		/// developer to configure the window before it is displayed to the user.
		/// </summary>
		public override void WindowDidLoad()
		{
			base.WindowDidLoad();

			// Initialize
			UpdateUI();

			// Wireup events
			CollectionViewController.SelectionChanged += () =>
			{
				UpdateUI();
			};
		}
		#endregion

		#region Custom Action
		/// <summary>
		/// Adds a new employee to the collection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void AddEmployee(Foundation.NSObject sender) {
			CollectionViewController.AddNewPerson();
		}

		/// <summary>
		/// Deletes the selected employee from the collection.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void DeleteEmployee(Foundation.NSObject sender) {
			CollectionViewController.DeletePerson();
		}

		/// <summary>
		/// Edits the selected employee in the collection view.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void EditEmployee(Foundation.NSObject sender) {
			CollectionViewController.EditPerson();
		}

		/// <summary>
		/// Shows the selected employee's information.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void ShowInfo(Foundation.NSObject sender) {
			CollectionViewController.ShowPersonInfo();
		}
		#endregion
	}
}
