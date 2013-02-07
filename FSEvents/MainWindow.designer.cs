// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace FSEventWatcher
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField LatencyTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSStepper LatencyStepper { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField WatchPathTextField { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton StartStopButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView EventStreamView { get; set; }

		[Action ("ShowStream:")]
		partial void ShowStream (MonoMac.Foundation.NSObject sender);

		[Action ("FlushStreamAsync:")]
		partial void FlushStreamAsync (MonoMac.Foundation.NSObject sender);

		[Action ("FlushStreamSync:")]
		partial void FlushStreamSync (MonoMac.Foundation.NSObject sender);

		[Action ("ChangeWatchPath:")]
		partial void ChangeWatchPath (MonoMac.Foundation.NSObject sender);

		[Action ("RevealWatchPathInFinder:")]
		partial void RevealWatchPathInFinder (MonoMac.Foundation.NSObject sender);

		[Action ("RevealWatchPathInTerminal:")]
		partial void RevealWatchPathInTerminal (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (LatencyTextField != null) {
				LatencyTextField.Dispose ();
				LatencyTextField = null;
			}

			if (LatencyStepper != null) {
				LatencyStepper.Dispose ();
				LatencyStepper = null;
			}

			if (WatchPathTextField != null) {
				WatchPathTextField.Dispose ();
				WatchPathTextField = null;
			}

			if (StartStopButton != null) {
				StartStopButton.Dispose ();
				StartStopButton = null;
			}

			if (EventStreamView != null) {
				EventStreamView.Dispose ();
				EventStreamView = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
