
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.QTKit;
using System.IO;
using MonoMac.CoreImage;
using System.Text;

namespace QTRecorder
{
	public partial class QTRDocument : MonoMac.AppKit.NSDocument
	{
		QTCaptureSession session;
		QTCaptureDeviceInput videoDeviceInput, audioDeviceInput;
		QTCaptureMovieFileOutput movieFileOutput;
		QTCaptureAudioPreviewOutput audioPreviewOutput;
		
		public QTRDocument ()
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
			captureView.WillDisplayImage = (view, image) => {
				if (videoPreviewFilterDescription == null)
					return image;
				var selectedFilter = (NSString) videoPreviewFilterDescription [filterNameKey];
				
				var filter = CIFilter.FromName (selectedFilter);
				filter.SetDefaults ();
				filter.SetValueForKey (image, CIFilterInputKey.Image);
				
				return (CIImage) filter.ValueForKey (CIFilterOutputKey.Image);
			};
		
			// Attach outputs to session
			movieFileOutput = new QTCaptureMovieFileOutput ();

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
				save.AllowedFileTypes = new string[] {"mov"};
				save.CanSelectHiddenExtension = true;
				save.Begin (code => {
					NSError err2;
					if (code == (int)NSPanelButtonType.Ok){
						NSFileManager.DefaultManager.Move (e.OutputFileURL, save.Url, out err2);
					} else {
						NSFileManager.DefaultManager.Remove (e.OutputFileURL.Path, out err2);
					}
				});
			};

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
		static NSString filterNameKey = new NSString ("filterName");
		static NSString localizedFilterKey = new NSString ("localizedName");
		
		// Creates descriptions that can be accessed with Key/Values
		[Export]
		NSDictionary [] VideoPreviewFilterDescriptions {
			get {
				return (from name in filterNames 
					select NSDictionary.FromObjectsAndKeys (
						new NSObject [] { new NSString (name), new NSString (CIFilter.FilterLocalizedName (name)) },
						new NSObject [] { filterNameKey, localizedFilterKey })).ToArray ();
			}
		}
				
