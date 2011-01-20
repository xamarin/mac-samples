// HelloCoreWlanSample.cs
// Ashok Gelal
// Thanks to Duane Wandless

using System;
using MonoMac.CoreLocation;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreWlan;
using System.Runtime.InteropServices;
namespace HelloCoreWlan
{
	public class HelloCoreWlanSample
	{
		static void Main (string[] args)
		{
			NSApplication.Init();
			
			using(new NSAutoreleasePool())
			{
				string[] interfaces = CWInterface.SupportedInterfaces;
				
				if(interfaces.Length<1){
					Console.WriteLine("No supported interface is available in this computer");
					return;
				}
			
				CWInterface selectedIntface = CWInterface.FromName (interfaces[0]);
				// print interface information
				Console.WriteLine("\nInterface Information:\nName: {0}, Active SSID: {1}, Active BSSID: {2}\n", selectedIntface.Name, selectedIntface.Ssid, selectedIntface.Bssid);
				
				NSError error;
				CWNetwork[] data = selectedIntface.ScanForNetworksWithParameters(null, out error);
				
				if(error!=null){
					Console.Error.WriteLine("An error occurred while scanning for available networks");
					return;
				}
			
				foreach(CWNetwork d in data)
				{
					Console.Write("SSID: {0}, BSSID: {1}\n", d.Ssid, d.Bssid);
				}
			}
		}
	}
}

