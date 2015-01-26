using System;
using AppKit;
using CoreGraphics;
using Foundation;

namespace NSOutlineViewAndTableViewExample
{
	static class OutlineSetup
	{
		// This sets up a NSOutlineView for demonstration
		internal static NSView SetupOutlineView (CGRect frame)
		{
			// Create our NSOutlineView and set it's frame to a reasonable size. It will be autosized via the NSClipView
			NSOutlineView outlineView = new NSOutlineView () {
				Frame = frame
			};

			// Every NSOutlineView must have at least one column or your Delegate will not be called.
			NSTableColumn column = new NSTableColumn ("Values");
			outlineView.AddColumn (column);
			// You must set OutlineTableColumn or the arrows showing children/expansion will not be drawn
			outlineView.OutlineTableColumn = column;

			// Setup the Delegate/DataSource instances to be interrogated for data and view information
			// In Unified, these take an interface instead of a base class and you can combine these into
			// one instance. 
			outlineView.Delegate = new OutlineViewDelegate ();
			outlineView.DataSource = new OutlineViewDataSource ();

			// NSOutlineView expects to be hosted inside an NSClipView and won't draw correctly otherwise  
			NSClipView clipView = new NSClipView (frame) {
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
			};
			clipView.DocumentView = outlineView;
			return clipView;
		}
	}

	// Delegates recieve events associated with user action and determine how an item should be visualized
	class OutlineViewDelegate : NSOutlineViewDelegate
	{
		const string identifer = "myCellIdentifier";
		public override NSView GetView (NSOutlineView outlineView, NSTableColumn tableColumn, NSObject item)
		{
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
			NSTextField view = (NSTextField)outlineView.MakeView (identifer, this);
			if (view == null) {
				view = new NSTextField () { 
					Identifier = identifer,
					Bordered = false,
					Selectable = false,
					Editable = false
				};
			}

			view.StringValue = ((Node)item).Name;
			return view;
		}

		// An example of responding to user input 
		public override bool ShouldSelectItem (NSOutlineView outlineView, NSObject item)
		{
			Console.WriteLine ("ShouldSelectItem: {0}", ((Node)item).Name );
			return true;
		}
	}

	// Data sources walk a given data source and respond to questions from AppKit to generate
	// the data used in your Delegate. In this example, we walk a simple tree.
	class OutlineViewDataSource : NSOutlineViewDataSource
	{
		Node parentNode;
		public OutlineViewDataSource ()
		{
			parentNode = Node.CreateExampleTree ();
		}
			
		public override nint GetChildrenCount (NSOutlineView outlineView, NSObject item)
		{
			// If item is null, we are referring to the root element in the tree
			item = item == null ? parentNode : item;
			return ((Node)item).ChildCount;
		}

		public override NSObject GetChild (NSOutlineView outlineView, nint childIndex, NSObject item)
		{
			// If item is null, we are referring to the root element in the tree
			item = item == null ? parentNode : item;
			return ((Node)item).GetChild ((int)childIndex);
		}

		public override bool ItemExpandable (NSOutlineView outlineView, NSObject item)
		{
			// If item is null, we are referring to the root element in the tree
			item = item == null ? parentNode : item;
			return !((Node)item).IsLeaf;
		}
	}
}

