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
	public partial class SubviewOutlineBindingController : AppKit.NSViewController
	{
		#region Private Variables
		private NSMutableArray _people = new NSMutableArray();
		private SqliteConnection _conn = null;
		#endregion

		#region Computed Properties
		//strongly typed view accessor
		public new SubviewOutlineBinding View {
			get {
				return (SubviewOutlineBinding)base.View;
			}
		}

		[Export("personModelArray")]
		public NSArray People {
			get { return _people; }
		}
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewOutlineBindingController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewOutlineBindingController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewOutlineBindingController (SqliteConnection conn) : base ("SubviewOutlineBinding", NSBundle.MainBundle)
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

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Load only managers employees
			_conn.Open ();
			using (var command = _conn.CreateCommand ()) {
				// Create new command
				command.CommandText = "SELECT ID FROM [People] WHERE isManager = 1";

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
