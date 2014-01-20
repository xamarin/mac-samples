using System;
using MonoMac.Foundation;

namespace CoreWLANWirelessManager
{
	// Copyright Ashok Gelal (http://ashokgelal.com)
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
	}
}
