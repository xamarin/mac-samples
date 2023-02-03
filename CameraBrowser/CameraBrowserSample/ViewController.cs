using System;
using System.Linq;
using AppKit;
using Foundation;
using ImageCaptureCore;
using ObjCRuntime;

namespace CameraBrowserSample
{
	public partial class ViewController : NSViewController, IICDeviceBrowserDelegate, IICCameraDeviceDelegate, IICCameraDeviceDownloadDelegate, IICDeviceDelegate {
		public ICDeviceBrowser DeviceBrowser { get; set; } = new ICDeviceBrowser();

		[Export("Cameras")]
		public NSMutableArray Cameras { get; set; } = new NSMutableArray();

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

			NSNotificationCenter.DefaultCenter.AddObserver((NSString)"ICLaunchParamsNotification", (v) => HandleProcessICLaunchParams(v));

            CamerasController.SelectsInsertedObjects = false;
			

			MediaFilesController.AddObserver((NSString)"selectedObjects", (NSKeyValueObservingOptions)0, (v)=>
			{
                WillChangeValue("canDelete");
                WillChangeValue("canDownload");
                DidChangeValue("canDelete");
                DidChangeValue("canDownload");
            });

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
			InvokeOnMainThread(() =>
			{
				//The following are ways to express the same logic using different linq queries

				// can = MediaFilesController.SelectedObjects.Any (file => !((file as ICCameraFile)?.Locked ?? true));
				// -> solid way, good time complexity (O(N)), but not very readable

				// can = MediaFilesController.SelectedObjects.Any( file => !((ICCameraFile) file).Locked);
				// can = MediaFilesController.SelectedObjects.Select(file => (ICCameraFile)file).Any(file => !file.Locked);
				// -> more readable than first ex but casting may throw exceptiion if item is not ICCameraFile

				// can = MediaFilesController.SelectedObjects.Select(file => file as ICCameraFile).Any(file => file is not null && !file.Locked);
				// -> repetitve, iterating over same item again

				// readable, O(2N) complexity but constant is << so barely any significant impact
				can = MediaFilesController.SelectedObjects.Any(obj => obj is ICCameraFile file ? !file.Locked: false);

            });
			return can;
		}

		[Export("canDownload")]
		public bool CanDownload()
		{
			var res = false;
			InvokeOnMainThread(() => {
				res = MediaFilesController.SelectedObjects?.Length > 0;
			} );
			return res;
		}

		[Export("selectedCamera")]
		public ICCameraDevice? SelectedCamera
		{
			get
			{
				return CamerasController.SelectedObjects?.FirstOrDefault() as ICCameraDevice;
			}

		}

		[Export("downloadFiles:")]
		public void DownloadFiles (ICCameraFile[] files)
		{
			var myPictures = (NSString) Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var dict = new NSDictionary<NSString, NSObject>(myPictures, ICCameraDownloadOptionKeys.DownloadsDirectoryUrl);

            foreach (ICCameraFile f in files)
			{
				f?.Device?.RequestDownloadFile(f, dict, DidDownloadFile);
			}
		}

		public void DidDownloadFile(ICCameraFile? file, NSError? error, NSDictionary? options)
		{
			Console.WriteLine("didDownloadFile called with:");
			Console.WriteLine($"file: {file}");
			Console.WriteLine($"error: {error}");
			Console.WriteLine($"options: {options}");
		}

		[Export("readFiles:")]
		public void ReadFiles (ICCameraFile[] files)
		{
			foreach(ICCameraFile f in files)
			{
				f?.Device?.RequestReadDataFromFile(f, 0, f.FileSize, DidReadData);
				
            }
        }

		public void DidReadData(NSData? data, ICCameraFile? file, NSError? error)
		{
			Console.WriteLine("didReadData called with:");
			Console.WriteLine($"data: {data}");
			Console.WriteLine($"file: {file}");
			Console.WriteLine($"error: {error}");
		}

