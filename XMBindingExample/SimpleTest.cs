using System;

using AppKit;
using Simple;
using System.Reflection;
using System.IO;

namespace SimpleTest
{
	static class MainClass
	{
		// http://stackoverflow.com/questions/52797/how-do-i-get-the-path-of-the-assembly-the-code-is-in
		public static string GetCurrentExecutingDirectory()
		{
			string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
			return Path.GetDirectoryName(filePath);
		}

		static void Main (string[] args)
		{
			var v = ObjCRuntime.Dlfcn.dlopen (GetCurrentExecutingDirectory () + "/SimpleClass.dylib", 0);

			NSApplication.Init ();
			SimpleClass c = new SimpleClass ();
			Console.WriteLine (c.DoIt());
		}
	}
}
