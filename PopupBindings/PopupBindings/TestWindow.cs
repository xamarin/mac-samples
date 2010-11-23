
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PopupBindings
{
	public partial class TestWindow : MonoMac.AppKit.NSWindow
	{
		public TestWindow (IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public TestWindow (NSCoder coder) : base(coder)
		{
		}
	}
}

