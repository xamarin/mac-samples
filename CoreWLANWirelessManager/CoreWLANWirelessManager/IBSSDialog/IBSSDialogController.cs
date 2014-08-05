using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreWlan;

namespace CoreWLANWirelessManager
{
	public partial class IBSSDialogController : AppKit.NSWindowController
	{
		public  CWInterface CurrentInterface { get; private set; }

		// Call to load from the XIB/NIB file
		public IBSSDialogController (CWInterface currentInterface) : base ("IBSSDialog")
		{
			CurrentInterface = currentInterface;
		}

		//strongly typed window accessor
		public new IBSSDialog Window {
			get {
				return (IBSSDialog)base.Window;
			}
		}

		partial void cancelButtonClicked (AppKit.NSButton sender)
		{
			Window.Close();
		}

		partial void okButtonClicked (AppKit.NSButton sender)
		{
			spinner.Hidden = false;
			spinner.StartAnimation(this);

			NSError error;
			var networkName = nameTextField.StringValue;
			var password = passwordTextField.StringValue;
			var channelNumber = UInt32.Parse(channelPicker.SelectedItem.Title);
			var security = string.IsNullOrEmpty(password) ? CWIbssModeSecurity.None : CWIbssModeSecurity.WEP40;

			CurrentInterface.StartIbssModeWithSsid(new NSData(), security, channelNumber, password, out error);

			spinner.StopAnimation(this);
			spinner.Hidden = true;

			if(error != null)
				NSAlert.WithError(error).RunModal();
			else
				Window.Close();
		}

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			spinner.Hidden = true;
			channelPicker.AddItems (new string[] { "1", "2", "3", "4", "5", "6", "7", "8", 
				"9", "10", "11", "36", "40", "44", "48"});

			channelPicker.SelectItem ("11");

			var machineName = NSHost.Current.LocalizedName;
			if(!string.IsNullOrEmpty(machineName)) {
				nameTextField.StringValue = machineName;
			}
		}
	}
}

