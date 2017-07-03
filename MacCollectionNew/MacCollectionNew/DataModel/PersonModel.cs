using System;
using Foundation;
using AppKit;

namespace MacCollectionNew
{
	/// <summary>
	/// The Person model holds all information about a given person that will be displayed in the
	/// collection view.
	/// </summary>
	[Register("PersonModel")]
	public class PersonModel : NSObject
	{
		#region Private Variables
		private string _name = "";
		private string _occupation = "";
		private bool _isManager = false;
		private NSMutableArray _people = new NSMutableArray();
		#endregion

		#region Computed Properties
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name of the person.</value>
		[Export("Name")]
		public string Name
		{
			get { return _name; }
			set
			{
				WillChangeValue("Name");
				_name = value;
				DidChangeValue("Name");
			}
		}

		/// <summary>
		/// Gets or sets the occupation.
		/// </summary>
		/// <value>The occupation of the person.</value>
		[Export("Occupation")]
		public string Occupation
		{
			get { return _occupation; }
			set
			{
				WillChangeValue("Occupation");
				_occupation = value;
				DidChangeValue("Occupation");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="T:MacCollectionNew.PersonModel"/> is manager.
		/// </summary>
		/// <value><c>true</c> if is manager; otherwise, <c>false</c>.</value>
		[Export("isManager")]
		public bool isManager
		{
			get { return _isManager; }
			set
			{
				WillChangeValue("isManager");
				WillChangeValue("Icon");
				_isManager = value;
				DidChangeValue("isManager");
				DidChangeValue("Icon");
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:MacCollectionNew.PersonModel"/> is employee.
		/// </summary>
		/// <value><c>true</c> if is employee; otherwise, <c>false</c>.</value>
		[Export("isEmployee")]
		public bool isEmployee
		{
			get { return (NumberOfEmployees == 0); }
		}

		/// <summary>
		/// Gets the icon that will be displayed for the person based on their status:
		/// manager or employee.
		/// </summary>
		/// <value>The icon.</value>
		[Export("Icon")]
		public NSImage Icon
		{
			get
			{
				if (isManager)
				{
					return NSImage.ImageNamed("IconGroup");
				}
				else
				{
					return NSImage.ImageNamed("IconUser");
				}
			}
		}

		/// <summary>
		/// Gets the people that this manger manages.
		/// </summary>
		/// <value>The people under this manager.</value>
		[Export("personModelArray")]
		public NSArray People
		{
			get { return _people; }
		}

		/// <summary>
		/// Gets the number of employees.
		/// </summary>
		/// <value>The number of employees that work for this manager.</value>
		[Export("NumberOfEmployees")]
		public nint NumberOfEmployees
		{
			get { return (nint)_people.Count; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.PersonModel"/> class.
		/// </summary>
		public PersonModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.PersonModel"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="occupation">Occupation.</param>
		public PersonModel(string name, string occupation)
		{
			// Initialize
			this.Name = name;
			this.Occupation = occupation;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MacCollectionNew.PersonModel"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="occupation">Occupation.</param>
		/// <param name="manager">If set to <c>true</c> manager.</param>
		public PersonModel(string name, string occupation, bool manager)
		{
			// Initialize
			this.Name = name;
			this.Occupation = occupation;
			this.isManager = manager;
		}
		#endregion

		#region Array Controller Methods
		/// <summary>
		/// Adds the person.
		/// </summary>
		/// <param name="person">The Person to add.</param>
		[Export("addObject:")]
		public void AddPerson(PersonModel person)
		{
			WillChangeValue("personModelArray");
			isManager = true;
			_people.Add(person);
			DidChangeValue("personModelArray");
		}

		/// <summary>
		/// Inserts the person.
		/// </summary>
		/// <param name="person">The Person to insert.</param>
		/// <param name="index">The Index to insert the person at.</param>
		[Export("insertObject:inPersonModelArrayAtIndex:")]
		public void InsertPerson(PersonModel person, nint index)
		{
			WillChangeValue("personModelArray");
			_people.Insert(person, index);
			DidChangeValue("personModelArray");
		}

		/// <summary>
		/// Removes the person.
		/// </summary>
		/// <param name="index">The Index of the person to remove.</param>
		[Export("removeObjectFromPersonModelArrayAtIndex:")]
		public void RemovePerson(nint index)
		{
			WillChangeValue("personModelArray");
			_people.RemoveObject(index);
			DidChangeValue("personModelArray");
		}

		/// <summary>
		/// Sets the people.
		/// </summary>
		/// <param name="array">An Array of people that this manager will manage.</param>
		[Export("setPersonModelArray:")]
		public void SetPeople(NSMutableArray array)
		{
			WillChangeValue("personModelArray");
			_people = array;
			DidChangeValue("personModelArray");
		}
		#endregion
	}
}

