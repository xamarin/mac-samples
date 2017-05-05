using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacCollectionNew
{
	/// <summary>
	/// The Employee item controller handles the display of the individual items that will
	/// be displayed in the collection view as defined in the associated .XIB file.
	/// </summary>
	public partial class EmployeeItemController : NSCollectionViewItem
	{
		#region Private Variables
		/// <summary>
		/// The person that will be displayed.
		/// </summary>
		private PersonModel _person;
		#endregion

		#region Computed Properties
		// strongly typed view accessor
		public new EmployeeItem View
		{
			get
			{
				return (EmployeeItem)base.View;
			}
		}

		/// <summary>
		/// Gets or sets the person.
		/// </summary>
		/// <value>The person that this item belongs to.</value>
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
		/// Gets or sets the color of the background for the item.
		/// </summary>
		/// <value>The color of the background.</value>
		public NSColor BackgroundColor {
			get { return Background.FillColor; }
			set { Background.FillColor = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MacCollectionNew.EmployeeItemController"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
		/// <remarks>This also changes the background color based on the selected state
		/// of the item.</remarks>
		public override bool Selected
		{
			get
			{
				return base.Selected;
			}
			set
			{
				base.Selected = value;

				// Set background color based on the selection state
				if (value) {
					BackgroundColor = NSColor.DarkGray;
				} else {
					BackgroundColor = NSColor.LightGray;
				}
			}
		}
		#endregion

		#region Constructors
		// Called when created from unmanaged code
		public EmployeeItemController(IntPtr handle) : base(handle)
		{
			Initialize();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public EmployeeItemController(NSCoder coder) : base(coder)
		{
			Initialize();
		}

		// Call to load from the XIB/NIB file
		public EmployeeItemController() : base("EmployeeItem", NSBundle.MainBundle)
		{
			Initialize();
		}

		// Added to support loading from XIB/NIB
		public EmployeeItemController(string nibName, NSBundle nibBundle) : base(nibName, nibBundle) {

			Initialize();
		}

		// Shared initialization code
		void Initialize()
		{
		}
		#endregion
	}
}
