using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;

namespace MouseTrackingExample
{
	public partial class MyTrackingView : AppKit.NSView
	{
		#region Constructors

		// Called when created from unmanaged code
		public MyTrackingView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MyTrackingView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
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
			this.AddTrackingArea (trackingArea);
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
