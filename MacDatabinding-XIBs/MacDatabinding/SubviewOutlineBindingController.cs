using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacDatabinding
{
	public partial class SubviewOutlineBindingController : AppKit.NSViewController
	{
		#region Private Variables
		private NSMutableArray _people = new NSMutableArray();
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
		public SubviewOutlineBindingController () : base ("SubviewOutlineBinding", NSBundle.MainBundle)
		{
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

			// Build list of employees
			var Craig = new PersonModel ("Craig Dunn", "Documentation Manager");
			Craig.AddPerson (new PersonModel ("Amy Burns", "Technical Writer"));
			Craig.AddPerson (new PersonModel ("Joel Martinez", "Web & Infrastructure"));
			Craig.AddPerson (new PersonModel ("Kevin Mullins", "Technical Writer"));
			Craig.AddPerson (new PersonModel ("Mark McLemore", "Technical Writer"));
			Craig.AddPerson (new PersonModel ("Tom Opgenorth", "Technical Writer"));
			AddPerson (Craig);

			var Larry = new PersonModel ("Larry O'Brien", "API Documentation Manager");
			Larry.AddPerson (new PersonModel ("Mike Norman", "API Documentor"));
			AddPerson (Larry);

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
