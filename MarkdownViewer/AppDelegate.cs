using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

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