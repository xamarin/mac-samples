using System;
using Foundation;
using System.Collections.Generic;

namespace NSOutlineViewAndTableViewExample
{
	// This is a little tree data structure to show off outline view display
	class Node : NSObject
	{
		public string Name { get; private set; }
		List<Node> Children;

		public static Node CreateExampleTree ()
		{
			Node parentNode = new Node ("");
			for (int i = 0 ; i < 5 ; ++i) {
				Node n = parentNode.AddChild ("Parent: " + i);
				for (int j = 0; j < 3 ; ++j)
					n.AddChild ("Child: " + j);
			}
			return parentNode;
		}

		public Node (string name)
		{
			Name = name;
			Children = new List<Node> ();
		}

		public Node AddChild (string name)
		{
			Node n = new Node (name);
			Children.Add (n);
			return n;
		}

		public Node GetChild (int index)
		{
			return Children [index];
		}

		public int ChildCount { get { return Children.Count; } }
		public bool IsLeaf { get { return ChildCount == 0; } }
	}
}

