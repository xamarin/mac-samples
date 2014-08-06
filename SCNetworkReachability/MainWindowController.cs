using System;
using System.Net;

using Foundation;
using AppKit;
using SystemConfiguration;
using CoreWlan;

namespace SCNetworkReachability
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		NetworkReachability hostReachability;
		NetworkReachability networkReachability;
		CWInterface currentInterface;

		public MainWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}

		public MainWindowController () : base ("MainWindow")
		{
		}

		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			InitializeReachability ();
			InitializeWirelessInterfaces ();
		}

		#region Reachability

		void InitializeReachability ()
		{
			networkReachability = new NetworkReachability (IPAddress.Any);
			networkReachability.SetNotification (flags => UpdateReachability (flags, NetworkIcon, NetworkStatusTextField));
			networkReachability.Schedule ();

			NetworkReachabilityFlags networkReachabilityFlags;
			networkReachability.TryGetFlags (out networkReachabilityFlags);
			UpdateReachability (networkReachabilityFlags, NetworkIcon, NetworkStatusTextField);

			CreateHostReachability ();

			HostTextField.Changed += (sender, e) => CreateHostReachability ();
		}

		void CreateHostReachability ()
		{
			if (hostReachability != null) {
				hostReachability.Unschedule ();
				hostReachability.Dispose ();
			}

			if (String.IsNullOrEmpty (HostTextField.StringValue)) {
				HostIcon.Image = NSImage.ImageNamed ("disconnected");
				HostStatusTextField.StringValue = "Enter a host name or IP address";
				return;
			}

			hostReachability = new NetworkReachability (HostTextField.StringValue);
			hostReachability.SetNotification (flags => UpdateReachability (flags, HostIcon, HostStatusTextField));
			hostReachability.Schedule ();

			NetworkReachabilityFlags networkReachabilityFlags;
			networkReachability.TryGetFlags (out networkReachabilityFlags);
			UpdateReachability (networkReachabilityFlags, NetworkIcon, NetworkStatusTextField);
		}

		void UpdateReachability (NetworkReachabilityFlags flags, NSImageView icon, NSTextField statusField)
		{
			if (flags.HasFlag (NetworkReachabilityFlags.Reachable) && !flags.HasFlag (NetworkReachabilityFlags.ConnectionRequired)) {
				icon.Image = NSImage.ImageNamed ("connected");
			} else {
				icon.Image = NSImage.ImageNamed ("disconnected");
			}

			statusField.StringValue = flags == 0 ? String.Empty : flags.ToString ();
		}

		#endregion

		#region Wireless Interfaces

		void InitializeWirelessInterfaces ()
		{
			var ifaces = CWInterface.SupportedInterfaces;
			if (ifaces != null && ifaces.Length >= 0) {
				WirelessInterfaceButton.AddItems (ifaces);
			}

			WirelessInterfaceButton.Activated += (sender, e) => WirelessInterfaceSelected ();
			WirelessInterfaceSelected ();

			WirelessInterfaceToggleButton.Activated += (sender, e) => ChangeWirelessInterfacePower ();
		}

		void WirelessInterfaceSelected ()
		{
			if (WirelessInterfaceButton.SelectedItem != null) {
				currentInterface = CWInterface.FromName (WirelessInterfaceButton.SelectedItem.Title);
			} else {
				currentInterface = null;
			}

			UpdateWirelessInterfaceUI ();
		}

		void UpdateWirelessInterfaceUI ()
		{
			WirelessInterfaceButton.Enabled = currentInterface != null;
			WirelessInterfaceToggleButton.Enabled = currentInterface != null;

			if (currentInterface != null) {
				WirelessInterfaceToggleButton.SelectedSegment = currentInterface.Power ? 0 : 1;
			}
		}

		void ChangeWirelessInterfacePower ()
		{
			NSError error = null;
			if (!currentInterface.SetPower (WirelessInterfaceToggleButton.SelectedSegment == 0, out error)) {
				NSAlert.WithError (error).BeginSheet (Window);
			}

			UpdateWirelessInterfaceUI ();
		}

		#endregion
	}
}