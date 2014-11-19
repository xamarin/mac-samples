using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace AnimatedClock
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

