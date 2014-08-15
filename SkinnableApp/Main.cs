using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace SkinnableApp
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

