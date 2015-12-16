using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacDatabinding
{
	public partial class SubviewSimpleBindingController : AppKit.NSViewController
	{
		#region Private Variables
		private PersonModel _person = new PersonModel();
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
		public SubviewSimpleBindingController () : base ("SubviewSimpleBinding", NSBundle.MainBundle)
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

			// Set a default person
			var Craig = new PersonModel ("Craig Dunn", "Documentation Manager");
			Craig.AddPerson (new PersonModel ("Amy Burns", "Technical Writer"));
			Craig.AddPerson (new PersonModel ("Joel Martinez", "Web & Infrastructure"));
			Craig.AddPerson (new PersonModel ("Kevin Mullins", "Technical Writer"));
			Craig.AddPerson (new PersonModel ("Mark McLemore", "Technical Writer"));
			Craig.AddPerson (new PersonModel ("Tom Opgenorth", "Technical Writer"));
			Person = Craig;
		}
		#endregion
	}
}
