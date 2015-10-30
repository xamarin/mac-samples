using System;

using AppKit;
using Foundation;

namespace ProgressBarExample
{
	public partial class MainWindowController : NSWindowController
	{
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

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
	}
}
