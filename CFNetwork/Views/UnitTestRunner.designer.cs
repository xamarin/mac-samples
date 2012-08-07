// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	[Register ("UnitTestRunnerController")]
	partial class UnitTestRunnerController
	{
		[Outlet]
		MonoMac.AppKit.NSTextFieldCell Status { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView ResultArea { get; set; }

		[Action ("Run:")]
		partial void Run (MonoMac.Foundation.NSObject sender);

		[Action ("Stop:")]
		partial void Stop (MonoMac.Foundation.NSObject sender);

		[Action ("Close:")]
		partial void Close (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (Status != null) {
				Status.Dispose ();
				Status = null;
			}

			if (ResultArea != null) {
				ResultArea.Dispose ();
				ResultArea = null;
			}
		}
	}

	[Register ("UnitTestRunner")]
	partial class UnitTestRunner
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
