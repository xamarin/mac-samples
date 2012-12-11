using System;
using System.Runtime;
using System.IO;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace TwoMinuteGrowler {
	class MainClass {
		static void Main ( string[] args ) {
			
			var baseAppPath = Directory.GetParent (Directory.GetParent (System.AppDomain.CurrentDomain.BaseDirectory).ToString ());
			//var growlPath = baseAppPath + "/Frameworks/Growl-WithInstaller.framework/Growl-WithInstaller";
			var growlPath = baseAppPath + "/Frameworks/Growl.framework/Growl";
			
			Dlfcn.dlopen (growlPath, 0);
			
			NSApplication.Init ();
			NSApplication.Main (args);
		}
	}
}

