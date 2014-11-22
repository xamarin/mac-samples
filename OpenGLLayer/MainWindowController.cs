
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace OpenGLLayer
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder)
		{
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base("MainWindow")
		{
		}
	}
}

