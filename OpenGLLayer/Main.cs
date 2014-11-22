using System;
using CoreGraphics;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace OpenGLLayer
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

