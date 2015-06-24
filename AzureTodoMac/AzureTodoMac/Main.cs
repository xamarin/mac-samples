using System;

using AppKit;

namespace AzureTodo
{
	static class MainClass
	{
		static void Main (string[] args)
		{
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}
