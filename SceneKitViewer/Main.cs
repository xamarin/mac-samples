using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace SceneKitViewer
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		public AppDelegate ()
		{
		}
		
		public override void DidFinishLaunching (NSNotification notification)
		{
			// You can put any code here after your app launched.
		}
	
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}	

