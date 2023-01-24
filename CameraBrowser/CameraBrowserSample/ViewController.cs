using System;
using AppKit;
using Foundation;
using ImageCaptureCore;
using ObjCRuntime;

namespace CameraBrowserSample
{

    public partial class ViewController : NSViewController, IICDeviceBrowserDelegate, IICCameraDeviceDelegate, IICCameraDeviceDownloadDelegate, IICDeviceDelegate {
        public ICDeviceBrowser DeviceBrowser { get; set; } = new ICDeviceBrowser();
        public NSMutableArray Cameras { [Export("Cameras")] get; [Export("setCameras:")] set; } = new NSMutableArray();

        static ViewController() //init
        {
            var imageTransformer = new CGImageRefToNSImageTransformer();
            NSValueTransformer.SetValueTransformer(imageTransformer, "CGImageRefToNSImage");
        }

        public ViewController (IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            View.Window.Title = "Camera Browser Sample for Xamarin.Mac ";
        }

        public void HandleProcessICLaunchParams(NSNotification note) => Console.WriteLine($"NSNotification: {note}");

        public override void ViewDidLoad () //application did finish launching
		{
			base.ViewDidLoad ();

            // Do any additional setup after loading the view.

            CamerasController.SelectsInsertedObjects = false;

            NSNotificationCenter.DefaultCenter.AddObserver(this, new Selector("processICLaunchParams:"), (NSString)"ICLaunchParamsNotification", null);

            CamerasController.SelectsInsertedObjects = false;

            MediaFilesController.AddObserver(this, (NSString)"selectedObjects", (NSKeyValueObservingOptions)0, (IntPtr)null);

            DeviceBrowser.Delegate = this;
            DeviceBrowser.BrowsedDeviceTypeMask = ICBrowsedDeviceType.Local | ICBrowsedDeviceType.Remote | ICBrowsedDeviceType.Camera;
            DeviceBrowser.Start();
        }

        [Export("observeValueForKeyPath:ofObject:change:context:")]
        private void ObserveValueForKeyPath (NSString keyPath, NSArrayController ofObject, NSDictionary change, IntPtr context)
        {
            if (keyPath == "selectedObjects" && ofObject == MediaFilesController)
            {
                WillChangeValue("canDelete");
                WillChangeValue("canDownload");
                DidChangeValue("canDelete");
                DidChangeValue("canDownload");

            }
        }

        [Export("canDelete")]
        public bool CanDelete ()
        {
            bool can = false;
            InvokeOnMainThread(() => {
                var selectedFiles = MediaFilesController.SelectedObjects;
                if (selectedFiles.Length > 0)
                {
                    foreach (ICCameraFile f in selectedFiles)
                    {
                        if (!f.Locked)
                        {
                            can = true;
                            break;
                        }
                    }
                }
            });
            return can;
        }

        [Export("canDownload")]
        public bool CanDownload()
        {
            var res = false;
            InvokeOnMainThread(() => {
                res = MediaFilesController.SelectedObjects != null ? MediaFilesController.SelectedObjects.Length > 0 : false;
            } );
            return res;
        }

        [Export("selectedCamera")]
        public ICCameraDevice SelectedCamera
        {
            get
            {
                var selectedObjects = CamerasController.SelectedObjects;
                if (selectedObjects == null || selectedObjects.Length == 0)
                    return null;
                return selectedObjects[0] as ICCameraDevice;
            }

        }

        [Export("downloadFiles:")]
        public void downloadFiles (ICCameraFile[] files)
        {
            NSString filePath = (NSString)"~/Pictures";
            var filePath1 = filePath.ExpandTildeInPath();
            string[] fileUrl = new string[] { filePath1 };

            var objects = new[] { NSUrl.CreateFileUrl(fileUrl) };
            var keys = new[] { ICCameraDownloadOptionKeys.DownloadsDirectoryUrl };
            var dict = new NSDictionary<NSString, NSObject>(keys, objects);
            foreach (ICCameraFile f in files)
            {
                //f.Device.RequestDownloadFile(f, dict, this, Selector.FromHandle(Selector.GetHandle("didDownloadFile:error:options:contextInfo:")), IntPtr.Zero);
                f.Device.RequestDownloadFile(f, dict, didDownloadFileDel);
            }
        }

        public void didDownloadFileDel(ICCameraFile f, NSError err, NSDictionary options)
        {
            Console.WriteLine("didDownloadFile called with:");
            Console.WriteLine($"file: {f}");
            Console.WriteLine($"error: {err}");
            Console.WriteLine($"options: {options}");
        }

        [Export("readFiles:")]
        public void readFiles (ICCameraFile[] files)
        {
            foreach(ICCameraFile f in files)
            {
                //f.Device.RequestReadDataFromFile(f, 0, f.FileSize, this, Selector.FromHandle(Selector.GetHandle("didReadData:fromFile:error:contextInfo:")), IntPtr.Zero);
                f.Device.RequestReadDataFromFile(f, 0, f.FileSize, didReadDataDel);
            }
        }

        public void didReadDataDel(NSData data, ICCameraFile file, NSError err)
        {
            Console.WriteLine("didReadData called with:");
            Console.WriteLine($"data: {data}");
            Console.WriteLine($"file: {file}");
            Console.WriteLine($"error: {err}");
        }

        [Export("openCamera")]
        public void openCamera()
        {
            SelectedCamera.RequestOpenSession();
        }

