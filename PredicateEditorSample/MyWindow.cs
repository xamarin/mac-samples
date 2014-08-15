
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace PredicateEditorSample
{
	public partial class MyWindow : AppKit.NSWindow
	{
		public MyWindow (IntPtr handle) : base(handle)
		{

		}

		[Export("initWithCoder:")]
		public MyWindow (NSCoder coder) : base(coder)
		{
		}
	}
}

