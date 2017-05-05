using System;
using AppKit;
using Foundation;
using CoreGraphics;

namespace MacCollectionNew
{
	/// <summary>
	/// The View controller controls the main view that houses the Collection View.
	/// </summary>
	public partial class ViewController : NSViewController
	{
		#region Private Variables
		private PersonModel _personSelected;
		private bool shouldEdit = true;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the datasource that provides the data to display in the 
		/// Collection View.
		/// </summary>
		/// <value>The datasource.</value>
		public CollectionViewDataSource Datasource { get; set; }

		/// <summary>
		/// Gets or sets the person currently selected in the collection view.
		/// </summary>
		/// <value>The person selected or <c>null</c> if no person is selected.</value>
		[Export("PersonSelected")]
		public PersonModel PersonSelected
		{
			get { return _personSelected; }
			set
			{
				WillChangeValue("PersonSelected");
				_personSelected = value;
				DidChangeValue("PersonSelected");
				RaiseSelectionChanged();
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.ViewController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public ViewController(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region Override Methods
		/// <summary>
		/// Called after the view has finished loading from the Storyboard to allow it to
		/// be configured before displaying to the user.
		/// </summary>
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Initialize Collection View
			ConfigureCollectionView();
			PopulateWithData();
		}

		/// <summary>
		/// Prepares for segue by configuring it before it is presented to the user.
		/// </summary>
		/// <param name="segue">The Segue being presented.</param>
		/// <param name="sender">Sender.</param>
		public override void PrepareForSegue(NSStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue(segue, sender);

			// Take action based on segue type
			switch(segue.Identifier){
				case "EditorSegue":
					var editor = segue.DestinationController as PersonEditorController;
					editor.Presentor = this;
					editor.CanEdit = shouldEdit;
					editor.Person = PersonSelected;
					break;
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Configures the collection view.
		/// </summary>
		private void ConfigureCollectionView()
		{
			EmployeeCollection.RegisterClassForItem(typeof(EmployeeItemController), "EmployeeCell");

			// Create a flow layout
			var flowLayout = new NSCollectionViewFlowLayout()
			{
				ItemSize = new CGSize(150, 150),
				SectionInset = new NSEdgeInsets(10, 10, 10, 20),
				MinimumInteritemSpacing = 10,
				MinimumLineSpacing = 10
			};
			EmployeeCollection.WantsLayer = true;

			// Setup collection view
			EmployeeCollection.CollectionViewLayout = flowLayout;
			EmployeeCollection.Delegate = new CollectionViewDelegate(this);

		}

		/// <summary>
		/// Populates the Datasource with data and attaches it to the collection view.
		/// </summary>
		private void PopulateWithData()
		{
			// Make datasource
			Datasource = new CollectionViewDataSource(EmployeeCollection);

			// Build list of employees
			Datasource.Data.Add(new PersonModel("Craig Dunn", "Documentation Manager", true));
			Datasource.Data.Add(new PersonModel("Amy Burns", "Technical Writer"));
			Datasource.Data.Add(new PersonModel("Joel Martinez", "Web & Infrastructure"));
			Datasource.Data.Add(new PersonModel("Kevin Mullins", "Technical Writer"));
			Datasource.Data.Add(new PersonModel("Mark McLemore", "Technical Writer"));
			Datasource.Data.Add(new PersonModel("Tom Opgenorth", "Technical Writer"));
			Datasource.Data.Add(new PersonModel("Larry O'Brien", "API Docs Manager", true));
			Datasource.Data.Add(new PersonModel("Mike Norman", "API Documentor"));

			// Populate collection view
			EmployeeCollection.ReloadData();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Shows the person info for the currently selected person in the collection view.
		/// </summary>
		public void ShowPersonInfo() {
			// Display the Person editor
			shouldEdit = false;
			PerformSegue("EditorSegue", this);
		}

		/// <summary>
		/// Adds a new person to the collection view.
		/// </summary>
		public void AddNewPerson() {

			// Create new person, add to collection and update
			PersonSelected = new PersonModel();
			Datasource.Data.Add(PersonSelected);
			EmployeeCollection.ReloadData();

			// Select the new person
			var index = EmployeeCollection.GetNumberOfItems(0) - 1;
			EmployeeCollection.SelectionIndexes = new NSIndexSet(index);

			// Display the Person editor
			shouldEdit = true;
			PerformSegue("EditorSegue", this);
		}

		/// <summary>
		/// Edits the person currently selected in the collection view.
		/// </summary>
		public void EditPerson() {
			// Display the Person editor
			shouldEdit = true;
			PerformSegue("EditorSegue", this);
		}

		/// <summary>
		/// Deletes the person currently selected from the collection view.
		/// </summary>
		public void DeletePerson() {

			// Confirm removal
			var alert = new NSAlert()
			{
				AlertStyle = NSAlertStyle.Informational,
				InformativeText = $"Are you sure you want to delete {PersonSelected.Name}?.",
				MessageText = "Delete Employee",
			};
			alert.AddButton("Ok");
			alert.AddButton("Cancel");
			alert.BeginSheetForResponse(this.View.Window, (result) =>
			{
				// Did the user confirm the deletion?
				if (result == 1000) {
					// Yes, remove the account
					Datasource.Data.Remove(PersonSelected);
					PersonSelected = null;
					EmployeeCollection.ReloadData();
				}
			});

		}
		#endregion

		#region Events
		/// <summary>
		/// Selection changed delegate.
		/// </summary>
		public delegate void SelectionChangedDelegate();

		/// <summary>
		/// Occurs when selection changed.
		/// </summary>
		public event SelectionChangedDelegate SelectionChanged;

		/// <summary>
		/// Raises the selection changed event.
		/// </summary>
		internal void RaiseSelectionChanged() {
			// Inform caller
			if (this.SelectionChanged != null) SelectionChanged();
		}
		#endregion
	}
}
