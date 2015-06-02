using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using SQLite;
using System.IO;

namespace MacDatabase
{
	public partial class SubviewTableORMController : AppKit.NSViewController
	{
		#region Computed Properties
		// Strongly typed view accessor
		public new SubviewTableORM View {
			get {
				return (SubviewTableORM)base.View;
			}
		}

		public SQLiteConnection Conn { get; set; }
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewTableORMController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewTableORMController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewTableORMController () : base ("SubviewTableORM", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion
	
		#region Private Methods
		private SQLiteConnection GetDatabaseConnection() {
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
			string db = Path.Combine (documents, "Occupation.db3");
			OccupationModel Occupation;

			// Create the database if it doesn't already exist
			bool exists = File.Exists (db);

			// Create connection to database
			var conn = new SQLiteConnection (db);

			// Initially populate table?
			if (!exists) {
				// Yes, build table
				conn.CreateTable<OccupationModel> ();

				// Add occupations
				Occupation = new OccupationModel ("Documentation Manager", "Manages the Documentation Group");
				conn.Insert (Occupation);

				Occupation = new OccupationModel ("Technical Writer", "Writes technical documentation and sample applications");
				conn.Insert (Occupation);

				Occupation = new OccupationModel ("Web & Infrastructure", "Creates and maintains the websites that drive documentation");
				conn.Insert (Occupation);

				Occupation = new OccupationModel ("API Documentation Manager", "Manages the API Doucmentation Group");
				conn.Insert (Occupation);

				Occupation = new OccupationModel ("API Documentor", "Creates and maintains API documentation");
				conn.Insert (Occupation);
			}

			return conn;
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Get database connection
			Conn = GetDatabaseConnection ();

			// Create the Occupation Table Data Source and populate it
			var DataSource = new TableORMDatasource (Conn);

			// Populate the Product Table
			OccupationTable.DataSource = DataSource;
			OccupationTable.Delegate = new TableORMDelegate (DataSource);
		}
		#endregion
	}
}
