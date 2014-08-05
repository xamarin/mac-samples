using System;
using CoreWlan;

namespace CoreWLANWirelessManager
{
	public static class CWExtensions
	{
		public static string GetStringInterpretation (this CWPhyMode phyMode)
		{
			switch (phyMode) {
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

		public static string GetStringInterpretation (this CWSecurity securityMode)
		{
			switch (securityMode) {
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

		public static CWPhyMode GetSupportedPHYMode (this CWNetwork network)
		{
			foreach (var phyMode in (CWPhyMode[])Enum.GetValues(typeof(CWPhyMode))) {
				if (network.SupportsPhyMode (phyMode)) {
					return phyMode;
				}
			}

			return CWPhyMode.None;
		}

		public static CWSecurity GetSecurityMode (this CWNetwork network)
		{
			foreach (var securityMode in (CWSecurity[])Enum.GetValues(typeof(CWSecurity))) {
				if (network.SupportsSecurity (securityMode)) {
					return securityMode;
				}
			}

			return CWSecurity.None;
		}
	}
}

