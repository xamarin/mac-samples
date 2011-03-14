
using System;
using MonoMac.Foundation;

namespace TwoMinuteGrowler
{
	public partial class MainWindow : MonoMac.AppKit.NSWindow
	{
		public MainWindow (IntPtr handle) : base(handle) {}

		[Export("initWithCoder:")]
		public MainWindow (NSCoder coder) : base(coder) {}
	}
}

