
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace PopupBindings
{
	public partial class TestWindow : AppKit.NSWindow
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

