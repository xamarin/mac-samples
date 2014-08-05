// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace FSEventWatcher
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSTextField LatencyTextField { get; set; }

		[Outlet]
		AppKit.NSStepper LatencyStepper { get; set; }

		[Outlet]
		AppKit.NSTextField WatchPathTextField { get; set; }

		[Outlet]
		AppKit.NSButton StartStopButton { get; set; }

		[Outlet]
		AppKit.NSTableView EventStreamView { get; set; }

		[Action ("ShowStream:")]
		partial void ShowStream (Foundation.NSObject sender);

		[Action ("FlushStreamAsync:")]
		partial void FlushStreamAsync (Foundation.NSObject sender);

		[Action ("FlushStreamSync:")]
		partial void FlushStreamSync (Foundation.NSObject sender);

		[Action ("ChangeWatchPath:")]
		partial void ChangeWatchPath (Foundation.NSObject sender);

		[Action ("RevealWatchPathInFinder:")]
		partial void RevealWatchPathInFinder (Foundation.NSObject sender);

		[Action ("RevealWatchPathInTerminal:")]
		partial void RevealWatchPathInTerminal (Foundation.NSObject sender);
		
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
