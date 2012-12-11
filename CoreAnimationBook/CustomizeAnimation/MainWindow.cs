
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace CustomizeAnimation
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		public MainWindow (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindow (NSCoder coder) : base(coder) {}
	}
}

