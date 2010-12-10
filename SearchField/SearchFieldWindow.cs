
using System;
using MonoMac.AppKit;

namespace SearchField
{
	public partial class SearchFieldWindow : NSWindow
	{
		// called from unmanaged code
		public SearchFieldWindow (IntPtr handle) : base(handle)
		{
		}
	}
}

