
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QTKit;
using MonoMac.CoreImage;
using MonoMac.CoreVideo;

namespace StillMotion
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		QTMovie movie;
		QTCaptureSession captureSession;
		QTCaptureDeviceInput captureInput;
		QTCaptureDecompressedVideoOutput decompressedVideo;
		
		public MainWindowController () : base("MainWindow")
		{
		}
		
		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();
			NSError err;
			
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
			decompressedVideo.DidOutputVideoFrame += NewVideoFrame;
			if (!captureSession.AddOutput (decompressedVideo, out err)){
				NSAlert.WithError (err).RunModal ();
				return;
			}
			
			// Activate preview
			captureView.CaptureSession = captureSession;
			
			// Start running.
			captureSession.StartRunning ();
		}
		
		volatile CVImageBuffer currentImage;
		
		// This is inovked on a separate thread
		void NewVideoFrame (object sender, QTCaptureVideoFrameEventArgs e)
		{
			Console.WriteLine ("Here");
			lock (this){
				currentImage = e.VideoFrame;
				if (currentImage == null)
					throw new Exception ();
			}
		}
		
		NSDictionary imageAttributes = NSDictionary.FromObjectAndKey (new NSString ("jpeg"), QTMovie.ImageCodecType);
			
		// Invoked when the user clicks "Add Frame"
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
			movie.AddImageForDuration (image, new QTTime (1, 10), imageAttributes);
			movie.CurrentTime = movie.Duration;
			movieView.NeedsDisplay = true;
		}

		//strongly typed window accessor
		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}
	}
}

