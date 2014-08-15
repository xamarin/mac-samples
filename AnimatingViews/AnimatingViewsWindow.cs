
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace AnimatingViews
{
	public partial class AnimatingViewsWindow : AppKit.NSWindow
	{
		// Called when created from unmanaged code
		public AnimatingViewsWindow (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public AnimatingViewsWindow (NSCoder coder) : base(coder)
		{
		}
	}
}

