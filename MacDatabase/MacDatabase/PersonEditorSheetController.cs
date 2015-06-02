using System;

using Foundation;
using AppKit;


namespace MacDatabase
{
	public class PersonEditorSheetController : NSObject
	{
		#region Private Variables
		private PersonModel _person;
		#endregion

		#region Outlets
		[Outlet]
		AppKit.NSButton CancelButton { get; set; }
		#endregion

		#region Computed Properties
		[Export("window")]
		public PersonEditSheet Window { get; set;}

		[Export("Person")]
		public PersonModel Person {
			get { return _person; }
			set {
				WillChangeValue ("Person");
				_person = value;
				DidChangeValue ("Person");
			}
		}
		#endregion

		#region Constructors
		public PersonEditorSheetController (PersonModel person, bool isNew)
		{
			// Load the .xib file for the sheet
			NSBundle.LoadNib ("PersonEditSheet", this);

			CancelButton.Hidden = !isNew;

			// Save person
			Person = person;
		}
		#endregion

		#region Public Methods
		public void ShowSheet(NSWindow inWindow) {
			NSApplication.SharedApplication.BeginSheet (Window, inWindow);
		}

		public void CloseSheet() {
			NSApplication.SharedApplication.EndSheet (Window);
			Window.Close();
		}
		#endregion

		#region Actions
		[Action ("CancelAction:")]
		public void CancelAction (Foundation.NSObject sender){
			CloseSheet();
		}

		[Action ("OkAction:")]
		public void OkAction (Foundation.NSObject sender){
			RaisePersonModified(Person);
			CloseSheet();
		}
		#endregion

		#region Events
		public delegate void PersonModifiedDelegate(PersonModel person);
		public event PersonModifiedDelegate PersonModified;

		internal void RaisePersonModified(PersonModel person) {
			if (this.PersonModified!=null) this.PersonModified(person);
		}
		#endregion
	}
}

