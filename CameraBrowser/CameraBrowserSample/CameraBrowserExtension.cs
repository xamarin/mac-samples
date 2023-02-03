using System;
using ImageCaptureCore;
using System.Linq;

namespace CameraBrowserSample
{
	public static class CameraBrowserExtension 
	{
		public static bool CanSyncClock(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanSyncClock);

		public static bool CanTakePicture(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanTakePicture);

		public static bool CanDeleteOneFile(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanDeleteOneFile);

		public static bool CanDeleteAllFiles(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanDeleteAllFiles);

		public static bool CanReceiveFile(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanReceiveFile);

		public static bool CanEject(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.DeviceCanEjectOrDisconnect);

	}
}

