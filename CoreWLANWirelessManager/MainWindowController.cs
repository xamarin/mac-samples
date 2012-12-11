#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreWlan;
using System.Text;
using MonoMac.ObjCRuntime;

#endregion

namespace CoreWLANWirelessManager
{
	// Copyright Ashok Gelal (http://ashokgelal.com)
	
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Properties
		
		private CWInterface CurrentInterface { get; set; }
		private CWNetwork SelectedNetwork { get; set; }

		private CWNetwork[] ScanResults { get; set; }

		private bool JoinDialogContext { get; set; }
		
		//strongly typed window accessor
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
		
		#endregion
		
		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public MainWindowController () : base("MainWindow")
		{
			Initialize ();
		}

		// Shared initialization code
		public void Initialize ()
		{
			ScanResults = new CWNetwork[0];
		}

		#endregion
		
		#region Utility Methods
		
		private static string StringForPhyMode (NSNumber mode)
		{
			if (mode == null)
				return String.Empty;
			
			string phyModeStr = String.Empty;
			switch ((CWPHYMode)mode.IntValue) {
			case CWPHYMode.CWPHYMode11A:
				
				{
					phyModeStr = "802.11a";
					break;
				}

			case CWPHYMode.CWPHYMode11B:
				
				{
					phyModeStr = "802.11b";
					break;
				}

			case CWPHYMode.CWPHYMode11G:
				
				{
					phyModeStr = "802.11g";
					break;
				}

			case CWPHYMode.CWPHYMode11N:
				
				{
					phyModeStr = "802.11n";
					break;
				}

				
			}
			return phyModeStr;
		}

		private static string StringForSecurityMode (NSNumber mode)
		{
			if (mode == null)
				return String.Empty;
			
			string securityModeStr = String.Empty;
			switch ((CWSecurityMode)mode.IntValue) {
			case CWSecurityMode.CWSecurityModeOpen:
				
				{
					securityModeStr = "Open";
					break;
				}

			case CWSecurityMode.CWSecurityModeWEP:
				
				{
					securityModeStr = "WEP";
					break;
				}

			case CWSecurityMode.CWSecurityModeWPA_PSK:
				
				{
					securityModeStr = "WPA Personal";
					break;
				}

			case CWSecurityMode.CWSecurityModeWPA_Enterprise:
				
				{
					securityModeStr = "WPA Enterprise";
					break;
				}

			case CWSecurityMode.CWSecurityModeWPA2_PSK:
				
				{
					securityModeStr = "WPA2 Personal";
					break;
				}

			case CWSecurityMode.CWSecurityModeWPA2_Enterprise:
				
				{
					securityModeStr = "WPA2 Enterprise";
					break;
				}

			case CWSecurityMode.CWSecurityModeWPS:
				
				{
					securityModeStr = "WiFI Protected Setup";
					break;
				}

			case CWSecurityMode.CWSecurityModeDynamicWEP:
				
				{
					securityModeStr = "802.1X WEP";
					break;
				}

			}
			return securityModeStr;
		}

		private static string StringForOpMode (NSNumber mode)
		{
			if (mode == null)
				return String.Empty;
			
			string opModeStr = String.Empty;
			switch ((CWOpMode)mode.IntValue) {
			case CWOpMode.CWOpModeIBSS:
				
				{
					opModeStr = "IBSS";
					break;
				}

			case CWOpMode.CWOpModeStation:
				
				{
					opModeStr = "Infrastructure";
					break;
				}

			case CWOpMode.CWOpModeHostAP:
				
				{
					opModeStr = "Host Access Point";
					break;
				}

			case CWOpMode.CWOpModeMonitorMode:
				
				{
					opModeStr = "Monitor Mode";
					break;
				}

			}
			return opModeStr;
		}

		private static CWSecurityMode SecurityModeForString (string modeStr)
		{
			switch (modeStr) {
			case "WEP":
				return CWSecurityMode.CWSecurityModeWEP;
			case "WPA Personal":
				return CWSecurityMode.CWSecurityModeWPA_PSK;
			case "WPA2 Personal":
				return CWSecurityMode.CWSecurityModeWPA2_PSK;
			case "WPA Enterprise":
				return CWSecurityMode.CWSecurityModeWPA_Enterprise;
			case "WPA2 Enterprise":
				return CWSecurityMode.CWSecurityModeWPA2_Enterprise;
			case "802.1X WEP":
				return CWSecurityMode.CWSecurityModeDynamicWEP;
			}
			return CWSecurityMode.CWSecurityModeOpen;
			
		}
		
