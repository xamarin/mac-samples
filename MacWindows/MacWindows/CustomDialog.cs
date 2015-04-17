using System;

using Foundation;
using AppKit;

namespace MacWindows
{
	public partial class CustomDialog : NSWindow
	{
		public CustomDialog (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public CustomDialog (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

		}

		#region Button Handlers
		[Export ("dialogClose:")]
		void DialogClose (NSObject sender) {

			NSApplication.SharedApplication.StopModal ();
			this.Close ();
		}

		[Export ("dialogCancel:")]
		void DialogCancel (NSObject sender) {

			NSApplication.SharedApplication.AbortModal ();
			this.Close ();
		}
		#endregion
	}
}
