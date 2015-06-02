using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Foundation;
using AppKit;

namespace MacDatabase
{
	[Register("PersonModel")]
	public class PersonModel : NSObject
	{
		#region Private Variables
		private string _ID = "";
		private string _managerID = "";
		private string _name = "";
		private string _occupation = "";
		private bool _isManager = false;
		private NSMutableArray _people = new NSMutableArray();
		private SqliteConnection _conn = null;
		#endregion

		#region Computed Properties
		public SqliteConnection Conn {
			get { return _conn; }
			set { _conn = value; }
		}

		[Export("ID")]
		public string ID {
			get { return _ID; }
			set {
				WillChangeValue ("ID");
				_ID = value;
				DidChangeValue ("ID");
			}
		}

		[Export("ManagerID")]
		public string ManagerID {
			get { return _managerID; }
			set {
				WillChangeValue ("ManagerID");
				_managerID = value;
				DidChangeValue ("ManagerID");
			}
		}

		[Export("Name")]
		public string Name {
			get { return _name; }
			set {
				WillChangeValue ("Name");
				_name = value;
				DidChangeValue ("Name");

				// Save changes to database?
				if (_conn != null) Update (_conn);
			}
		}

		[Export("Occupation")]
		public string Occupation {
			get { return _occupation; }
			set {
				WillChangeValue ("Occupation");
				_occupation = value;
				DidChangeValue ("Occupation");

				// Save changes to database?
				if (_conn != null) Update (_conn);
			}
		}

		[Export("isManager")]
		public bool isManager {
			get { return _isManager; }
			set {
				WillChangeValue ("isManager");
				WillChangeValue ("Icon");
				_isManager = value;
				DidChangeValue ("isManager");
				DidChangeValue ("Icon");

				// Save changes to database?
				if (_conn != null) Update (_conn);
			}
		}

		[Export("isEmployee")]
		public bool isEmployee {
			get { return (NumberOfEmployees == 0); }
		}

		[Export("Icon")]
		public NSImage Icon {
			get {
				if (isManager) {
					return NSImage.ImageNamed ("group.png");
				} else {
					return NSImage.ImageNamed ("user.png");
				}
			}
		}

		[Export("personModelArray")]
		public NSArray People {
			get { return _people; }
		}

		[Export("NumberOfEmployees")]
		public nint NumberOfEmployees {
			get { return (nint)_people.Count; }
		}
		#endregion

		#region Constructors
		public PersonModel ()
		{
		}

		public PersonModel (string name, string occupation)
		{
			// Initialize
			this.Name = name;
			this.Occupation = occupation;
		}

		public PersonModel (string name, string occupation, bool manager)
		{
			// Initialize
			this.Name = name;
			this.Occupation = occupation;
			this.isManager = manager;
		}

		public PersonModel (string id, string name, string occupation)
		{
			// Initialize
			this.ID = id;
			this.Name = name;
			this.Occupation = occupation;
		}

		public PersonModel (SqliteConnection conn, string id)
		{
			// Load from database
			Load (conn, id);
		}
		#endregion

