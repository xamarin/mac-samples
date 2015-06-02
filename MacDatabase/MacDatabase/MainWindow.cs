using System;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using Foundation;
using AppKit;
using CoreGraphics;

namespace MacDatabase
{
	public partial class MainWindow : NSWindow
	{
		#region Private Variables
		private SubviewType ViewType = SubviewType.None;
		private NSViewController SubviewController = null;
		private NSView Subview = null;
		private SqliteConnection DatabaseConnection = null;
		#endregion

		#region Constructors
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}
		#endregion

		#region Private Methods
		private void DisplaySubview(NSViewController controller, SubviewType type) {

			// Is this view already displayed?
			if (ViewType == type) return;

			// Is there a view already being displayed?
			if (Subview != null) {
				// Yes, remove it from the view
				Subview.RemoveFromSuperview ();

				// Release memory
				Subview = null;
				SubviewController = null;
			}

			// Save values
			ViewType = type;
			SubviewController = controller;
			Subview = controller.View;

			// Define frame and display
			Subview.Frame = new CGRect (0, 0, ViewContainer.Frame.Width, ViewContainer.Frame.Height);
			ViewContainer.AddSubview (Subview);

			// Take action on type
			switch (type) {
			case SubviewType.TableBinding:
				AddButton.Active = true;
				EditButton.Active = true;
				DeleteButton.Active = true;
				Search.Enabled = true;
				break;
			case SubviewType.CollectionView:
				AddButton.Active = true;
				EditButton.Active = true;
				DeleteButton.Active = true;
				Search.Enabled = true;
				break;
			default:
				AddButton.Active = false;
				EditButton.Active = false;
				DeleteButton.Active = false;
				Search.Enabled = false;
				break;
			}
		}

		private SqliteConnection GetDatabaseConnection() {
			var documents = Environment.GetFolderPath (Environment.SpecialFolder.Desktop);
			string db = Path.Combine (documents, "People.db3");

			// Create the database if it doesn't already exist
			bool exists = File.Exists (db);
			if (!exists)
				SqliteConnection.CreateFile (db);

			// Create connection to the database
			var conn = new SqliteConnection("Data Source=" + db);

			// Set the structure of the database
			if (!exists) {
				var commands = new[] {
					"CREATE TABLE People (ID TEXT, Name TEXT, Occupation TEXT, isManager BOOLEAN, ManagerID TEXT)"
				};
				conn.Open ();
				foreach (var cmd in commands) {
					using (var c = conn.CreateCommand()) {
						c.CommandText = cmd;
						c.CommandType = CommandType.Text;
						c.ExecuteNonQuery ();
					}
				}
				conn.Close ();

				// Build list of employees
				var Craig = new PersonModel ("0","Craig Dunn", "Documentation Manager");
				Craig.AddPerson (new PersonModel ("Amy Burns", "Technical Writer"));
				Craig.AddPerson (new PersonModel ("Joel Martinez", "Web & Infrastructure"));
				Craig.AddPerson (new PersonModel ("Kevin Mullins", "Technical Writer"));
				Craig.AddPerson (new PersonModel ("Mark McLemore", "Technical Writer"));
				Craig.AddPerson (new PersonModel ("Tom Opgenorth", "Technical Writer"));
				Craig.Create (conn);

				var Larry = new PersonModel ("1","Larry O'Brien", "API Documentation Manager");
				Larry.AddPerson (new PersonModel ("Mike Norman", "API Documentor"));
				Larry.Create (conn);
			}

			// Return new connection
			return conn;
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Get access to database
			DatabaseConnection = GetDatabaseConnection ();

			// Wire-up controls
			AddButton.Active = false;
			AddButton.Activated += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					var person = new PersonModel("Unknown","Unknown");
					var sheet = new PersonEditorSheetController(person, true);

					// Wire-up
					sheet.PersonModified += (p) => {
						// Save person to database
						p.Create(DatabaseConnection);
						controller.AddPerson(p);
					};

					// Display sheet
					sheet.ShowSheet(this);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					var collectionPerson = new PersonModel("Unknown","Unknown");
					var collectionSheet = new PersonEditorSheetController(collectionPerson, true);

					// Wire-up
					collectionSheet.PersonModified += (p) => {
						// Save person to database
						p.Create(DatabaseConnection);
						collection.AddPerson(p);
					};

					// Display sheet
					collectionSheet.ShowSheet(this);
					break;
				}
			};

			EditButton.Active = false;
			EditButton.Activated += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					controller.EditPerson(this);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					collection.EditPerson(this);
					break;
				}
			};

			DeleteButton.Active = false;
			DeleteButton.Activated += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					controller.DeletePerson(this);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					collection.DeletePerson(this);
					break;
				}
			};

			Search.Enabled = false;
			Search.EditingEnded += (sender, e) => {
				// Take action based on type
				switch(ViewType) {
				case SubviewType.TableBinding:
					var controller = SubviewController as SubviewTableBindingController;
					controller.FindPerson(Search.StringValue);
					break;
				case SubviewType.CollectionView:
					var collection = SubviewController as SubviewCollectionViewController;
					collection.FindPerson(Search.StringValue);
					break;
				}
			};

			// Populate Source List
			SourceList.Initialize ();

			var TableViews = new SourceListItem ("Direct SQLite");
			TableViews.AddItem ("Simple Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewSimpleBindingController(DatabaseConnection), SubviewType.SimpleBinding);
			});
			TableViews.AddItem ("Table Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewTableBindingController(DatabaseConnection), SubviewType.TableBinding);
			});
			TableViews.AddItem ("Outline Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewOutlineBindingController(DatabaseConnection), SubviewType.OutlineBinging);
			});
			TableViews.AddItem ("Collection View", "shoebox.png", () => {
				DisplaySubview(new SubviewCollectionViewController(DatabaseConnection), SubviewType.CollectionView);
			});
			SourceList.AddItem (TableViews);

			var ORMViews = new SourceListItem ("SQLite.Net ORM");
			ORMViews.AddItem ("Table Binding", "shoebox.png", () => {
				DisplaySubview(new SubviewTableORMController(), SubviewType.TableORM);
			});
			SourceList.AddItem (ORMViews);

			// Display Source List
			SourceList.ReloadData();
			SourceList.ExpandItem (null, true);
		}
		#endregion
	}
}
