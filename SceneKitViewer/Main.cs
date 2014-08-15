using System;
using System.Drawing;
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
		
		public override void FinishedLaunching (NSObject notification)
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

