//
// MainWindowController.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.
// Copyright 2017 Microsoft.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using CoreGraphics;

using AppKit;
using CoreAnimation;
using CoreBluetooth;
using Foundation;

namespace Xamarin.HeartMonitor
{	
	public partial class MainWindowController : NSWindowController
	{
		readonly CBCentralManager manager = new CBCentralManager ();
		readonly HeartRateMonitorTableDataSource heartRateMonitors = new HeartRateMonitorTableDataSource ();

		HeartRateMonitor connectedMonitor;

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
		
		void DismissDeviceListSheet (NSObject sender)
		{
			NSApplication.SharedApplication.EndSheet (deviceListSheet);
			deviceListSheet.OrderOut (sender);
		}
		
		public override void AwakeFromNib ()
		{
			DisconnectMonitor ();
			InitializeAnimation ();
			
			deviceListScanningProgressIndicator.StartAnimation (this);

			deviceTableView.DataSource = heartRateMonitors;
			deviceTableView.DoubleClick += ConnectToSelectedDevice;
			
			connectButton.Activated += (sender, e) => NSApplication.SharedApplication.BeginSheet (deviceListSheet, Window);
			disconnectButton.Activated += (sender, e) => DisconnectMonitor ();
			dismissDeviceListButton.Activated += (sender, e) => DismissDeviceListSheet ((NSObject)sender);
			chooseDeviceButton.Activated += ConnectToSelectedDevice;

			InitializeCoreBluetooth ();
		}
		
		void ConnectToSelectedDevice (object sender, EventArgs e)
		{
			var peripheral = heartRateMonitors [(int)deviceTableView.SelectedRow];
			if (peripheral != null) {
				DisconnectMonitor ();

				statusLabel.StringValue = "Connecting...";

				connectedMonitor = new HeartRateMonitor (manager, peripheral);
				connectedMonitor.HeartBeat += OnHeartBeat;
				connectedMonitor.HeartRateUpdated += OnHeartRateUpdated;
				connectedMonitor.RssiUpdated += OnRssiUpdated;
				connectedMonitor.NameUpdated += OnNameUpdated;
				connectedMonitor.Connect ();
			}

			DismissDeviceListSheet ((NSObject)sender);
		}

		#region Heart Pulse UI
		
		void DisconnectMonitor ()
		{
			statusLabel.StringValue = "Not connected";
			heartRateLabel.IntValue = 0;
			heartRateLabel.Hidden = true;
			heartRateUnitLabel.Hidden = true;
			rssiLabel.Hidden = true;
			deviceNameLabel.StringValue = String.Empty;
			deviceNameLabel.Hidden = true;
			disconnectButton.Hidden = true;

			if (connectedMonitor != null) {
				connectedMonitor.HeartBeat -= OnHeartBeat;
				connectedMonitor.HeartRateUpdated -= OnHeartRateUpdated;
				connectedMonitor.RssiUpdated -= OnRssiUpdated;
				connectedMonitor.NameUpdated -= OnNameUpdated;
				connectedMonitor.Dispose ();
				connectedMonitor = null;
			}
		}

		void InitializeAnimation ()
		{
			NSAnimationContext.BeginGrouping ();
			NSAnimationContext.CurrentContext.Duration = 0f;
			
			heartImage.WantsLayer = true;
			heartImage.Layer.AnchorPoint = new CGPoint (0.5f, 0.5f);
			heartImage.Layer.Position = new CGPoint (
				heartImage.Layer.Frame.Size.Width / 2,
				heartImage.Layer.Frame.Size.Height / 2
			);

			NSAnimationContext.EndGrouping ();
		}

		void OnHeartRateUpdated (object sender, HeartBeatEventArgs e)
		{
			heartRateUnitLabel.Hidden = false;
			heartRateLabel.Hidden = false;
			heartRateLabel.IntValue = e.CurrentHeartBeat.Rate;

			var monitor = (HeartRateMonitor)sender;
			if (monitor.Location == HeartRateMonitorLocation.Unknown) {
				statusLabel.StringValue = "Connected";
			} else {
				statusLabel.StringValue = String.Format ("Connected on {0}", monitor.Location);
			}
		}

		void OnHeartBeat (object sender, EventArgs e)
		{
			using (var anim = CABasicAnimation.FromKeyPath ("transform.scale")) {
				anim.From = NSNumber.FromFloat (1.0f);
				anim.To = NSNumber.FromFloat (1.2f);
				anim.Duration = 0.2;
				anim.RepeatCount = 1;
				anim.AutoReverses = true;
				anim.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseIn);
				heartImage.Layer.AddAnimation (anim, "scale");
			}
		}

		void OnRssiUpdated (object sender, EventArgs e)
		{
			rssiLabel.Hidden = connectedMonitor.Peripheral.RSSI == null;
			rssiLabel.StringValue = $"RSSI: {connectedMonitor.Peripheral.RSSI} dB";
		}

		void OnNameUpdated (object sender, EventArgs e)
		{
			deviceNameLabel.Hidden = false;
			deviceNameLabel.StringValue = connectedMonitor.Name;
		}

		#endregion

		#region Bluetooth

		void LogPeripheral (string message, CBPeripheral peripheral)
			=> Console.WriteLine ($"{message}: {peripheral.Identifier} {peripheral}");

		void InitializeCoreBluetooth ()
		{
			manager.UpdatedState += OnCentralManagerUpdatedState;

			manager.DiscoveredPeripheral += (sender, e) => {
				LogPeripheral ("Discovered", e.Peripheral);
				heartRateMonitors.Add (e.Peripheral);
				deviceTableView.ReloadData ();
			};

			manager.FailedToConnectPeripheral += (sender, e) => {
				LogPeripheral ($"Failed to connect ({e.Error})", e.Peripheral);
				DisconnectMonitor ();
			};

			manager.ConnectedPeripheral += (sender, e) => {
				LogPeripheral ("Connected", e.Peripheral);
				statusLabel.StringValue = "Discovering services...";
				disconnectButton.Hidden = false;
				e.Peripheral.DiscoverServices ();
			};

			manager.DisconnectedPeripheral += (sender, e) => {
				LogPeripheral ("Disconneceted", e.Peripheral);
				heartRateMonitors.Remove (e.Peripheral);
				deviceTableView.ReloadData ();
				DisconnectMonitor ();
			};
		}

		void OnCentralManagerUpdatedState (object sender, EventArgs e)
		{
			string message = null;

			switch (manager.State) {
			case CBCentralManagerState.PoweredOn:
				if (!connectButton.Enabled) {
					HeartRateMonitor.ScanForHeartRateMonitors(manager);
					connectButton.Enabled = true;
				}
				return;
			case CBCentralManagerState.Unsupported:
				message = "The platform or hardware does not support Bluetooth Low Energy.";
				break;
			case CBCentralManagerState.Unauthorized:
				message = "The application is not authorized to use Bluetooth Low Energy.";
				break;
			case CBCentralManagerState.PoweredOff:
				message = "Bluetooth is currently powered off.";
				break;
			default:
				break;
			}

			if (message != null) {
				new NSAlert {
					MessageText = "Heart Rate Monitor cannot be used at this time.",
					InformativeText = message
				}.RunSheetModal (Window);
				NSApplication.SharedApplication.Terminate (manager);
			}
		}

		#endregion
	}
}