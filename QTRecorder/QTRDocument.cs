
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
		QTCaptureSession session;
		QTCaptureDeviceInput videoDeviceInput, audioDeviceInput;
		QTCaptureMovieFileOutput movieFileOutput;
		QTCaptureAudioPreviewOutput audioPreviewOutput;
		
		public QTRDocument (IntPtr handle) : base(handle)
		{
		}

		[Export("initWithCoder:")]
		public QTRDocument (NSCoder coder) : base(coder)
		{
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			NSError error;
			base.WindowControllerDidLoadNib (windowController);
			
			// Create session
			session = new QTCaptureSession ();
			
			// Attach preview to session
			captureView.CaptureSession = session;
		
			// Attach outputs to session
			movieFileOutput = new QTCaptureMovieFileOutput ();
			session.AddOutput (movieFileOutput, out error);
			
			audioPreviewOutput = new QTCaptureAudioPreviewOutput ();
			session.AddOutput (audioPreviewOutput, out error);
			
			if (VideoDevices.Length > 0)
				SelectedVideoDevice = VideoDevices [0];
			
			if (AudioDevices.Length > 0)
				SelectedAudioDevice = AudioDevices [0];
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
		
		QTCaptureDevice [] videoDevices, audioDevices;
		void RefreshDevices ()
		{
			WillChangeValue ("VideoDevices");
			videoDevices = QTCaptureDevice.GetInputDevices (QTMediaType.Video).Concat (QTCaptureDevice.GetInputDevices (QTMediaType.Muxed)).ToArray ();
			DidChangeValue ("VideoDevices");
			
			WillChangeValue ("AudioDevices");
			audioDevices = QTCaptureDevice.GetInputDevices (QTMediaType.Sound);
			DidChangeValue ("AudioDevices");
			
			if (!videoDevices.Contains (SelectedVideoDevice))
				SelectedVideoDevice = null;
			
			if (!audioDevices.Contains (SelectedAudioDevice))
				SelectedAudioDevice = null;
		}
		
		//
		// Binding support
		//
		[Export]
		public QTCaptureDevice [] VideoDevices {
			get {
				if (videoDevices == null)
					RefreshDevices ();
				return videoDevices;
			}
			set {
				videoDevices = value;
			}
		}
		
		[Export]
		public QTCaptureDevice SelectedVideoDevice { 
			get {
				return videoDeviceInput == null ? null : videoDeviceInput.Device;
			}
			set {
				if (videoDeviceInput != null){
					session.RemoveInput (videoDeviceInput);
					videoDeviceInput.Device.Close ();
					videoDeviceInput.Dispose ();
					videoDeviceInput = null;
				}
				
				if (value != null){
					NSError err;
					if (!value.Open (out err)){
						NSAlert.WithError (err).BeginSheet (WindowControllers [0].Window, delegate {});
						return;
					}
					videoDeviceInput = new QTCaptureDeviceInput (value);
					if (!session.AddInput (videoDeviceInput, out err)){
						NSAlert.WithError (err).BeginSheet (WindowControllers [0].Window, delegate {});
						videoDeviceInput.Dispose ();
						videoDeviceInput = null;
						value.Close ();
						return;
					}
				}
				// If the video device provides audio, do not use a new audio device
				if (SelectedVideoDeviceProvidesAudio)
					SelectedAudioDevice = null;
			}
		}
		
		[Export]
		public QTCaptureDevice [] AudioDevices {
			get {
				if (audioDevices == null)
					RefreshDevices ();
				return audioDevices;
			}
		}
		
		[Export]
		public QTCaptureDevice SelectedAudioDevice { 
			get {
				if (audioDeviceInput == null)
					return null;
				return audioDeviceInput.Device;
			}
			set {
				if (videoDeviceInput != null){
					
				}
			}
		}
		
		bool SelectedVideoDeviceProvidesAudio {
			get {
				var x = SelectedVideoDevice;
				if (x == null)
					return false;
				return x.HasMediaType (QTMediaType.Muxed) || x.HasMediaType (QTMediaType.Sound);
			}
		}
		public override string WindowNibName {
			get {
				return "QTRDocument";
			}
		}
	}
}

