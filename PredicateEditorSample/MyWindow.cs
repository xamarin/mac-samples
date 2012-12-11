
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace PredicateEditorSample
{
	public partial class MyWindow : MonoMac.AppKit.NSWindow
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

