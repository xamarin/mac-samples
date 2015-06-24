using System;

using AppKit;
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace DragAndDropExample
{
	public class SourceView : NSView, INSDraggingSource
	{
		#region Read-Only Variables
		readonly NSImage homeImage = NSImage.ImageNamed (NSImageName.HomeTemplate);
		#endregion

		#region Constructors
		public SourceView (IntPtr handle) : base (handle)
		{
		}

		public SourceView (CGRect r) : base (r)
		{
			// Set the background color of the view to Red
			CALayer layer = new CALayer ();
			layer.BackgroundColor = new CGColor (1f, 0f, 0f);
			WantsLayer = true;
			Layer = layer;
		}
		#endregion

		#region Override Methods
		public override void ViewDidMoveToSuperview ()
		{
			// Display the Home icon
			AddSubview (new NSImageView (this.Frame) { Image = homeImage });
		}

		public override void MouseDragged (NSEvent theEvent)
		{
			// Create two items that are dragged: 1) a text string, 2) an image
			NSDraggingItem text = new NSDraggingItem ((NSString)"Hello World");
			NSDraggingItem images = new NSDraggingItem (homeImage);

			// Inform the OS that the drag has started and that it contains the two elements
			// that we created above
			BeginDraggingSession (new NSDraggingItem[] { text, images }, theEvent, this);
		}
		#endregion
	}
}

