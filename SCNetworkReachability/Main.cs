using System;

using MonoMac.AppKit;

namespace SCNetworkReachability
{
	class MainClass
	{
		static void Main (string [] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}