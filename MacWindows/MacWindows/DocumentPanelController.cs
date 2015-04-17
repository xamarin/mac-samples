using System;

using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class DocumentPanelController : NSWindowController
	{
		public DocumentPanelController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public DocumentPanelController (NSCoder coder) : base (coder)
		{
		}

		public DocumentPanelController () : base ("DocumentPanel")
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		public new DocumentPanel Window {
			get { return (DocumentPanel)base.Window; }
		}
	}
}
