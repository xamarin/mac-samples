
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace CustomizeAnimation
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder) {}

		public MainWindowController () : base("MainWindow") {}
	}
}

