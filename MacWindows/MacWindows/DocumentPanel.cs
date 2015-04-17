using System;

using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class DocumentPanel : NSPanel
	{
		public DocumentPanel (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public DocumentPanel (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
	}
}
