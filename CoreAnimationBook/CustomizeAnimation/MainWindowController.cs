
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace CustomizeAnimation
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder) {}

		public MainWindowController () : base("MainWindow") {}
	}
}

