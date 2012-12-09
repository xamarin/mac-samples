
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QTKit;
using MonoMac.CoreVideo;
using MonoMac.CoreImage;

namespace StillMotion
{
	public partial class StillMotionDocument : NSDocument
	{
		NSWindowController windowController;
		QTMovie movie;
		QTCaptureSession captureSession;
		QTCaptureDeviceInput captureInput;
		QTCaptureDecompressedVideoOutput decompressedVideo;
		volatile CVImageBuffer currentImage;
		
		// Called when created from unmanaged code
		public StillMotionDocument (IntPtr handle) : base (handle)
		{
		}

		public override string WindowNibName {
			get {

				return "StillMotion";
			}
		}
		
		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);

			// A reference to the window controller must be kept on the managed side
			// to keep the object from being GC'd so that the delegates below resolve.
			// Don't remove unless the framework is updated to track the reference.
			this.windowController = windowController;

			NSError err;
			
			windowController.Window.WillClose += delegate {
				if (captureSession != null)
					captureSession.StopRunning ();
				var dev = captureInput.Device;
				if (dev.IsOpen)
					dev.Close ();
			};
			
			// Create a movie, and store the information in memory on an NSMutableData
			movie = new QTMovie (new NSMutableData (1), out err);
			if (movie == null){
				NSAlert.WithError (err).RunModal ();
				return;
			}
			movieView.Movie = movie;
			
			// Find video device
			captureSession = new QTCaptureSession ();
			var device = QTCaptureDevice.GetDefaultInputDevice (QTMediaType.Video);
			if (!device.Open (out err)){
				NSAlert.WithError (err).RunModal ();
				return;
			}
			
			// Add device input
			captureInput = new QTCaptureDeviceInput (device);
			if (!captureSession.AddInput (captureInput, out err)){
				NSAlert.WithError (err).RunModal ();
				return;
			}
			
			// Create decompressor for video output, to get raw frames
			decompressedVideo = new QTCaptureDecompressedVideoOutput ();
			decompressedVideo.DidOutputVideoFrame += delegate(object sender, QTCaptureVideoFrameEventArgs e) {
				lock (this){
					currentImage = e.VideoFrame;
				}
			};
			if (!captureSession.AddOutput (decompressedVideo, out err)){
				NSAlert.WithError (err).RunModal ();
				return;
			}
			
			// Activate preview
			captureView.CaptureSession = captureSession;
			
			// Start running.
			captureSession.StartRunning ();
		}
		
		QTImageAttributes attrs = new QTImageAttributes () { CodecType = "jpeg" };
		
		partial void addFrame (NSObject sender)
		{
			NSImage image;
			
			lock (this){
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
		
		public override bool WriteToUrl (NSUrl absoluteUrl, string typeName, NSSaveOperationType saveOperation, NSUrl absoluteOriginalContentsUrl, out NSError outError)
		{
			return movie.SaveTo (absoluteUrl.Path, new QTMovieSaveOptions () { Flatten = true }, out outError);
		}
		
		public override bool ReadFromUrl (NSUrl absoluteUrl, string typeName, out NSError outError)
		{
			var loaded = QTMovie.FromUrl (absoluteUrl, out outError);
			if (loaded != null){
				loaded.SetAttribute (NSNumber.FromBoolean (true), QTMovie.EditableAttribute);
				
				if (movie != null)
					movie.Dispose ();
				movie = loaded;
			}
			return loaded != null;
		}
		
		//
		// Not strictly necessary, but the movie might be very large, so we might
		// as well help the system by disposing before the GC kicks-in
		//
		protected override void Dispose (bool disposing)
		{
			if (disposing){
				movie.Dispose ();
				captureSession.Dispose ();
				captureInput.Dispose ();
				decompressedVideo.Dispose ();
			}
			base.Dispose (disposing);
		}
	}
}

