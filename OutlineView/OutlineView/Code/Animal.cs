using System;
using System.Collections.Generic;
using Foundation;

namespace OutlineView
{
	public class Animal : NSObject
	{
		public string Name { get; set; }

		public Animal () { this.Name = String.Empty; }

		public IList<Animal> Children
		{ 
			get { return this.children; }
			set { this.children = value; }
		}
		protected IList<Animal> children = new List<Animal>();

		public bool HasChildren
		{
			get { return (this.children.Count > 0); }
		}

		public override string ToString ()
		{
			return this.Name.ToString ();
		}
	}
}

