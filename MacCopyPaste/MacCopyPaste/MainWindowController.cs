using System;

using Foundation;
using AppKit;

namespace MacCopyPaste
{
	public partial class MainWindowController : NSWindowController
	{
		#region Constructors
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}
		#endregion

		#region Computed Properties
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		#endregion
	}
}
