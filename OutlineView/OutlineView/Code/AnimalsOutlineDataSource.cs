using System;
using System.Collections.Generic;
using MonoMac.AppKit;
using MonoMac.Foundation;

namespace OutlineView
{
	public class AnimalsOutlineDataSource : NSOutlineViewDataSource
	{
		// declarations
		protected Animal animalsTree;
		
		public AnimalsOutlineDataSource (Animal animalsTree) : base()
		{
			this.animalsTree = animalsTree;
		}
		
		public override int GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			Console.Write ("GetChildrenCount called on " + animalsTree.ToString() + ", ");
			
			// if null, it's asking about the root element
			if (item == null) {
				Console.WriteLine ("root. Returning " + animalsTree.Children.Count);
				return animalsTree.Children.Count;
			} else {
				// get the number of children from the element passed
				Animal passedNode = item as Animal;
				if (passedNode != null) {
					Console.WriteLine (passedNode.ToString() + ". returning " + passedNode.Children.Count);
					return passedNode.Children.Count;
				} else {
					Console.WriteLine ("could not cast, there is a problem here");
					return 0;
				}
			}
		}
		
		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
			Console.WriteLine ("ItemExpandable called on " + animalsTree.ToString());
			// get the number of children from the element passed
			if (item != null) {
				Animal passedNode = item as Animal;
				if (passedNode != null)
					return passedNode.HasChildren;
				else
					return false;
			} else {
				// if null, it's asking about the root element
				return animalsTree.HasChildren;
			}
		}
		
		public override NSObject GetObjectValue (NSOutlineView outlineView, NSTableColumn forTableColumn, NSObject byItem)
		{
			Console.Write ("GetObjectValue called on " + animalsTree.ToString () + ", ");
			
			// get the number of children from the element passed
			if (byItem == null) {
				Console.WriteLine ("passed null, returning " + animalsTree.Name);
				return (NSString)animalsTree.Name;
				//return new NSString();
			} else {
				Animal passedNode = byItem as Animal;
				if (passedNode != null) {
					Console.WriteLine ("returning " + passedNode.Name);
					return (NSString)passedNode.Name;
				} else {
					Console.WriteLine ("returning an empty string, cast failed.");
					return new NSString();
				}
			}
		}
		
		public override NSObject GetChild (NSOutlineView outlineView, int childIndex, NSObject ofItem)
		{
			Console.Write ("GetChild called on " + animalsTree.ToString () + ", ");
			// null means it's asking for the root
			if (ofItem == null) {
				Console.WriteLine ("asked for root, returning " + animalsTree.Children [childIndex].ToString ());
				return animalsTree.Children [childIndex];
			} else {
				Console.WriteLine ("asked for child, returning " + ((ofItem as Animal).Children [childIndex]).ToString() );
				return (NSObject)((ofItem as Animal).Children [childIndex]);
			}
		}
	}
}