		#endregion
		
		#region Methods
		
		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();
			supportedInterfacesPopup.SelectItem (0);
			interfaceSelected(null);
		}
		
		public override void AwakeFromNib ()
		{
			// populate interfaces popup with all supported interfaces
			supportedInterfacesPopup.RemoveAllItems ();
			supportedInterfacesPopup.AddItems (CWInterface.SupportedInterfaces);
			
			// setup scan results table
			scanResultsTable.DataSource = new ScanResultsTableDataSource (this);
			
			// hide progress indicators
			refreshSpinner.Hidden = true;
			joinSpinner.Hidden = true;
			ibssSpinner.Hidden = true;
			
			NSNotificationCenter.DefaultCenter.AddObserver(CWConstants.CWBSSIDDidChangeNotification, HandleNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(CWConstants.CWCountryCodeDidChangeNotification, HandleNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(CWConstants.CWLinkDidChangeNotification, HandleNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(CWConstants.CWModeDidChangeNotification, HandleNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(CWConstants.CWPowerDidChangeNotification, HandleNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(CWConstants.CWSSIDDidChangeNotification, HandleNotification);
		}
		
		private void UpdateInterfaceInfoTab ()
		{
			bool powerState = CurrentInterface.Power;
			powerStateControl.SetSelected (true, powerState ? 0 : 1);
			bool isRunning = (CWInterfaceState)CurrentInterface.InterfaceState.Int32Value == CWInterfaceState.CWInterfaceStateRunning;
			
			if (isRunning)
				disconnectButton.Enabled = true;
			else
				disconnectButton.Enabled = false;
			
			opModeField.StringValue = powerState ? StringForOpMode (CurrentInterface.OpMode) : String.Empty;
			
			securityModeField.StringValue = powerState ? StringForSecurityMode (CurrentInterface.SecurityMode) : String.Empty;
			
			phyModeField.StringValue = powerState ? StringForPhyMode (CurrentInterface.PhyMode) : String.Empty;
			
			string str = CurrentInterface.Ssid;
			ssidField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			str = CurrentInterface.Bssid;
			bssidField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			NSNumber num = CurrentInterface.TxRate;
			txRateField.StringValue = (powerState && num!=null) ? String.Format ("{0} Mbps", num) : String.Empty;
			
			num = CurrentInterface.Rssi;
			rssiField.StringValue = (powerState && num!=null)? String.Format ("{0} dBm", num) : String.Empty;
			
			num = CurrentInterface.Noise;
			noiseField.StringValue = (powerState && num!=null)? String.Format ("{0} dBm", num) : String.Empty;
			
			num = CurrentInterface.TxPower;
			txPowerField.StringValue = (powerState && num!=null)? String.Format ("{0} mW", num) : String.Empty;
			
			str = CurrentInterface.CountryCode;
			countryCodeField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			NSNumber[] supportedChannelsArray = CurrentInterface.SupportedChannels;
			StringBuilder tempString = new StringBuilder ();
			channelPopup.RemoveAllItems ();
			
			foreach (NSNumber eachChannel in supportedChannelsArray)
			{
				if (eachChannel.IsEqualToNumber (supportedChannelsArray.Last ()))
					tempString.Append (eachChannel.ToString ());
				else
					tempString.AppendFormat ("{0}, ", eachChannel.ToString ());
				if (powerState)
					channelPopup.AddItem (eachChannel.ToString ());
			}
			
			supportedChannelsField.StringValue = tempString.ToString ();
			
			NSNumber[] supportedPhyModesArray = CurrentInterface.SupportedPhyModes;
			tempString = new StringBuilder ("802.11");
			
			foreach (NSNumber eachPhyMode in supportedPhyModesArray) {
				switch ((CWPHYMode)eachPhyMode.Int32Value)
				{
					case CWPHYMode.CWPHYMode11A:
						tempString.Append("a/");
						break;
					case CWPHYMode.CWPHYMode11B:
						tempString.Append("b/");
						break;
					case CWPHYMode.CWPHYMode11G:
						tempString.Append("g/");
						break;
					case CWPHYMode.CWPHYMode11N:
						tempString.Append("n");
						break;
				}
			}
			
			string phymode = tempString.ToString();
			if(phymode.EndsWith("/"))
				phymode.Remove(phymode.Length-1);
			if(phymode.Equals("802.11"))
				phymode = "None";
				
			supportedPHYModes.StringValue = phymode;
			
			channelPopup.SelectItem(CurrentInterface.Channel!=null?CurrentInterface.Channel.StringValue:String.Empty);
			if(!powerState || isRunning)
				channelPopup.Enabled = false;
			else
				channelPopup.Enabled = true;
		}
		
		private void UpdateScanTab ()
		{
			NSDictionary param = NSDictionary.FromObjectAndKey (NSNumber.FromBoolean (mergeScanResultsCheckbox.State == NSCellStateValue.On), new NSString ("true"));
			NSError error = null;
			ScanResults = CurrentInterface.ScanForNetworksWithParameters (param, out error);
			if (error == null)
			{
				Array.Sort (ScanResults, delegate(CWNetwork networkA, CWNetwork networkB)
					{
						return networkA.Ssid.CompareTo(networkB.Ssid);	
				});
			}
			else
			{
				ScanResults = new CWNetwork[0];
				NSAlert.WithError(error).RunModal();
			}
			
			scanResultsTable.ReloadData();
		}
		
		private void HandleNotification (NSNotification note)
		{
			UpdateInterfaceInfoTab();
		}
		
		private void ResetDialog ()
		{
			joinNetworkNameField.StringValue = String.Empty;
			joinNetworkNameField.Enabled = true;
			
			joinUsernameField.StringValue = String.Empty;
			joinUsernameField.Enabled = true;
			
			joinPassphraseField.StringValue = String.Empty;
			joinPassphraseField.Enabled = true;
			
			joinSecurityPopupButton.RemoveAllItems ();
			joinSecurityPopupButton.AddItems (new[] {
												"Open", "WEP", "WEP Personal", "WPA2 Personal",
												"WPA Enterprise", "WPA2 Enterprise", "802.11X WEP"
											});
			
			joinSecurityPopupButton.SelectItem (0);
			joinSecurityPopupButton.Enabled = true;
			
			joinUser8021XProfilePopupButton.RemoveAllItems ();
			changeSecurityMode (null);
		}
		
		#endregion
		
		#region IBAction Methods
		
		partial void interfaceSelected (NSObject sender)
		{
			CurrentInterface = CWInterface.FromName (supportedInterfacesPopup.SelectedItem.Title);
			UpdateInterfaceInfoTab ();
		}
		
		partial void refreshPressed (NSObject sender)
		{
			refreshSpinner.Hidden = false;
			refreshSpinner.StartAnimation(this);
			
			if (tabView.Selected.Label.Equals ("Interface Info"))
				UpdateInterfaceInfoTab ();
			else if (tabView.Selected.Label.Equals ("Scan"))
				UpdateScanTab ();
			
			refreshSpinner.StopAnimation (this);
			refreshSpinner.Hidden = true;
		}
		
		partial void changePower (NSObject sender)
		{
			NSError error = null;
			bool result = CurrentInterface.SetPower (powerStateControl.SelectedSegment == 0, out error);
			if (!result)
				NSAlert.WithError (error).RunModal ();
					
			UpdateInterfaceInfoTab();
		}
		
		partial void changeChannel (NSObject sender)
		{
			NSError error = null;
			bool result = CurrentInterface.SetChannel(Convert.ToUInt32(channelPopup.SelectedItem.Title), out error);
			if (!result)
				NSAlert.WithError (error).RunModal ();
					
			UpdateInterfaceInfoTab();
		}
		
		partial void disconnect (NSObject sender)
		{
			CurrentInterface.Disassociate ();
			UpdateInterfaceInfoTab();
		}
		
		partial void changeSecurityMode (NSObject sender)
		{
			if (
					String.Equals (joinSecurityPopupButton.SelectedItem.Title, "WPA Enterprise") ||
					String.Equals (joinSecurityPopupButton.SelectedItem.Title, "WPA2 Enterprise") ||
					String.Equals (joinSecurityPopupButton.SelectedItem.Title, "802.1X WEP")
				)
			{
				joinUsernameField.Enabled = true;
				joinUser8021XProfilePopupButton.Enabled = true;
				joinPassphraseField.Enabled = true;
				
				joinUser8021XProfilePopupButton.AddItem ("Default");
				foreach (var item in CW8021XProfile.AllUser8021XProfiles)
					joinUser8021XProfilePopupButton.AddItem (item.UserDefinedName);
				
				joinUser8021XProfilePopupButton.SelectItem (0);
			}
			
			else if (
					String.Equals (joinSecurityPopupButton.SelectedItem.Title, "WPA Personal") ||
					String.Equals (joinSecurityPopupButton.SelectedItem.Title, "WPA2 Personal") ||
					String.Equals (joinSecurityPopupButton.SelectedItem.Title, "WEP")
			)
			{
				joinUser8021XProfilePopupButton.RemoveAllItems ();
				joinUser8021XProfilePopupButton.Enabled = false;
				joinUsernameField.Enabled = false;
				joinPassphraseField.Enabled = true;
			}
			else 
			{
				joinUser8021XProfilePopupButton.RemoveAllItems ();
				joinUser8021XProfilePopupButton.Enabled = false;
				joinUsernameField.Enabled = false;
				joinPassphraseField.Enabled = false;
			}
		}
		
		partial void change8021XProfile (NSObject sender)
		{
			var index = joinUser8021XProfilePopupButton.IndexOfSelectedItem;
			if (index >= 0)
			{
				CW8021XProfile tmp = CW8021XProfile.AllUser8021XProfiles[index];
				if (tmp != null)
				{
					if (String.Equals (joinUser8021XProfilePopupButton.SelectedItem.Title, "Default"))
					{
						joinUsernameField.StringValue = String.Empty;
						joinUsernameField.Enabled = true;
						joinPassphraseField.StringValue = String.Empty;
						joinPassphraseField.Enabled = true;
					}
					else
					{
						joinUsernameField.StringValue = tmp.Username;
						joinUsernameField.Enabled = false;
						joinPassphraseField.StringValue = tmp.Password;
						joinPassphraseField.Enabled = false;
					}
				}
			}
		}
		
		partial void joinOKButtonPressed (NSObject sender)
		{
			CW8021XProfile profile = null;
			
			joinSpinner.Hidden = false;
			joinSpinner.StartAnimation (Window);
			
			if (joinUser8021XProfilePopupButton.Enabled)
			{
				if(String.Equals(joinUser8021XProfilePopupButton, "Default"))
				{
					profile = CW8021XProfile.Profile;
					profile.Ssid = joinNetworkNameField.StringValue;
					profile.UserDefinedName = joinNetworkNameField.StringValue;
					profile.Username = !String.IsNullOrEmpty (joinUsernameField.StringValue) ? joinUsernameField.StringValue : null;
					profile.Password = !String.IsNullOrEmpty (joinPassphraseField.StringValue) ? joinPassphraseField.StringValue : null;
				}
				else
				{
					var index = joinUser8021XProfilePopupButton.IndexOfSelectedItem;
					if (index >= 0)
						profile = CW8021XProfile.AllUser8021XProfiles[index];
				}
			}
			
			if (JoinDialogContext)
			{
				NSMutableDictionary param = new NSMutableDictionary ();
				if (profile != null)
					param.SetValueForKey (profile, CWConstants.CWAssocKey8021XProfile);
				
				else
					param.SetValueForKey (!String.IsNullOrEmpty (joinPassphraseField.StringValue) ? joinPassphraseField.ObjectValue: null, CWConstants.CWAssocKeyPassPhrase);
				
				NSError error = null;
				bool result = CurrentInterface.AssociateToNetwork (SelectedNetwork, NSDictionary.FromDictionary (param), out error);
				
				joinSpinner.StopAnimation (Window);
				joinSpinner.Hidden = true;
				
				if (!result)
					NSAlert.WithError (error).RunModal ();
					
				else
					joinCancelButtonPressed(this);
			}
		}
		
		partial void joinCancelButtonPressed (NSObject sender)
		{
			NSApplication.SharedApplication.EndSheet(joinDialogWindow);
			joinDialogWindow.OrderOut(sender);
		}
		
		partial void joinButtonPressed (NSObject sender)
		{
			int index = scanResultsTable.SelectedRow;
			
			if (index >= 0)
			{
				ResetDialog ();
				
				SelectedNetwork = ScanResults[index];
				joinNetworkNameField.StringValue = SelectedNetwork.Ssid;
				joinNetworkNameField.Enabled = false;
				joinSecurityPopupButton.SelectItem (StringForSecurityMode (SelectedNetwork.SecurityMode));
				joinSecurityPopupButton.Enabled = false;
				
				changeSecurityMode (null);
				
				CWWirelessProfile wProfile = SelectedNetwork.WirelessProfile;
				CW8021XProfile xProfile = wProfile.User8021XProfile;
				
				switch ((CWSecurityMode)SelectedNetwork.SecurityMode.IntValue)
				{
				case CWSecurityMode.CWSecurityModeWPA_PSK:
				case CWSecurityMode.CWSecurityModeWPA2_PSK:
				case CWSecurityMode.CWSecurityModeWEP:
					if (!String.IsNullOrEmpty (wProfile.Passphrase))
						joinPassphraseField.StringValue = wProfile.Passphrase;
					break;
				
				case CWSecurityMode.CWSecurityModeOpen:
					break;
				case CWSecurityMode.CWSecurityModeWPA_Enterprise:
				case CWSecurityMode.CWSecurityModeWPA2_Enterprise:
					if (xProfile != null)
					{
						joinUser8021XProfilePopupButton.SelectItem (xProfile.UserDefinedName);
						joinUsernameField.StringValue = xProfile.Username;
						joinUsernameField.Enabled = false;
						joinPassphraseField.StringValue = xProfile.Password;
						joinPassphraseField.Enabled = false;
					}
					break;
				}
				
				joinDialogWindow.MakeFirstResponder (joinNetworkNameField);
				
				JoinDialogContext = true;
				NSApplication.SharedApplication.BeginSheet(joinDialogWindow, Window, delegate{ JoinDialogContext = false;});
			}
		}
		
		partial void createIBSSButtonPressed (NSObject sender)
		{
			var machineName = System.Environment.MachineName;
			if (!String.IsNullOrEmpty (machineName))
				ibssNetworkNameField.StringValue = machineName;
			
			ibssChannelPopupButton.AddItem ("11");
			ibssChannelPopupButton.Enabled = false;
			
			ibssChannelPopupButton.SelectItem ("11");
			
			ibssPassphraseField.StringValue = String.Empty;
			
			ibssDialogWindow.MakeFirstResponder (ibssNetworkNameField);
			
			NSApplication.SharedApplication.BeginSheet(ibssDialogWindow, Window);
			
		}
		
		partial void ibssOKButtonPressed (NSObject sender)
		{
			ibssSpinner.Hidden = false;
			ibssSpinner.StartAnimation (this);
			
			string networkName = ibssNetworkNameField.StringValue;
			NSNumber channel = new NSNumber(Convert.ToInt32(ibssChannelPopupButton.SelectedItem.Title));
			string passPhrase = ibssPassphraseField.StringValue;
			
			NSMutableDictionary ibssParams = new NSMutableDictionary ();
			if (!(String.IsNullOrEmpty (networkName)))
				ibssParams.SetValueForKey (ibssNetworkNameField.ObjectValue, CWConstants.CWIBSSKeySSID);
			
			
			if (channel.IntValue > 0)
				ibssParams.SetValueForKey (channel, CWConstants.CWIBSSKeyChannel);
			
			if (!(String.IsNullOrEmpty (passPhrase)))
				ibssParams.SetValueForKey (ibssPassphraseField.ObjectValue, CWConstants.CWIBSSKeyPassphrase);
			
			NSError error = null;
			
			bool created = CurrentInterface.EnableIBSSWithParameters (NSDictionary.FromDictionary (ibssParams), out error);
			
			ibssSpinner.StopAnimation (this);
			ibssSpinner.Hidden = true;
			
			if (!created)
				NSAlert.WithError (error).RunModal ();
			else
				ibssCancelButtonPressed(this);
		}
		
		partial void ibssCancelButtonPressed (NSObject sender)
		{
			NSApplication.SharedApplication.EndSheet (ibssDialogWindow);
			ibssDialogWindow.OrderOut(sender);
		}
		
		#endregion
	}
}
