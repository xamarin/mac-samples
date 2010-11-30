
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QTKit;
using System.IO;

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
#if false
			movieFileOutput.WillStartRecording += delegate {
				Console.WriteLine ("Will start recording");
			};
			movieFileOutput.DidStartRecording += delegate {
				Console.WriteLine ("Started Recording");
			};

			movieFileOutput.ShouldChangeOutputFile = (output, url, connections, reason) => {
				// Should change the file on error
				Console.WriteLine (reason.LocalizedDescription);
				return false;
			};
			movieFileOutput.MustChangeOutputFile += delegate(object sender, QTCaptureFileErrorEventArgs e) {
				Console.WriteLine ("Must change file due to error");
			};

			// These ones we care about, some notifications
			movieFileOutput.WillFinishRecording += delegate(object sender, QTCaptureFileErrorEventArgs e) {
				Console.WriteLine ("Will finish recording");
				InvokeOnMainThread (delegate {
					WillChangeValue ("Recording");
				});
			};
			movieFileOutput.DidFinishRecording += delegate(object sender, QTCaptureFileErrorEventArgs e) {
				Console.WriteLine ("Recorded {0} bytes duration {1}", movieFileOutput.RecordedFileSize, movieFileOutput.RecordedDuration);
				DidChangeValue ("Recording");
				if (e.Reason != null){
					NSAlert.WithError (e.Reason).BeginSheet (Window, delegate {});
					return;
				}
				var save = NSSavePanel.SavePanel;
				save.RequiredFileType = "mov";
				save.CanSelectHiddenExtension = true;
				save.Begin (code => {
					NSError err2;
					if (code == (int) NSPanelButtonType.Ok){
						var filename = save.Filename;
						NSFileManager.DefaultManager.Move (e.OutputFileURL.Path, filename, out err2);
					} else {
						NSFileManager.DefaultManager.Remove (e.OutputFileURL.Path, out err2);
					}
				});
			};
#endif
			session.AddOutput (movieFileOutput, out error);

			audioPreviewOutput = new QTCaptureAudioPreviewOutput ();
			session.AddOutput (audioPreviewOutput, out error);
			
			if (VideoDevices.Length > 0)
				SelectedVideoDevice = VideoDevices [0];
			
			if (AudioDevices.Length > 0)
				SelectedAudioDevice = AudioDevices [0];
			
			session.StartRunning ();
			
			// events: devices added/removed
			AddObserver (QTCaptureDevice.WasConnectedNotification, DevicesDidChange);
			AddObserver (QTCaptureDevice.WasDisconnectedNotification, DevicesDidChange);
			
			// events: connection format changes
			AddObserver (QTCaptureConnection.FormatDescriptionDidChangeNotification, FormatDidChange);
			AddObserver (QTCaptureConnection.FormatDescriptionWillChangeNotification, FormatWillChange);
				
			AddObserver (QTCaptureDevice.AttributeDidChangeNotification, AttributeDidChange);
			AddObserver (QTCaptureDevice.AttributeWillChangeNotification, AttributeWillChange);
		}
		
		List<NSObject> notifications = new List<NSObject> ();
		void AddObserver (NSString key, Action<NSNotification> notification)
		{
			notifications.Add (NSNotificationCenter.DefaultCenter.AddObserver (key, notification));
		}
		
		NSWindow Window { 
			get {
				return WindowControllers [0].Window;
			}
		}
					
		protected override void Dispose (bool disposing)
		{
			if (disposing){
				NSNotificationCenter.DefaultCenter.RemoveObservers (notifications);
				notifications = null;
			}
			base.Dispose (disposing);
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
		
#if false
		// Not available until we bind CIFilter
		
		string [] filterNames = new string [] {
			"CIKaleidoscope", "CIGaussianBlur",	"CIZoomBlur",
			"CIColorInvert", "CISepiaTone", "CIBumpDistortion",
			"CICircularWrap", "CIHoleDistortion", "CITorusLensDistortion",
			"CITwirlDistortion", "CIVortexDistortion", "CICMYKHalftone",
			"CIColorPosterize", "CIDotScreen", "CIHatchedScreen",
			"CIBloom", "CICrystallize", "CIEdges",
			"CIEdgeWork", "CIGloom", "CIPixellate",
		};
		
		NSString [] videoPreviewFilterDescriptions;
		NSString [] VideoPreviewFilterDescriptions {
			get {
			}
		}
#endif
				
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
		
		void DevicesDidChange (NSNotification notification)
		{
			RefreshDevices ();
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
						NSAlert.WithError (err).BeginSheet (Window, delegate {});
						return;
					}
					videoDeviceInput = new QTCaptureDeviceInput (value);
					if (!session.AddInput (videoDeviceInput, out err)){
						NSAlert.WithError (err).BeginSheet (Window, delegate {});
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
				if (audioDeviceInput != null){
					session.RemoveInput (audioDeviceInput);
					audioDeviceInput.Device.Close ();
					audioDeviceInput.Dispose ();
					audioDeviceInput = null;
				}
				
				if (value == null || SelectedVideoDeviceProvidesAudio)
					return;
				
				NSError err;
				
				// try to open
				if (!value.Open (out err)){
					NSAlert.WithError (err).BeginSheet (Window, delegate {});
					return;
				}
				
				audioDeviceInput = new QTCaptureDeviceInput (value);
				if (session.AddInput (audioDeviceInput, out err))
					return;
				
				NSAlert.WithError (err).BeginSheet (Window, delegate {});
				audioDeviceInput.Dispose ();
				audioDeviceInput = null;
				value.Close ();
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
		
		bool HasRecordingDevice {
			get {
				return videoDeviceInput != null || audioDeviceInput != null;
			}
		}
		
		[Export]
		bool Recording {
			get {
				return movieFileOutput.OutputFileUrl != null;	
			}
			set {
				if (value == Recording)
					return;
				if (value){
					movieFileOutput.RecordToOutputFile (NSUrl.FromFilename (Path.GetTempFileName () + ".mov"));
					Path.GetTempPath ();
				} else {
					movieFileOutput.RecordToOutputFile (null);
				}
			}
		}
		
		//
		// Notifications
		//
		void AttributeWillChange (NSNotification n)
		{
		}
		
		void AttributeDidChange (NSNotification n)
		{
		}
		
		void FormatWillChange (NSNotification n)
		{
		}
		
		void FormatDidChange (NSNotification n)
		{
		}

		public override string WindowNibName {
			get {
				return "QTRDocument";
			}
		}
	}
}

