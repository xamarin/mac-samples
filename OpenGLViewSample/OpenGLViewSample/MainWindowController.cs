using System;
using System.Linq;
using Foundation;
using AppKit;

namespace OpenGLViewSample
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
		}


		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}
	}
}

