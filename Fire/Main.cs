using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace Fire
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

