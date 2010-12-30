
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NSTableViewBinding
{
	public partial class TestWindow : MonoMac.AppKit.NSWindow
	{
		// Called when created from unmanaged code
		public TestWindow (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public TestWindow (NSCoder coder) : base(coder)
		{
		}
	}
}