		#region Array Controller Methods
		[Export("addObject:")]
		public void AddPerson(PersonModel person) {
			WillChangeValue ("personModelArray");
			isManager = true;
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

		#region SQLite Routines
		public void Create(SqliteConnection conn) {

			// Clear last connection to prevent circular call to update
			_conn = null;

			// Create new record ID?
			if (ID == "") {
				ID = Guid.NewGuid ().ToString();
			}

			// Execute query
			conn.Open ();
			using (var command = conn.CreateCommand ()) {
				// Create new command
				command.CommandText = "INSERT INTO [People] (ID, Name, Occupation, isManager, ManagerID) VALUES (@COL1, @COL2, @COL3, @COL4, @COL5)";

				// Populate with data from the record
				command.Parameters.AddWithValue ("@COL1", ID);
				command.Parameters.AddWithValue ("@COL2", Name);
				command.Parameters.AddWithValue ("@COL3", Occupation);
				command.Parameters.AddWithValue ("@COL4", isManager);
				command.Parameters.AddWithValue ("@COL5", ManagerID);

				// Write to database
				command.ExecuteNonQuery ();
			}
			conn.Close ();

			// Save children to database as well
			for (nuint n = 0; n < People.Count; ++n) {
				// Grab person
				var Person = People.GetItem<PersonModel>(n);

				// Save manager ID and create the sub record
				Person.ManagerID = ID;
				Person.Create (conn);
			}

			// Save last connection
			_conn = conn;
		}

		public void Update(SqliteConnection conn) {

			// Clear last connection to prevent circular call to update
			_conn = null;

			// Execute query
			conn.Open ();
			using (var command = conn.CreateCommand ()) {
				// Create new command
				command.CommandText = "UPDATE [People] SET Name = @COL2, Occupation = @COL3, isManager = @COL4, ManagerID = @COL5 WHERE ID = @COL1";

				// Populate with data from the record
				command.Parameters.AddWithValue ("@COL1", ID);
				command.Parameters.AddWithValue ("@COL2", Name);
				command.Parameters.AddWithValue ("@COL3", Occupation);
				command.Parameters.AddWithValue ("@COL4", isManager);
				command.Parameters.AddWithValue ("@COL5", ManagerID);

				// Write to database
				command.ExecuteNonQuery ();
			}
			conn.Close ();

			// Save children to database as well
			for (nuint n = 0; n < People.Count; ++n) {
				// Grab person
				var Person = People.GetItem<PersonModel>(n);

				// Update sub record
				Person.Update (conn);
			}

			// Save last connection
			_conn = conn;
		}

		public void Load(SqliteConnection conn, string id) {
			bool shouldClose = false;

			// Clear last connection to prevent circular call to update
			_conn = null;

			// Is the database already open?
			if (conn.State != ConnectionState.Open) {
				shouldClose = true;
				conn.Open ();
			}

			// Execute query
			using (var command = conn.CreateCommand ()) {
				// Create new command
				command.CommandText = "SELECT * FROM [People] WHERE ID = @COL1";

				// Populate with data from the record
				command.Parameters.AddWithValue ("@COL1", id);

				using (var reader = command.ExecuteReader ()) {
					while (reader.Read ()) {
						// Pull values back into class
						ID = (string)reader [0];
						Name = (string)reader [1];
						Occupation = (string)reader [2];
						isManager = (bool)reader [3];
						ManagerID = (string)reader [4];
					}
				}
			}

			// Is this a manager?
			if (isManager) {
				// Yes, load children
				using (var command = conn.CreateCommand ()) {
					// Create new command
					command.CommandText = "SELECT ID FROM [People] WHERE ManagerID = @COL1";

					// Populate with data from the record
					command.Parameters.AddWithValue ("@COL1", id);

					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Load child and add to collection
							var childID = (string)reader [0];
							var person = new PersonModel (conn, childID);
							_people.Add (person);
						}
					}
				}
			}

			// Should we close the connection to the database
			if (shouldClose) {
				conn.Close ();
			}

			// Save last connection
			_conn = conn;
		}

		public void Delete(SqliteConnection conn) {

			// Clear last connection to prevent circular call to update
			_conn = null;

			// Execute query
			conn.Open ();
			using (var command = conn.CreateCommand ()) {
				// Create new command
				command.CommandText = "DELETE FROM [People] WHERE (ID = @COL1 OR ManagerID = @COL1)";

				// Populate with data from the record
				command.Parameters.AddWithValue ("@COL1", ID);

				// Write to database
				command.ExecuteNonQuery ();
			}
			conn.Close ();

			// Empty class
			ID = "";
			ManagerID = "";
			Name = "";
			Occupation = "";
			isManager = false;
			_people = new NSMutableArray();

			// Save last connection
			_conn = conn;
		}
		#endregion
	}
}

