using System;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace DragAndDropExample
{
	public class DestView : NSView
	{
		#region Private Variables
		private NSTextView countText;
		private int dragCount = 0;
		#endregion

		#region Constructors
		public DestView (IntPtr handle) : base (handle)
		{
		}

		public DestView (CGRect r) : base (r)
		{

			// Set the background color of this view to Green
			CALayer layer = new CALayer ();
			layer.BackgroundColor = new CGColor (0f, 1f, 0f);
			WantsLayer = true;
			Layer = layer;
		}
		#endregion

		#region Private Methods
		void UpdateDragCount ()
		{
			// Display the current value of the counter
			countText.TextStorage.SetString (new NSAttributedString (dragCount.ToString ()));
			countText.Font = NSFont.SystemFontOfSize (16);
		}
		#endregion

		#region Override Methods
		public override void ViewDidMoveToSuperview ()
		{
			// Tell the system what types of files you allow to be dragged into this view
			// In this case, we only accept TIFF formatted images
			RegisterForDraggedTypes (new string[] { NSPasteboard.NSTiffType });

			// Create a text field to show the count of items that have been drug in
			// and add it to the view
			countText = new NSTextView (new CGRect (Frame.Width / 2 - 20, 0, 100, 40));
			countText.BackgroundColor = NSColor.Green;
			countText.Selectable = false;
			countText.Editable = false;
			AddSubview (countText);

			// Update the current count
			UpdateDragCount ();
		}

		public override NSDragOperation DraggingEntered (NSDraggingInfo sender)
		{
			// When we start dragging, inform the system that we will be handling this as
			// a copy/paste
			return NSDragOperation.Copy;
		}

		public override void DraggingEnded (NSDraggingInfo sender)
		{
			// Update the dropped item counter and display
			dragCount++;
			UpdateDragCount ();

			// Pull the individual items from the drag package and write them to the console
			NSImage i = new NSImage (sender.DraggingPasteboard.GetDataForType (NSPasteboard.NSTiffType));
			string s = NSString.FromData (sender.DraggingPasteboard.GetDataForType (NSPasteboard.NSStringType), NSStringEncoding.UTF8);
			Console.WriteLine ("String Data: " + s);
			Console.WriteLine ("Image Data: " + i.Size);
		}
		#endregion
	}
}

