using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace Test_Hello_Mac
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

