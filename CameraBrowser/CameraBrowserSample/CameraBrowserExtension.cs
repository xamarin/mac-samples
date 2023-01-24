using System;
using ImageCaptureCore;
using System.Linq;

namespace CameraBrowserSample
{
	public static class CameraBrowserExtension 
    {
		public static bool canSyncClock(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanSyncClock);

        public static bool canTakePicture(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanTakePicture);

        public static bool canDeleteOneFile(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanDeleteOneFile);

        public static bool canDeleteAllFiles(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanDeleteAllFiles);

        public static bool canReceiveFile(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.CameraDeviceCanReceiveFile);

        public static bool canEject(this ICDevice self) => self.Capabilities.Contains(ICDeviceCapabilities.DeviceCanEjectOrDisconnect);

    }
}

