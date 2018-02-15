using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;
using System.Collections.Generic;

namespace NSOutlineViewAndTableView
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
			mainWindowController = new MainWindowController ();

			// This is where we setup our visual tree. These could be setup in MainWindow.xib, but
			// this example is showing programmatic creation.

			// We create a tab control to insert both examples into, and set it to take the entire window and resize
			CGRect frame = mainWindowController.Window.ContentView.Frame;
			NSTabView tabView = new NSTabView (frame) {
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
			};

			NSTabViewItem firstTab = new NSTabViewItem () {
				View = OutlineSetup.SetupOutlineView (frame),
				Label = "NSOutlineView"
			};
			tabView.Add (firstTab);

			NSTabViewItem secondTab = new NSTabViewItem () {
				View = TableSetup.SetupTableView (frame),
				Label = "NSTableView"
			};
			tabView.Add (secondTab);

			mainWindowController.Window.ContentView.AddSubview (tabView);
			mainWindowController.Window.MakeKeyAndOrderFront (this);
		}
	}
}

