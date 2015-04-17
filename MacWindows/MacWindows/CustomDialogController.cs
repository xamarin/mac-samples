using System;

using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class CustomDialogController : NSWindowController
	{
		public CustomDialogController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CustomDialogController (NSCoder coder) : base (coder)
		{
		}

		public CustomDialogController () : base ("CustomDialog")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public new CustomDialog Window {
			get { return (CustomDialog)base.Window; }
		}
	}
}
