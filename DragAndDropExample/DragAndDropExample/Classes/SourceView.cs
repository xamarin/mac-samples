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
			var layer = new CALayer {
				BackgroundColor = new CGColor (1f, 0f, 0f)
			};

			WantsLayer = true;
			Layer = layer;
		}
		#endregion

		#region Override Methods
		public override void ViewDidMoveToSuperview ()
		{
			// Display the Home icon
			AddSubview (new NSImageView (Frame) { Image = homeImage });
		}

		public override void MouseDragged (NSEvent theEvent)
		{
			// Create two items that are dragged: 1) a text string, 2) an image
			var text = new NSDraggingItem ((NSString)"Hello World");
			var images = new NSDraggingItem (homeImage);

			// Inform the OS that the drag has started and that it contains the two elements
			// that we created above
			BeginDraggingSession (new [] { text, images }, theEvent, this);
		}
		#endregion
	}
}

