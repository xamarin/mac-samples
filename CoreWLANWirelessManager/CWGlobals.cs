using System;
using MonoMac.Foundation;
namespace CoreWLANWirelessManager
{
	// Copyright Ashok Gelal (http://ashokgelal.com)
	
	public enum CWInterfaceState
	{
		CWInterfaceStateInactive = 0,
		CWInterfaceStateScanning,
		CWInterfaceStateAuthenticating,
		CWInterfaceStateAssociating,
		CWInterfaceStateRunning
	}
	
	public enum CWSecurityMode
	{
		CWSecurityModeOpen	= 0,
		CWSecurityModeWEP,
		CWSecurityModeWPA_PSK,
		CWSecurityModeWPA2_PSK,
		CWSecurityModeWPA_Enterprise,
		CWSecurityModeWPA2_Enterprise,
		CWSecurityModeWPS,
		CWSecurityModeDynamicWEP
	}
	
	public enum CWOpMode
	{
		CWOpModeStation	= 0,
		CWOpModeIBSS,
		CWOpModeMonitorMode,
		CWOpModeHostAP
	}
	
	public enum CWPHYMode
	{
		CWPHYMode11A	= 0,
		CWPHYMode11B,
		CWPHYMode11G,
		CWPHYMode11N
	}
	
	public static class CWConstants
	{
		public static readonly NSString CWModeDidChangeNotification = new NSString("MODE_CHANGED_NOTIFICATION");
		public static readonly NSString CWSSIDDidChangeNotification = new NSString("SSID_CHANGED_NOTIFICATION");
		public static readonly NSString CWBSSIDDidChangeNotification = new NSString("BSSID_CHANGED_NOTIFICATION");
		public static readonly NSString CWCountryCodeDidChangeNotification = new NSString("COUNTRY_CODE_CHANGED_NOTIFICATION");
		public static readonly NSString CWLinkDidChangeNotification = new NSString("LINK_CHANGED_NOTIFICATION");
		public static readonly NSString CWPowerDidChangeNotification = new NSString("POWER_CHANGED_NOTIFICATION");
		public static readonly NSString CWAssocKeyPassPhrase = new NSString("ASSOC_KEY_PASSPHRASE"); 
		public static readonly NSString CWAssocKey8021XProfile = new NSString("ASSOC_KEY_8021X_PROFILE");
		
		public static readonly NSString CWIBSSKeySSID = new NSString("IBSS_KEY_SSID");
		public static readonly NSString CWIBSSKeyChannel = new NSString("IBSS_KEY_CHANNEL");
		public static readonly NSString CWIBSSKeyPassphrase = new NSString("IBSS_KEY_PASSPHRASE");
	}
}
