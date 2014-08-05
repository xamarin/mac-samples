
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace KeyFrameMoveAView
{
	public partial class MainWindow : AppKit.NSWindow
	{
		public MainWindow (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindow (NSCoder coder) : base(coder) {}
	}
}

