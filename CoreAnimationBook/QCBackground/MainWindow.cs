
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace QCBackground
{
	public partial class MainWindow : AppKit.NSWindow
	{
		// Called when created from unmanaged code
		public MainWindow (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindow (NSCoder coder) : base(coder)
		{
		}
	}
}

