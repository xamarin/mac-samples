using System;
using CoreGraphics;
using Foundation;
using AppKit;

namespace Markdown
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public AppDelegate ()
		{
		}

		public override void DidFinishLaunching (NSNotification notification)
		{
		}

		public override bool ApplicationShouldOpenUntitledFile (NSApplication sender)
		{
			return false;
		}
	}
}