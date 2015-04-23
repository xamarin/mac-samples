using System;

using Foundation;
using AppKit;

namespace MacOutlines
{
	public partial class RotationWindowController : NSWindowController
	{
		public RotationWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public RotationWindowController (NSCoder coder) : base (coder)
		{
		}

		public RotationWindowController () : base ("RotationWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public new RotationWindow Window {
			get { return (RotationWindow)base.Window; }
		}
	}
}
