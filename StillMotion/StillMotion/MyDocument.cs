
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using QTKit;
using CoreVideo;
using CoreImage;

namespace StillMotion
{
	public partial class MyDocument : AppKit.NSDocument
	{
		QTMovie movie;
		QTCaptureSession captureSession;
		QTCaptureDeviceInput captureInput;
		QTCaptureDecompressedVideoOutput decompressedVideo;
		volatile CVImageBuffer currentImage;

		QTImageAttributes attrs = new QTImageAttributes () { CodecType = "jpeg" };

		// If this returns the name of a NIB file instead of null, a NSDocumentController
		// is automatically created for you.
		public override string WindowNibName { 
			get {
				return "StillMotion";
			}
		}

		// Called when created from unmanaged code
		public MyDocument (IntPtr handle) : base (handle)
		{
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MyDocument (NSCoder coder) : base (coder.Handle)
		{
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			NSError err;

			// Create a movie, and store the information in memory on an NSMutableData
			movie = new QTMovie (new NSMutableData (1), out err);
			if (movie == null) {
				NSAlert.WithError (err).RunModal ();
				return;
			}

			movieView.Movie = movie;

			// Find video device
			captureSession = new QTCaptureSession ();
			var device = QTCaptureDevice.GetDefaultInputDevice (QTMediaType.Video);
			if (device == null) {
				new NSAlert { MessageText = "You do not have a camera connected." }.BeginSheet (windowController.Window);
				return;
			} else if (!device.Open (out err)) {
				NSAlert.WithError (err).BeginSheet (windowController.Window);
				return;
			}

			// Add device input
			captureInput = new QTCaptureDeviceInput (device);
			if (!captureSession.AddInput (captureInput, out err)) {
				NSAlert.WithError (err).BeginSheet (windowController.Window);
				return;
			}

			// Create decompressor for video output, to get raw frames
			decompressedVideo = new QTCaptureDecompressedVideoOutput ();
			decompressedVideo.DidOutputVideoFrame += delegate(object sender, QTCaptureVideoFrameEventArgs e) {
				lock (this) {
					currentImage = e.VideoFrame;
				}
			};
			if (!captureSession.AddOutput (decompressedVideo, out err)) {
				NSAlert.WithError (err).BeginSheet (windowController.Window);
				return;
			}

			// Activate preview
			captureView.CaptureSession = captureSession;

			// Start running.
			captureSession.StartRunning ();
		}

		partial void AddFrame (NSButton sender)
		{
			NSImage image;

			lock (this) {
				if (currentImage == null)
					return;

				var img = CIImage.FromImageBuffer (currentImage);
				Console.WriteLine (img);
				var imageRep = NSCIImageRep.FromCIImage (CIImage.FromImageBuffer (currentImage));
				image = new NSImage (imageRep.Size);
				image.AddRepresentation (imageRep);
			}
			movie.AddImage (image, new QTTime (1, 10), attrs);
			movie.CurrentTime = movie.Duration;
			movieView.NeedsDisplay = true;			
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

		public override bool ReadFromUrl (NSUrl url, string typeName, out NSError outError)
		{
			var loaded = QTMovie.FromUrl (url, out outError);
			if (loaded != null) {
				loaded.SetAttribute (NSNumber.FromBoolean (true), QTMovie.EditableAttribute);

				if (movie != null)
					movie.Dispose ();
				movie = loaded;
			}
			return loaded != null;
		}

		public override bool WriteToUrl (NSUrl url, string typeName, out NSError outError)
		{
			return movie.SaveTo (url.Path, new QTMovieSaveOptions () { Flatten = true }, out outError);
		}

		//
		// Not strictly necessary, but the movie might be very large, so we might
		// as well help the system by disposing before the GC kicks-in
		//
		protected override void Dispose (bool disposing)
		{
			if (captureSession != null)
				captureSession.StopRunning ();

			if (captureInput != null && captureInput.Device != null && captureInput.Device.IsOpen)
				captureInput.Device.Close ();

			if (disposing) {
				movie.Dispose ();
				captureSession.Dispose ();
				captureInput.Dispose ();
				decompressedVideo.Dispose ();
			}

			base.Dispose (disposing);
		}
	}
}

