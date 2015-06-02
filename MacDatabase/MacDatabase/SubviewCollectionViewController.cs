using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Foundation;
using AppKit;

namespace MacDatabase
{
	public partial class SubviewCollectionViewController : AppKit.NSViewController
	{
		#region Private Variables
		private NSMutableArray _people = new NSMutableArray();
		private SqliteConnection _conn = null;
		#endregion

		#region Computed Properties
		//strongly typed view accessor
		public new SubviewCollectionView View {
			get {
				return (SubviewCollectionView)base.View;
			}
		}

		[Export("personModelArray")]
		public NSArray People {
			get { return _people; }
		}

		public PersonModel SelectedPerson { get; private set;}
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewCollectionViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewCollectionViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewCollectionViewController (SqliteConnection conn) : base ("SubviewCollectionView", NSBundle.MainBundle)
		{
			// Initialize
			this._conn = conn;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion

		#region Public Methods
		public void DeletePerson(NSWindow window) {

			// Anything to process?
			if (SelectedPerson == null) {
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = "Please select the person to remove from the collection of people.",
					MessageText = "Delete Person",
				};
				alert.BeginSheet (window);
			} else {
				// Confirm delete
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Critical,
					InformativeText = string.Format ("Are you sure you want to delete person `{0}` from the collection?", SelectedPerson.Name),
					MessageText = "Delete Person",
				};
				alert.AddButton ("Ok");
				alert.AddButton ("Cancel");
				alert.BeginSheetForResponse (window, (result) => {
					// Delete?
					if (result == 1000) {
						// Grab person
						SelectedPerson = _people.GetItem<PersonModel> ((nuint)View.SelectionIndex);

						// Remove from database
						SelectedPerson.Delete(_conn);
						RemovePerson (View.SelectionIndex);
					}
				});
			}
		}

		public void EditPerson(NSWindow window) {
			if (SelectedPerson == null) {
				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Informational,
					InformativeText = "Please select the person to edit from the collection of people.",
					MessageText = "Edit Person",
				};
				alert.BeginSheet (window);
			} else {
				// Grab person
				SelectedPerson = _people.GetItem<PersonModel> ((nuint)View.SelectionIndex);

				var sheet = new PersonEditorSheetController(SelectedPerson, false);

				// Display sheet
				sheet.ShowSheet(window);
			}
		}

		public void FindPerson(string text) {

			// Convert to lower case
			text = text.ToLower ();

			// Scan each person in the list
			for (nuint n = 0; n < _people.Count; ++n) {
				var person = _people.GetItem<PersonModel> (n);
				if (person.Name.ToLower ().Contains (text)) {
					View.SelectionIndex = (nint)n;
					return;
				}
			}

			// Not found, auto select first
			View.SelectionIndex = 0;
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Load all employees
			_conn.Open ();
			using (var command = _conn.CreateCommand ()) {
				// Create new command
				command.CommandText = "SELECT ID FROM [People]";

				using (var reader = command.ExecuteReader ()) {
					while (reader.Read ()) {
						// Load child and add to collection
						var childID = (string)reader [0];
						var person = new PersonModel (_conn, childID);
						AddPerson (person);
					}
				}
			}
			_conn.Close ();

			// Wire-up events
			View.PersonSelected += (index) => {
				try {
					SelectedPerson = _people.GetItem<PersonModel>((nuint)index);
				} catch {
					SelectedPerson = null;
				}
			};

		}
		#endregion

		#region Array Controller Methods
		[Export("addObject:")]
		public void AddPerson(PersonModel person) {
			WillChangeValue ("personModelArray");
			_people.Add (person);
			DidChangeValue ("personModelArray");
		}

		[Export("insertObject:inPersonModelArrayAtIndex:")]
		public void InsertPerson(PersonModel person, nint index) {
			WillChangeValue ("personModelArray");
			_people.Insert (person, index);
			DidChangeValue ("personModelArray");
		}

		[Export("removeObjectFromPersonModelArrayAtIndex:")]
		public void RemovePerson(nint index) {
			WillChangeValue ("personModelArray");
			_people.RemoveObject (index);
			DidChangeValue ("personModelArray");
		}

		[Export("setPersonModelArray:")]
		public void SetPeople(NSMutableArray array) {
			WillChangeValue ("personModelArray");
			_people = array;
			DidChangeValue ("personModelArray");
		}
		#endregion
	}
}
