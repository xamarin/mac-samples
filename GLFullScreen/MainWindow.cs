using System;
using Foundation;
using AppKit;

namespace GLFullScreen
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