        // IICDeviceBrowserDelegate Impl

        [Export("deviceBrowser:didAddDevice:moreComing:")]
        public void DidAddDevice(ICDeviceBrowser browser, ICDevice device, bool moreComing)
        {
            Console.WriteLine($"{nameof(DidAddDevice)}: {device}");

            if (device.Type.HasFlag(ICDeviceType.Camera))
            {
                WillChangeValue("Cameras");
                Cameras.Add(device);
                DidChangeValue("Cameras");
                device.Delegate = this;
            }
        }

        [Export("deviceBrowser:didRemoveDevice:moreGoing:")]
        public void DidRemoveDevice(ICDeviceBrowser browser, ICDevice device, bool moreGoing)
        {
            Console.WriteLine($"{nameof(DidRemoveDevice)}: {device}");
            CamerasController.RemoveObject(device);
        }

        [Export("deviceBrowser:deviceDidChangeName:")]
        public void DeviceDidChangeName(ICDeviceBrowser browser, ICDevice device) => Console.WriteLine($"{nameof(DeviceDidChangeName)}: {device}");

        [Export("deviceBrowser:deviceDidChangeSharingState:")]
        public void DeviceDidChangeSharingState(ICDeviceBrowser browser, ICDevice device) => Console.WriteLine($"{nameof(DeviceDidChangeSharingState)}: {device}");

        [Export("deviceBrowser:requestsSelectDevice:")]
        public void RequestsSelectDevice(ICDeviceBrowser browser, ICDevice device) => Console.WriteLine($"{nameof(RequestsSelectDevice)}: {device}");

        // start of camera device

        [Export("cameraDevice:didAddItem:")]
        public void DidAddItem(ICCameraDevice camera, ICCameraItem item) => Console.WriteLine($"{nameof(DidAddItem)}: \n cameraDevice: {camera} item: {item}");

        [Export("cameraDevice:didRemoveItem:")]
        public void DidRemoveItem(ICCameraDevice camera, ICCameraItem item) => Console.WriteLine($"{nameof(DidRemoveItem)}: \n cameraDevice: {camera} \n item: {item}");

        [Export("cameraDevice:didRenameItems:")]
        public void DidRenameItems(ICCameraDevice camera, ICCameraItem[] items) => Console.WriteLine($"{nameof(DidRenameItems)}:\n device: {camera} \n items: {items}");

        [Export("cameraDevice:didCompleteDeleteFilesWithError:")]
        public void DidCompleteDeleteFiles(ICCameraDevice scanner, NSError error) => Console.WriteLine($"{nameof(DidCompleteDeleteFiles)}:\n device: {scanner} \n error: {error}");

        [Export("cameraDeviceDidChangeCapability:")]
        public void DidChangeCapability(ICCameraDevice camera) => Console.WriteLine($"cameraDeviceDidChangeCapability: {camera}");

        [Export("cameraDevice:didReceiveThumbnailForItem:")]
        public void DidReceiveThumbnail(ICCameraDevice camera, ICCameraItem forItem) => Console.WriteLine($"{nameof(DidReceiveThumbnail)}:\n device: {camera} \n item: {forItem}");

        [Export("cameraDevice:didReceiveMetadataForItem:")]
        public void DidReceiveMetadata(ICCameraDevice camera, ICCameraItem forItem) => Console.WriteLine($"{nameof(DidReceiveMetadata)}:\n device: {camera} \n item: {forItem}");

        [Export("cameraDevice:didReceivePTPEvent:")]
        public void DidReceivePtpEvent(ICCameraDevice camera, NSData eventData) => Console.WriteLine($"{nameof(DidReceivePtpEvent)}:\n device: {camera} \n data: {eventData}");

        // start of IC device

        [Export("device:didOpenSessionWithError:")]
        public void DidOpenSession(ICDevice device, NSError error) => Console.WriteLine($"{nameof(DidOpenSession)}: \n device: {device} \n error: {error}");

        [Export("device:didCloseSessionWithError:")]
        public void DidCloseSession(ICDevice device, NSError error) => Console.WriteLine($"{nameof(DidCloseSession)}: \n device: {device} \n error: {error}");

        [Export("deviceDidChangeName:")]
        public void DidChangeName(ICDevice device) => Console.WriteLine($"{nameof(DidChangeName)}: \n device: {device}");

        [Export("deviceDidChangeSharingState:")]
        public void DidChangeSharingState(ICDevice device) => Console.WriteLine($"{nameof(DidChangeSharingState)}: \n device: {device}");

        [Export("device:didReceiveStatusInformation:")]
        public void DidReceiveStatusInformation(ICDevice device, NSDictionary<NSString, NSObject> status) => Console.WriteLine($"{nameof(DidReceiveStatusInformation)}: \n device: {device} \n status: {status}");

        [Export("device:didEncounterError:")]
        public void DidEncounterError(ICDevice device, NSError error) => Console.WriteLine($"{nameof(DidEncounterError)}: \n device: {device} \n error: {error}");

        [Export("deviceDidBecomeReady:")]
        public void DidBecomeReadyWithCompleteContentCatalog(ICDevice device) => Console.WriteLine($"{nameof(DidBecomeReadyWithCompleteContentCatalog)}: \n device: {device}");

        [Export("didRemoveDevice:")]
        public void DidRemoveDevice(ICDevice device) => Console.WriteLine($"{nameof(DidRemoveDevice)}:\n device: {device}");


    }
}
