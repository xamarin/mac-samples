using System;
using Foundation;
using System.Collections.Generic;

namespace MacOutlines
{
	public class Product : NSObject
	{
		#region Public Variables
		public List<Product> Products = new List<Product>();
		#endregion

		#region Computed Properties
		public string Title { get; set;} = "";
		public string Description { get; set;} = "";
		public bool IsProductGroup {
			get { return (Products.Count > 0); }
		}
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

