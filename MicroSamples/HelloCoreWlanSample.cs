// HelloCoreWlanSample.cs
// Ashok Gelal
// Thanks to Duane Wandless

using System;
using System.Runtime.InteropServices;

using AppKit;
using CoreLocation;
using CoreWlan;
using Foundation;

namespace HelloCoreWlan {
	public class HelloCoreWlanSample {

		static void Main (string[] args)
		{
			NSApplication.Init ();
			
			using (new NSAutoreleasePool ()) {
				string[] interfaces = CWInterface.SupportedInterfaces;
				
				if (interfaces.Length < 1) {
					Console.WriteLine ("No supported interface is available in this computer");
					return;
				}
			
				CWInterface selectedIntface = CWInterface.FromName (interfaces[0]);
				// print interface information
				Console.WriteLine ($"\nInterface Information:\nName: {selectedIntface.Name}, Active SSID: {selectedIntface.Ssid}, Active BSSID: {selectedIntface.Bssid}\n");
				
				NSError error;
				CWNetwork[] data = selectedIntface.ScanForNetworksWithParameters (null, out error);
				
				if(error != null) {
					Console.Error.WriteLine ("An error occurred while scanning for available networks");
					return;
				}
			
				foreach (CWNetwork d in data) {
					Console.Write($"SSID: {d.Ssid}, BSSID: {d.Bssid}\n");
				}
			}
		}
	}
}

