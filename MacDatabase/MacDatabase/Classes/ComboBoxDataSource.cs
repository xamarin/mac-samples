using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Foundation;
using AppKit;

namespace MacDatabase
{
	public class ComboBoxDataSource : NSComboBoxDataSource
	{
		#region Private Variables
		private SqliteConnection _conn = null;
		private string _tableName = "";
		private string _IDField = "ID";
		private string _displayField = "";
		private nint _recordCount = 0;
		#endregion

		#region Computed Properties
		public SqliteConnection Conn {
			get { return _conn; }
			set { _conn = value; }
		}

		public string TableName {
			get { return _tableName; }
			set { 
				_tableName = value;
				_recordCount = GetRecordCount ();
			}
		}

		public string IDField {
			get { return _IDField; }
			set {
				_IDField = value; 
				_recordCount = GetRecordCount ();
			}
		}

		public string DisplayField {
			get { return _displayField; }
			set { 
				_displayField = value; 
				_recordCount = GetRecordCount ();
			}
		}

		public nint RecordCount {
			get { return _recordCount; }
		}
		#endregion

		#region Constructors
		public ComboBoxDataSource (SqliteConnection conn, string tableName, string displayField)
		{
			// Initialize
			this.Conn = conn;
			this.TableName = tableName;
			this.DisplayField = displayField;
		}

		public ComboBoxDataSource (SqliteConnection conn, string tableName, string idField, string displayField)
		{
			// Initialize
			this.Conn = conn;
			this.TableName = tableName;
			this.IDField = idField;
			this.DisplayField = displayField;
		}
		#endregion

		#region Private Methods
		private nint GetRecordCount ()
		{
			bool shouldClose = false;
			nint count = 0;

			// Has a Table, ID and display field been specified?
			if (TableName !="" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT count({IDField}) FROM [{TableName}]";

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Read count from query
							var result = (long)reader [0];
							count = (nint)result;
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return the number of records
			return count;
		}
		#endregion

		#region Public Methods
		public string IDForIndex (nint index)
		{
			NSString value = new NSString ("");
			bool shouldClose = false;

			// Has a Table, ID and display field been specified?
			if (TableName != "" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT {IDField} FROM [{TableName}] ORDER BY {DisplayField} ASC LIMIT 1 OFFSET {index}";

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Read the display field from the query
							value = new NSString ((string)reader [0]);
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return results
			return value;
		}

		public string ValueForIndex (nint index)
		{
			NSString value = new NSString ("");
			bool shouldClose = false;

			// Has a Table, ID and display field been specified?
			if (TableName != "" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT {DisplayField} FROM [{TableName}] ORDER BY {DisplayField} ASC LIMIT 1 OFFSET {index}";

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Read the display field from the query
							value = new NSString ((string)reader [0]);
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return results
			return value;
		}

		public string IDForValue (string value)
		{
			NSString result = new NSString ("");
			bool shouldClose = false;

			// Has a Table, ID and display field been specified?
			if (TableName != "" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT {IDField} FROM [{TableName}] WHERE {DisplayField} = @VAL";

					// Populate parameters
					command.Parameters.AddWithValue ("@VAL", value);

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Read the display field from the query
							result = new NSString ((string)reader [0]);
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return results
			return result;
		}
		#endregion 

		#region Override Methods
		public override nint ItemCount (NSComboBox comboBox)
		{
			return RecordCount;
		}

		public override NSObject ObjectValueForItem (NSComboBox comboBox, nint index)
		{
			NSString value = new NSString ("");
			bool shouldClose = false;

			// Has a Table, ID and display field been specified?
			if (TableName != "" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT {DisplayField} FROM [{TableName}] ORDER BY {DisplayField} ASC LIMIT 1 OFFSET {index}";

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Read the display field from the query
							value = new NSString((string)reader [0]);
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return results
			return value;
		}

		public override nint IndexOfItem (NSComboBox comboBox, string value)
		{
			bool shouldClose = false;
			bool found = false;
			string field = "";
			nint index = NSRange.NotFound;

			// Has a Table, ID and display field been specified?
			if (TableName != "" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT {DisplayField} FROM [{TableName}] ORDER BY {DisplayField} ASC";

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read () && !found) {
							// Read the display field from the query
							field = (string)reader [0];
							++index;

							// Is this the value we are searching for?
							if (value == field) {
								// Yes, exit loop
								found = true;
							}
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return results
			return index;
		}

		public override string CompletedString (NSComboBox comboBox, string uncompletedString)
		{
			bool shouldClose = false;
			bool found = false;
			string field = "";

			// Has a Table, ID and display field been specified?
			if (TableName != "" && IDField != "" && DisplayField != "") {
				// Is the database already open?
				if (Conn.State != ConnectionState.Open) {
					shouldClose = true;
					Conn.Open ();
				}

				// Escape search string
				uncompletedString = uncompletedString.Replace ("'", "");

				// Execute query
				using (var command = Conn.CreateCommand ()) {
					// Create new command
					command.CommandText = $"SELECT {DisplayField} FROM [{TableName}] WHERE {DisplayField} LIKE @VAL";

					// Populate parameters
					command.Parameters.AddWithValue ("@VAL", uncompletedString + "%");

					// Get the results from the database
					using (var reader = command.ExecuteReader ()) {
						while (reader.Read ()) {
							// Read the display field from the query
							field = (string)reader [0];
						}
					}
				}

				// Should we close the connection to the database
				if (shouldClose) {
					Conn.Close ();
				}
			}

			// Return results
			return field;
		}
		#endregion
	}
}

