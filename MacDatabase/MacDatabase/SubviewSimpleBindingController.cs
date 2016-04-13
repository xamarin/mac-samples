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
	public partial class SubviewSimpleBindingController : AppKit.NSViewController
	{
		#region Private Variables
		private PersonModel _person = new PersonModel();
		private SqliteConnection Conn;
		#endregion

		#region Computed Properties
		//strongly typed view accessor
		public new SubviewSimpleBinding View {
			get {
				return (SubviewSimpleBinding)base.View;
			}
		}

		[Export("Person")]
		public PersonModel Person {
			get {return _person; }
			set {
				WillChangeValue ("Person");
				_person = value;
				DidChangeValue ("Person");
			}
		}

		public ComboBoxDataSource DataSource {
			get { return EmployeeSelector.DataSource as ComboBoxDataSource; }
		}
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public SubviewSimpleBindingController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewSimpleBindingController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public SubviewSimpleBindingController (SqliteConnection conn) : base ("SubviewSimpleBinding", NSBundle.MainBundle)
		{
			// Initialize
			this.Conn = conn;
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}
		#endregion

		#region Private Methods
		private void LoadSelectedPerson (string id)
		{

			// Found?
			if (id != "") {
				// Yes, load requested record
				Person = new PersonModel (Conn, id);
			}
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Configure Employee selector dropdown
			EmployeeSelector.DataSource = new ComboBoxDataSource (Conn, "People", "Name");

			// Wireup events
			EmployeeSelector.Changed += (sender, e) => {
				// Get ID
				var id = DataSource.IDForValue (EmployeeSelector.StringValue);
				LoadSelectedPerson (id);
			};

			EmployeeSelector.SelectionChanged += (sender, e) => {
				// Get ID
				var id = DataSource.IDForIndex (EmployeeSelector.SelectedIndex);
				LoadSelectedPerson (id);
			};

			// Auto select the first person
			EmployeeSelector.StringValue = DataSource.ValueForIndex (0);
			Person = new PersonModel (Conn, DataSource.IDForIndex(0));
	
		}
		#endregion
	}
}
