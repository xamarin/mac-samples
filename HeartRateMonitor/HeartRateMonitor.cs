//
// HeartRateMonitor.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2013 Xamarin, Inc.
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

using Foundation;
using CoreBluetooth;

namespace Xamarin.HeartMonitor
{
	public class HeartRateMonitor : CBPeripheralDelegate
	{
		static readonly CBUUID PeripheralUUID = CBUUID.FromPartial (0x180D);
		static readonly CBUUID HeartRateMeasurementCharacteristicUUID = CBUUID.FromPartial (0x2A37);
		static readonly CBUUID BodySensorLocationCharacteristicUUID = CBUUID.FromPartial (0x2A38);
		static readonly CBUUID HeartRateControlPointCharacteristicUUID = CBUUID.FromPartial (0x2A39);
		
		public static void ScanForHeartRateMonitors (CBCentralManager manager)
		{
			if (manager == null) {
				throw new ArgumentNullException ("manager");
			}

			manager.ScanForPeripherals (PeripheralUUID);			
		}
		
		NSTimer beatTimer;
		bool disposed;

		public CBCentralManager Manager { get; private set; }
		public CBPeripheral Peripheral { get; private set; }

		public HeartRateMonitorLocation Location { get; private set; }
		public HeartBeat CurrentHeartBeat { get; private set; }
		public HeartBeat PreviousHeartBeat { get; private set; }

		public event EventHandler<HeartBeatEventArgs> HeartRateUpdated;
		public event EventHandler HeartBeat;
		public event EventHandler LocationUpdated;

		public string Name {
			get { return Peripheral.Name; }
		}

		protected override void Dispose (bool disposing)
		{
			disposed = true;
			if (beatTimer != null) {
				beatTimer.Dispose ();
				beatTimer = null;
			}
	
			base.Dispose (disposing);
		}

		public HeartRateMonitor (CBCentralManager manager, CBPeripheral peripheral)
		{
			if (manager == null) {
				throw new ArgumentNullException ("manager");
			} else if (peripheral == null) {
				throw new ArgumentNullException ("peripheral");
			}
			
			Location = HeartRateMonitorLocation.Unknown;
			
			Manager = manager;

			Peripheral = peripheral;
			Peripheral.Delegate = this;
			Peripheral.DiscoverServices ();
		}
		
		public void Connect ()
		{
			if (disposed) {
				return;
			}

			Manager.ConnectPeripheral (Peripheral, new PeripheralConnectionOptions {
				NotifyOnDisconnection = true
			});
		}

		public override void DiscoveredService (CBPeripheral peripheral, NSError error)
		{
			if (disposed) {
				return;
			}

			foreach (var service in peripheral.Services) {
				if (service.UUID == PeripheralUUID) {
					peripheral.DiscoverCharacteristics (service);
				}
			}
		}

		public override void DiscoveredCharacteristic (CBPeripheral peripheral, CBService service, NSError error)
		{
			if (disposed) {
				return;
			}

			foreach (var characteristic in service.Characteristics) {
				if (characteristic.UUID == HeartRateMeasurementCharacteristicUUID) {
					service.Peripheral.SetNotifyValue (true, characteristic);
				} else if (characteristic.UUID == BodySensorLocationCharacteristicUUID) {
					service.Peripheral.ReadValue (characteristic);
				} else if (characteristic.UUID == HeartRateControlPointCharacteristicUUID) {
					service.Peripheral.WriteValue (NSData.FromBytes ((IntPtr)1, 1),
						characteristic, CBCharacteristicWriteType.WithResponse);
				}
			}
		}
		
		public override void UpdatedCharacterteristicValue (CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
		{
			if (disposed || error != null || characteristic.Value == null) {
				return;
			}

			if (characteristic.UUID == HeartRateMeasurementCharacteristicUUID) {
				UpdateHeartRate (characteristic.Value);
			} else if (characteristic.UUID == BodySensorLocationCharacteristicUUID) {
				UpdateBodySensorLocation (characteristic.Value);
			}
		}
		
		protected virtual void OnHeartRateUpdated ()
		{
			var handler = HeartRateUpdated;
			if (handler != null) {
				handler (this, new HeartBeatEventArgs (PreviousHeartBeat, CurrentHeartBeat));
			}
		}
		
		protected virtual void OnHeartBeat ()
		{
			var handler = HeartBeat;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}
		
		protected virtual void OnLocationUpdated ()
		{
			var handler = LocationUpdated;
			if (handler != null) {
				handler (this, EventArgs.Empty);
			}
		}
		
		void ScheduleBeatTimer ()
		{
			if (disposed) {
				return;
			}
		
			if (beatTimer != null) {
				beatTimer.Dispose ();
			}
			
			OnHeartBeat ();
			beatTimer = NSTimer.CreateScheduledTimer (60 / (double)CurrentHeartBeat.Rate, delegate {
				ScheduleBeatTimer ();
			});
				
		}
		
		unsafe void UpdateHeartRate (NSData hr)
		{
			var now = DateTime.Now;
		
			var data = (byte *)hr.Bytes;
			ushort bpm = 0;
			if ((data [0] & 0x01) == 0) {
				bpm = data [1];
			} else {
				bpm = (ushort)data [1];
				bpm = (ushort)(((bpm >> 8) & 0xFF) | ((bpm << 8) & 0xFF00));
			}
						
			PreviousHeartBeat = CurrentHeartBeat;
			CurrentHeartBeat = new HeartBeat { Time = now, Rate = bpm };

			OnHeartRateUpdated ();

			if (PreviousHeartBeat.Rate == 0 && CurrentHeartBeat.Rate != 0) {			
				OnHeartBeat ();
				ScheduleBeatTimer ();
			}
		}
		
		unsafe void UpdateBodySensorLocation (NSData location)
		{
			var value = ((byte *)location.Bytes) [0];
			if (value < 0 || value > (byte)HeartRateMonitorLocation.Reserved) {
				Location = HeartRateMonitorLocation.Unknown;
			} else {
				Location = (HeartRateMonitorLocation)value;
			}
			
			OnLocationUpdated ();
		}
	}
}