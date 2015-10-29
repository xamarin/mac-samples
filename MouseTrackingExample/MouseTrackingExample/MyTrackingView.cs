using System;

using AppKit;
using CoreGraphics;
using Foundation;

namespace MouseTrackingExample
{
	public partial class MyTrackingView : NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public MyTrackingView (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MyTrackingView (NSCoder coder) : base (coder)
		{
		}

		#endregion

		NSTrackingArea trackingArea;
		public event EventHandler<TrackingEventArgs> OnDragChange = delegate {};

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			WantsLayer = true;
			Layer.BackgroundColor = new CGColor (1, 0, 0);
			trackingArea = new NSTrackingArea (Frame, NSTrackingAreaOptions.ActiveInKeyWindow | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.MouseMoved
				, this, null);
			AddTrackingArea (trackingArea);
		}

		public override void MouseEntered (NSEvent theEvent)
		{
			base.MouseEntered (theEvent);
			OnDragChange (this, new TrackingEventArgs ("Mouse Entered"));
		}

		public override void MouseExited (NSEvent theEvent)
		{
			base.MouseExited (theEvent);
			OnDragChange (this, new TrackingEventArgs ("Mouse Exited"));
		}

		public override void MouseMoved (NSEvent theEvent)
		{
			base.MouseMoved (theEvent);
			OnDragChange (this, new TrackingEventArgs ("Mouse Moved"));
		}
	}

	public class TrackingEventArgs : EventArgs
	{
		public TrackingEventArgs (string description) { Description = description; }

		public string Description { get; set; }
	}
}