		[Export("openCamera")]
		public void OpenCamera()
		{
			SelectedCamera?.RequestOpenSession();
		}

		// IICDeviceBrowserDelegate Impl

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

		public void DidRemoveDevice(ICDeviceBrowser browser, ICDevice device, bool moreGoing)
		{
			Console.WriteLine($"{nameof(DidRemoveDevice)}: {device}");
			CamerasController.RemoveObject(device);
		}

		public void DeviceDidChangeName(ICDeviceBrowser browser, ICDevice device) => Console.WriteLine($"{nameof(DeviceDidChangeName)}: {device}");

		public void DeviceDidChangeSharingState(ICDeviceBrowser browser, ICDevice device) => Console.WriteLine($"{nameof(DeviceDidChangeSharingState)}: {device}");

		public void RequestsSelectDevice(ICDeviceBrowser browser, ICDevice device) => Console.WriteLine($"{nameof(RequestsSelectDevice)}: {device}");

		// start of camera device

		public void DidAddItem(ICCameraDevice camera, ICCameraItem item) => Console.WriteLine($"{nameof(DidAddItem)}: \n cameraDevice: {camera} item: {item}");

		public void DidRemoveItem(ICCameraDevice camera, ICCameraItem item) => Console.WriteLine($"{nameof(DidRemoveItem)}: \n cameraDevice: {camera} \n item: {item}");

		public void DidRenameItems(ICCameraDevice camera, ICCameraItem[] items) => Console.WriteLine($"{nameof(DidRenameItems)}:\n device: {camera} \n items: {items}");

		public void DidCompleteDeleteFiles(ICCameraDevice scanner, NSError? error) => Console.WriteLine($"{nameof(DidCompleteDeleteFiles)}:\n device: {scanner} \n error: {error}");

		public void DidChangeCapability(ICCameraDevice camera) => Console.WriteLine($"cameraDeviceDidChangeCapability: {camera}");

		public void DidReceiveThumbnail(ICCameraDevice camera, ICCameraItem forItem) => Console.WriteLine($"{nameof(DidReceiveThumbnail)}:\n device: {camera} \n item: {forItem}");

		public void DidReceiveMetadata(ICCameraDevice camera, ICCameraItem forItem) => Console.WriteLine($"{nameof(DidReceiveMetadata)}:\n device: {camera} \n item: {forItem}");

		public void DidReceivePtpEvent(ICCameraDevice camera, NSData eventData) => Console.WriteLine($"{nameof(DidReceivePtpEvent)}:\n device: {camera} \n data: {eventData}");

		// start of IC device

		public void DidOpenSession(ICDevice device, NSError error) => Console.WriteLine($"{nameof(DidOpenSession)}: \n device: {device} \n error: {error}");

		public void DidCloseSession(ICDevice device, NSError error) => Console.WriteLine($"{nameof(DidCloseSession)}: \n device: {device} \n error: {error}");

		public void DidChangeName(ICDevice device) => Console.WriteLine($"{nameof(DidChangeName)}: \n device: {device}");

		public void DidChangeSharingState(ICDevice device) => Console.WriteLine($"{nameof(DidChangeSharingState)}: \n device: {device}");

		public void DidReceiveStatusInformation(ICDevice device, NSDictionary<NSString, NSObject> status) => Console.WriteLine($"{nameof(DidReceiveStatusInformation)}: \n device: {device} \n status: {status}");

		public void DidEncounterError(ICDevice device, NSError error) => Console.WriteLine($"{nameof(DidEncounterError)}: \n device: {device} \n error: {error}");

		public void DidBecomeReadyWithCompleteContentCatalog(ICDevice device) => Console.WriteLine($"{nameof(DidBecomeReadyWithCompleteContentCatalog)}: \n device: {device}");

		public void DidRemoveDevice(ICDevice device) => Console.WriteLine($"{nameof(DidRemoveDevice)}:\n device: {device}");


	}
}
