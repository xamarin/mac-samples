using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreWlan;

namespace CoreWLANWirelessManager
{
	public partial class JoinDialogController : AppKit.NSWindowController
	{
		public  CWNetwork NetworkToJoin { get; private set; }

		public  CWInterface CurrentInterface { get; private set; }
		// Call to load from the XIB/NIB file
		public JoinDialogController (CWNetwork networkToJoin, CWInterface currentInterface) : base ("JoinDialog")
		{
			NetworkToJoin = networkToJoin;
			CurrentInterface = currentInterface;
		}
		//strongly typed window accessor
		public new JoinDialog Window {
			get {
				return (JoinDialog)base.Window;
			}
		}

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			spinner.Hidden = true;

			var phyModes = (CWPhyMode[])Enum.GetValues (typeof(CWPhyMode));
			phyModePicker.AddItems (phyModes.Select (phyMode => phyMode.GetStringInterpretation ()).ToArray<string> ());
			phyModePicker.SelectItem (NetworkToJoin.GetSupportedPHYMode ().GetStringInterpretation ());
			phyModePicker.Enabled = false;

			var securityModes = (CWSecurity[])Enum.GetValues (typeof(CWSecurity));
			securityModePicker.AddItems (securityModes.Select (securityMode => securityMode.GetStringInterpretation ()).ToArray<string> ());

			networkNameTextField.StringValue = NetworkToJoin.Ssid;
			networkNameTextField.Enabled = false;

			securityModePicker.SelectItem (NetworkToJoin.GetSecurityMode ().GetStringInterpretation ());
			securityModePicker.Enabled = false;
		}

		partial void cancelButtonClicked (Foundation.NSObject sender)
		{
			Window.Close ();
		}

		partial void okButtonCkicked (Foundation.NSObject sender)
		{
			spinner.Hidden = false;
			spinner.StartAnimation (this);

			if (CurrentInterface == null)
				return;

			NSError error;
			CurrentInterface.AssociateToNetwork (NetworkToJoin, passphraseTextField.StringValue, out error);

			spinner.StopAnimation (this);
			spinner.Hidden = true;

			if (error != null)
				NSAlert.WithError (error).RunModal ();
			else
				Window.Close ();
		}
	}
}

