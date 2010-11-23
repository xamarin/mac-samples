
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NSTableViewBinding
{
	public partial class TableEdit : MonoMac.AppKit.NSWindow
	{
		// Called when created from unmanaged code
		public TableEdit (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public TableEdit (NSCoder coder) : base(coder)
		{
		}
	}
}

