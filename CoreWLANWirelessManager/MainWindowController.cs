#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoMac.AppKit;
using MonoMac.CoreWlan;
using MonoMac.Foundation;
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
			supportedInterfacesPopup.AddItems (CWInterface.InterfaceNames);
			
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
			bool powerState = CurrentInterface.PowerOn;
			powerStateControl.SetSelected (true, powerState ? 0 : 1);
			bool isRunning = CurrentInterface.ActivePHYMode != CWPhyMode.None;
			
			if (isRunning)
				disconnectButton.Enabled = true;
			else
				disconnectButton.Enabled = false;
			
			opModeField.StringValue = powerState ? CWHelpers.StringForOpMode (CurrentInterface.InterfaceMode) : String.Empty;
			
			securityModeField.StringValue = powerState ? CWHelpers.StringForSecurityMode (CurrentInterface.Security) : String.Empty;
			
			phyModeField.StringValue = powerState ? CWHelpers.StringForPhyMode (CurrentInterface.ActivePHYMode) : String.Empty;
			
			string str = CurrentInterface.Ssid;
			ssidField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			str = CurrentInterface.Bssid;
			bssidField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			NSNumber num = CurrentInterface.TransmitRate;
			txRateField.StringValue = (powerState && num!=null) ? String.Format ("{0} Mbps", num) : String.Empty;
			
			num = CurrentInterface.RssiValue;
			rssiField.StringValue = (powerState && num!=null)? String.Format ("{0} dBm", num) : String.Empty;
			
			num = CurrentInterface.NoiseMeasurement;
			noiseField.StringValue = (powerState && num!=null)? String.Format ("{0} dBm", num) : String.Empty;
			
			num = CurrentInterface.TransmitPower;
			txPowerField.StringValue = (powerState && num!=null)? String.Format ("{0} mW", num) : String.Empty;
			
			str = CurrentInterface.CountryCode;
			countryCodeField.StringValue = (powerState && !String.IsNullOrEmpty (str)) ? str : String.Empty;
			
			uint[] supportedChannelsArray = CurrentInterface.SupportedWlanChannels.Select (x => x.ChannelNumber).ToArray();

			StringBuilder supportedChannelString = new StringBuilder ();
			channelPopup.RemoveAllItems ();
			
			foreach (NSNumber eachChannel in supportedChannelsArray)
			{
				if (eachChannel.IsEqualToNumber (supportedChannelsArray.Last ()))
					supportedChannelString.Append (eachChannel.ToString ());
				else
					supportedChannelString.AppendFormat ("{0}, ", eachChannel.ToString ());
				if (powerState)
					channelPopup.AddItem (eachChannel.ToString ());
			}

			supportedChannelsField.StringValue = supportedChannelString.ToString ();

			string phyModeSuffix = CWHelpers.StringForPhyMode (CurrentInterface.ActivePHYMode);
			string phyMode = "None";
			if (phyModeSuffix != string.Empty)
				phyMode = "802.11" + phyModeSuffix;
				
			supportedPHYModes.StringValue = phyMode;
			
			channelPopup.SelectItem(CurrentInterface.WlanChannel != null ? CurrentInterface.WlanChannel.ToString () : String.Empty);
			if(!powerState || isRunning)
				channelPopup.Enabled = false;
			else
				channelPopup.Enabled = true;
		}
		
		private void UpdateScanTab ()
		{
			NSError error = null;
			ScanResults = CurrentInterface.ScanForNetworksWithName (null, out error);
			if (error == null)
			{
				Array.Sort (ScanResults, delegate(CWNetwork networkA, CWNetwork networkB) {
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
												"Open", "WEP", "WPA Personal", "WPA2 Personal",
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
			CurrentInterface = new CWInterface (supportedInterfacesPopup.SelectedItem.Title);
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
			NSError error;
			uint channelNumber = uint.Parse(channelPopup.SelectedItem.Title);
			CWChannel selectedChannel = CurrentInterface.SupportedWlanChannels.First (x => x.ChannelNumber == channelNumber);
			bool result = CurrentInterface.SetWlanChannel (selectedChannel, out error);
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

				string password = !string.IsNullOrEmpty(joinPassphraseField.StringValue) ? joinPassphraseField.StringValue : null;
				NSError error = null;
				bool result = CurrentInterface.AssociateToNetwork (SelectedNetwork, password, out error);

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

				CWSecurity security = CWHelpers.SecurityModeFromSSID (ScanResults, SelectedNetwork.Ssid);
				joinSecurityPopupButton.SelectItem (CWHelpers.StringForSecurityMode (security));
				joinSecurityPopupButton.Enabled = false;
				
				changeSecurityMode (null);
							
				if (SelectedNetwork.SupportsSecurity(CWSecurity.WPAPersonal) || SelectedNetwork.SupportsSecurity(CWSecurity.WPA2Personal) ||
					SelectedNetwork.SupportsSecurity(CWSecurity.WEP))
				{
					joinPassphraseField.StringValue = "";
				}
				else if (SelectedNetwork.SupportsSecurity(CWSecurity.WPAEnterprise) || SelectedNetwork.SupportsSecurity(CWSecurity.WPA2Enterprise))
				{
					joinUser8021XProfilePopupButton.SelectItem(0);
					joinUsernameField.StringValue = "";
					joinUsernameField.Enabled = false;
					joinPassphraseField.StringValue = "";
					joinPassphraseField.Enabled = false;
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
			
			string passPhrase = ibssPassphraseField.StringValue;

			NSError error = null;

			bool created = CurrentInterface.StartIbssModeWithSsid (null, CWIbssModeSecurity.None, uint.Parse (ibssChannelPopupButton.SelectedItem.Title), passPhrase, out error);

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
