// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("BenchmarkViewController")]
	partial class BenchmarkViewController
	{
		[Outlet]
		AppKit.NSTextView Results { get; set; }

		[Action ("Clear:")]
		partial void Clear (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Results != null) {
				Results.Dispose ();
				Results = null;
			}
		}
	}

	[Register ("BenchmarkView")]
	partial class BenchmarkView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
