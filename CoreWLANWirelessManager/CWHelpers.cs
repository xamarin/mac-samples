using System;
using MonoMac.Foundation;
using MonoMac.CoreWlan;
using System.Collections.Generic;
using System.Linq;

namespace CoreWLANWirelessManager
{
	public static class CWHelpers
	{
		static CWPhyMode [] phyPriorityList = {
			CWPhyMode.N,
			CWPhyMode.G,
			CWPhyMode.B,
			CWPhyMode.AC,
			CWPhyMode.A
		};

		static CWSecurity [] securityPriorityList = {
			CWSecurity.WPA2Enterprise,
			CWSecurity.WPA2Personal,
			CWSecurity.WPAEnterpriseMixed,
			CWSecurity.WPAPersonalMixed,
			CWSecurity.WPAEnterprise,
			CWSecurity.WPAPersonal,
			CWSecurity.Enterprise,
			CWSecurity.DynamicWEP,								
			CWSecurity.WEP,
			CWSecurity.Personal,
		};

		public static CWPhyMode PHYModeFromSSID (CWNetwork[] scanResults, string ssid) {
			CWNetwork network = scanResults.First (x => x.Ssid == ssid);

			foreach (var e in phyPriorityList) {
				if (network.SupportsPhyMode (e)) {
					return e;
				}
			}

			return CWPhyMode.None;
		}

		public static CWSecurity SecurityModeFromSSID (CWNetwork[] scanResults, string ssid) {
			CWNetwork network = scanResults.First (x => x.Ssid == ssid);
				
			foreach (var e in securityPriorityList) {
				if (network.SupportsSecurity (e)) {
					return e;
				}
			}

			return CWSecurity.None;
		}

		public static string StringForPhyMode (CWPhyMode mode) {
			switch (mode) {
			case CWPhyMode.A:
				return "802.11a";
			case CWPhyMode.B:
				return "802.11b";
			case CWPhyMode.AC:
				return "802.11ac";
			case CWPhyMode.G:
				return "802.11g";
			case CWPhyMode.N:
				return "802.11n";
			default:
				return string.Empty;
			}
		}

		public static string StringForSecurityMode (CWSecurity mode) {
			switch (mode) {
			case CWSecurity.None:
				return "Open";
			case CWSecurity.WEP:
				return "WEP";
			case CWSecurity.WPAPersonal:
				return "WPA Personal";
			case CWSecurity.WPAEnterprise:
				return "WPA Enterprise";
			case CWSecurity.WPA2Personal:
				return "WPA2 Personal";
			case CWSecurity.WPA2Enterprise:
				return "WPA2 Enterprise";
			case CWSecurity.DynamicWEP:
				return "802.1X WEP";
			default:
				return string.Empty;
			}
		}

		public static string StringForOpMode (CWInterfaceMode mode) {
			switch (mode) {
			case CWInterfaceMode.HostAP:
				return "Host Access Point";
			case CWInterfaceMode.Ibss:
				return "IBSS";
			case CWInterfaceMode.None:
				return string.Empty;
			case CWInterfaceMode.Station:
				return "Infrastructure";
			default:
				return string.Empty;
			}		
		}

		public static CWSecurity SecurityModeForString (string modeStr) {
			switch (modeStr) {
			case "WEP":
				return CWSecurity.WEP;
			case "WPA Personal":
				return CWSecurity.WPAPersonal;
			case "WPA2 Personal":
				return CWSecurity.WPA2Personal;
			case "WPA Enterprise":
				return CWSecurity.WPAEnterprise;
			case "WPA2 Enterprise":
				return CWSecurity.WPA2Enterprise;
			case "802.1X WEP":
				return CWSecurity.DynamicWEP;
			}
			return CWSecurity.None;
		}
	}
}

