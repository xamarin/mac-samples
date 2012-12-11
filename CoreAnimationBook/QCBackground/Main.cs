using System;
using MonoMac.AppKit;

namespace QCBackground
{
	class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}

