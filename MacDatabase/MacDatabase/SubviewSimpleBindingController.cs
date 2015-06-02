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

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Load the default person from the database
			Person = new PersonModel (Conn, "0");
	
		}
		#endregion
	}
}
