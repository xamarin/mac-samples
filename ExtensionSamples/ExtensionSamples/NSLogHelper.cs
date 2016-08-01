using System;
using System.Runtime.InteropServices;
using Foundation;

namespace ExtensionSamples
{
	// In most extension contexts, System.Console.WriteLine is not useful, as it is not readable.
	// Invoking NSLog directly will allow the message to appear directly in the System Log found in
	// the "Console" application.
	public static class NSLogHelper
	{
		[DllImport ("/System/Library/Frameworks/Foundation.framework/Foundation")]
		extern static void NSLog (IntPtr format, [MarshalAs (UnmanagedType.LPStr)] string s);

		public static void NSLog (string format, params object[] args)
		{
			var fmt = NSString.CreateNative ("%s");
			var val = (args == null || args.Length == 0) ? format : string.Format (format, args);

			NSLog (fmt, val);
			NSString.ReleaseNative (fmt);
		}
	}
}