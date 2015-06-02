using System;
using SQLite;

namespace MacDatabase
{
	public class OccupationModel
	{
		#region Computed Properties
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Name { get; set;}
		public string Description { get; set;}
		#endregion

		#region Constructors
		public OccupationModel ()
		{
		}

		public OccupationModel (string name, string description)
		{

			// Initialize
			this.Name = name;
			this.Description = description;

		}
		#endregion
	}
}