		NSDictionary videoPreviewFilterDescription;
		[Export]
		NSDictionary VideoPreviewFilterDescription {
			get {
				return videoPreviewFilterDescription;
			}
			set {
				if (value == videoPreviewFilterDescription)
					return;
				videoPreviewFilterDescription = value;
				captureView.NeedsDisplay = true;
			}
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
		
		void DevicesDidChange (NSNotification notification)
		{
			RefreshDevices ();
		}
		
		//
		// Binding support
		//
		[Export]
		public QTCaptureAudioPreviewOutput AudioPreviewOutput {
			get {
				return audioPreviewOutput;
			}
		}
		
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
		
		[Export]
		public string MediaFormatSummary {
			get {
				var sb = new StringBuilder ();
				
				if (videoDeviceInput != null)
					foreach (var c in videoDeviceInput.Connections)
						sb.AppendFormat ("{0}\n", c.FormatDescription.LocalizedFormatSummary);
				if (audioDeviceInput != null)
					foreach (var c in videoDeviceInput.Connections)
						sb.AppendFormat ("{0}\n", c.FormatDescription.LocalizedFormatSummary);

				return sb.ToString ();
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
		
		[Export]
		public bool HasRecordingDevice {
		
			get {
				return videoDeviceInput != null || audioDeviceInput != null;
			}
		}
		
		[Export]
		public bool Recording {
			get {
				if (movieFileOutput == null) return false;
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
		
		// UI controls
		[Export]
		public QTCaptureDevice ControllableDevice {
			get {
				if (SelectedVideoDevice == null)
					return null;
				
				if (SelectedVideoDevice.AvcTransportControl == null)
					return null;
				if (SelectedVideoDevice.IsAvcTransportControlReadOnly)
					return null;
			
				return SelectedVideoDevice;
			}
		}
		
		[Export]
		public bool DevicePlaying {
			get {
				var device = ControllableDevice;
				if (device == null)
					return false;
				
				var controls = device.AvcTransportControl;
				if (controls == null)
					return false;
				
				return controls.Speed  == QTCaptureDeviceControlsSpeed.NormalForward && controls.PlaybackMode == QTCaptureDevicePlaybackMode.Playing;
			}

			set {
				var device = ControllableDevice;
				if (device == null)
					return;
				device.AvcTransportControl = new QTCaptureDeviceTransportControl () {
					Speed = value ? QTCaptureDeviceControlsSpeed.NormalForward : QTCaptureDeviceControlsSpeed.Stopped,
					PlaybackMode = QTCaptureDevicePlaybackMode.Playing
				};
			}
		}
		
		partial void StopDevice (NSObject sender)
		{
			var device = ControllableDevice;
			if (device == null)
				return;
			device.AvcTransportControl = new QTCaptureDeviceTransportControl () {
				Speed = QTCaptureDeviceControlsSpeed.Stopped,
				PlaybackMode = QTCaptureDevicePlaybackMode.NotPlaying
			};
		}

		bool GetDeviceSpeed (Func<QTCaptureDeviceControlsSpeed?,bool> g)
		{
				var device = ControllableDevice;
				if (device == null)
					return false;
				var control = device.AvcTransportControl;
				if (control == null)
					return false;
				return g (control.Speed);
		}
		
		void SetDeviceSpeed (bool value, QTCaptureDeviceControlsSpeed speed)
		{
				var device = ControllableDevice;
				if (device == null)
					return;
				var control = device.AvcTransportControl;
				if (control == null)
					return;
				control.Speed = value ? speed : QTCaptureDeviceControlsSpeed.Stopped;
				device.AvcTransportControl = control;
		}
		
		[Export]
		public bool DeviceRewinding {
			get {
				return GetDeviceSpeed (x => x < QTCaptureDeviceControlsSpeed.Stopped);
			}
			set {
				SetDeviceSpeed (value, QTCaptureDeviceControlsSpeed.FastReverse);
			}
		}
		
		[Export]
		public bool DeviceFastForwarding {
			get {
				return GetDeviceSpeed (x => x > QTCaptureDeviceControlsSpeed.Stopped);
			}
			set {
				SetDeviceSpeed (value, QTCaptureDeviceControlsSpeed.FastForward);
			}
		}
		
		// Link any co-dependent keys
		[Export]
		public static NSSet keyPathsForValuesAffectingHasRecordingDevice ()
		{
			// With MonoMac 0.4:
			return new NSSet (new string [] {"SelectedVideoDevice", "SelectedAudioDevice" });

			// With the new MonoMac:
			//return new NSSet ("SelectedVideoDevice", "SelectedAudioDevice");
		}
		
		[Export]
		public static NSSet keyPathsForValuesAffectingControllableDevice ()
		{
			// With MonoMac 0.4:
			return new NSSet (new string [] { "SelectedVideoDevice" });

			// With the new MonoMac:
			//return new NSSet ("SelectedVideoDevice");
		}
		
		[Export]
		public static NSSet keyPathsForValuesAffectingSelectedVideoDeviceProvidesAudio ()
		{
			// With MonoMac 0.4:
			return new NSSet (new string [] { "SelectedVideoDevice" });

			// With the new MonoMac:
			//return new NSSet ("SelectedVideoDevice");
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
			var owner = ((QTCaptureConnection)n.Object).Owner;
			Console.WriteLine (owner);
			if (owner == videoDeviceInput || owner == audioDeviceInput)
				WillChangeValue ("mediaFormatSummary");
		}
		
		void FormatDidChange (NSNotification n)
		{
			var owner = ((QTCaptureConnection)n.Object).Owner;
			Console.WriteLine (owner);
			if (owner == videoDeviceInput || owner == audioDeviceInput)
				DidChangeValue ("mediaFormatSummary");
		}

		public override string WindowNibName {
			get {
				return "QTRDocument";
			}
		}
	}
}

