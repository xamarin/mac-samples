using System;

using AppKit;

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