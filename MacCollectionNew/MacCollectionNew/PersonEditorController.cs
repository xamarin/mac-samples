using System;

using Foundation;
using AppKit;

namespace MacCollectionNew
{
	/// <summary>
	/// The Person editor controller handles the editor interface for a new
	/// or existing employee. It is also used to show non-editable information
	/// for a given employee (PersonModel).
	/// </summary>
	public partial class PersonEditorController : NSViewController
	{
		#region Private Variables
		private PersonModel _person;
		private bool _canEdit = true;
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the presentor that displayed the editor as a sheet.
		/// </summary>
		/// <value>The NSViewController that presented the editor.</value>
		/// <remarks>This value is used to programmatically close the sheet.</remarks>
		public NSViewController Presentor { get; set; }

		/// <summary>
		/// Gets or sets the person that is being displayed/edited.
		/// </summary>
		/// <value>The person to display/edit.</value>
		[Export("Person")]
		public PersonModel Person
		{
			get { return _person; }
			set
			{
				WillChangeValue("Person");
				_person = value;
				DidChangeValue("Person");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MacCollectionNew.PersonEditorController"/> can
		/// be edited.
		/// </summary>
		/// <value><c>true</c> if can edit; otherwise, <c>false</c>.</value>
		[Export("CanEdit")]
		public bool CanEdit {
			get { return _canEdit; }
			set {
				WillChangeValue("CanEdit");
				_canEdit = value;
				DidChangeValue("CanEdit");
			}
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.PersonEditorController"/> class.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public PersonEditorController(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Closes the sheet.
		/// </summary>
		private void CloseSheet()
		{
			Presentor.DismissViewController(this);
		}
		#endregion

		#region Custom Actions
		/// <summary>
		/// Handles the user clicking the OK button to close the sheet.
		/// </summary>
		/// <param name="sender">Sender.</param>
		partial void ClickedOK(Foundation.NSObject sender) {
			CloseSheet();
		}
		#endregion
	}
}
