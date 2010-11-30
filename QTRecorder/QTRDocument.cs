
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QTKit;

namespace QTRecorder
{
	public partial class QTRDocument : MonoMac.AppKit.NSDocument
	{
		// Called when created from unmanaged code
		public QTRDocument (IntPtr handle) : base(handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public QTRDocument (NSCoder coder) : base(coder)
		{
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);
			
			// Add code to here after the controller has loaded the document window
		}

		// 
		// Save support:
		//    Override one of GetAsData, GetAsFileWrapper, or WriteToUrl.
		//

		// This method should store the contents of the document using the given typeName
		// on the return NSData value.
		public override NSData GetAsData (string documentType, out NSError outError)
		{
			outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
			return null;
		}

		// 
		// Load support:
		//    Override one of ReadFromData, ReadFromFileWrapper or ReadFromUrl
		//
		public override bool ReadFromData (NSData data, string typeName, out NSError outError)
		{
			outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
			return false;
		}
		
		QTCaptureDevice [] videoDevices;
		void RefreshDevices ()
		{
			Console.WriteLine ("Foo");
			WillChangeValue ("VideoDevices");
			videoDevices = QTCaptureDevice.GetInputDevices (QTMediaType.Video).Concat (QTCaptureDevice.GetInputDevices (QTMediaType.Muxed)).ToArray ();
			DidChangeValue ("VideoDevices");
		}
		
		//
		// Connections
		//
		public QTCaptureDevice [] VideoDevices {
			[Export ("VideoDevices")]
			get {
			Console.WriteLine ("Bar");
				if (videoDevices == null)
					RefreshDevices ();
				return videoDevices;
			}
			[Export ("setVideoDevices:")]
			set {
				videoDevices = value;
			}
		}
		
		[Connect ("SelectedVideoDevice")]
		public QTCaptureDevice SelectedVideoDevice { 
			get {
				return null;
			}
			set {
				Console.WriteLine ("Foo");
			}
		}
		
		public override string WindowNibName {
			get {
				return "QTRDocument";
			}
		}
	}
}

