using System;

namespace MacTables
{
	public class Product
	{
		#region Computed Propoperties
		public string Title { get; set;} = "";
		public string Description { get; set;} = "";
		#endregion

		#region Constructors
		public Product ()
		{
		}

		public Product (string title, string description)
		{
			this.Title = title;
			this.Description = description;
		}
		#endregion
	}
}

