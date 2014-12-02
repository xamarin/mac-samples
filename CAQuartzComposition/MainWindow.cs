
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace CAQuartzComposition
{
	public partial class MainWindow : AppKit.NSWindow
	{
		public MainWindow (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindow (NSCoder coder) : base(coder) {}
	}
}

