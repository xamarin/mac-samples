
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using QTKit;
using System.IO;
using CoreImage;
using System.Text;

namespace QTRecorder
{
	public partial class QTRDocument : AppKit.NSDocument
	{
		QTCaptureSession session;
		QTCaptureDeviceInput videoDeviceInput, audioDeviceInput;
		QTCaptureMovieFileOutput movieFileOutput;
		QTCaptureAudioPreviewOutput audioPreviewOutput;

		QTCaptureDevice [] videoDevices, audioDevices;

		NSTimer audioLevelTimer;

		public override string WindowNibName {
			get {
				return "QTRDocument";
			}
		}

		// Link any co-dependent keys
		[Export("keyPathsForValuesAffectingHasRecordingDevice")]
		public static NSSet keyPathsForValuesAffectingHasRecordingDevice ()
		{
			return new NSSet ("SelectedVideoDevice", "SelectedAudioDevice");
		}

		[Export("keyPathsForValuesAffectingControllableDevice")]
		public static NSSet keyPathsForValuesAffectingControllableDevice ()
		{
			return new NSSet ("SelectedVideoDevice");
		}

		[Export("keyPathsForValuesAffectingSelectedVideoDeviceProvidesAudio")]
		public static NSSet keyPathsForValuesAffectingSelectedVideoDeviceProvidesAudio ()
		{
			return new NSSet ("SelectedVideoDevice");
		}

		[Export("keyPathsForValuesAffectingMediaFormatSummary")]
		public static NSSet keyPathsForValuesAffectingMediaFormatSummary()
		{
			return new NSSet ("SelectedVideoDevice", "SelectedAudioDevice");
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
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

			movieFileOutput.WillStartRecording += WillStartRecording;
			movieFileOutput.DidStartRecording += DidStartRecording;
			movieFileOutput.ShouldChangeOutputFile = ShouldChangeOutputFile;
			movieFileOutput.MustChangeOutputFile += MustChangeOutputFile;

			// These ones we care about, some notifications
			movieFileOutput.WillFinishRecording += WillFinishRecording;
			movieFileOutput.DidFinishRecording += DidFinishRecording;

			NSError error;
			session.AddOutput (movieFileOutput, out error);

			audioPreviewOutput = new QTCaptureAudioPreviewOutput ();
			audioPreviewOutput.Volume = 0;
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

			AddObserver (QTCaptureDevice.AttributeWillChangeNotification, AttributeWillChange);
			AddObserver (QTCaptureDevice.AttributeDidChangeNotification, AttributeDidChange);

			audioLevelTimer = NSTimer.CreateRepeatingScheduledTimer(0.1, UpdateAudioLevels);
		}

		#region Device selection

		void DevicesDidChange (NSNotification notification)
		{
			RefreshDevices ();
		}

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

		[Export("VideoDevices")]
		public QTCaptureDevice [] VideoDevices {
			get {
				if (videoDevices == null)
					RefreshDevices ();
				return videoDevices;
			}
			// TODO: Remove setter
			set {
				videoDevices = value;
			}
		}

		[Export("AudioDevices")]
		public QTCaptureDevice [] AudioDevices {
			get {
				if (audioDevices == null)
					RefreshDevices ();
				return audioDevices;
			}
		}

		[Export("SelectedVideoDevice")]
		public QTCaptureDevice SelectedVideoDevice { 
			get {
				return videoDeviceInput != null ? videoDeviceInput.Device : null;
			}
			set {
				if (videoDeviceInput != null) {
					// Remove the old device input from the session and close the device
					session.RemoveInput (videoDeviceInput);
					videoDeviceInput.Device.Close ();
					videoDeviceInput.Dispose ();
					videoDeviceInput = null;
				}

				if (value != null) {
					NSError err;
					if (!value.Open (out err)) {
						NSAlert.WithError (err).BeginSheet (Window, ()=>{});
						return;
					}

					// Create a device input for the device and add it to the session
					videoDeviceInput = new QTCaptureDeviceInput (value);

					if (!session.AddInput (videoDeviceInput, out err)){
						NSAlert.WithError (err).BeginSheet (Window, ()=>{});
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

		[Export("SelectedAudioDevice")]
		public QTCaptureDevice SelectedAudioDevice { 
			get {
				return audioDeviceInput != null ? audioDeviceInput.Device : null;
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
				if (!value.Open (out err)) {
					NSAlert.WithError (err).BeginSheet (Window, ()=>{});
					return;
				}

				// Create a device input for the device and add it to the session
				audioDeviceInput = new QTCaptureDeviceInput (value);
				if (session.AddInput (audioDeviceInput, out err))
					return;

				NSAlert.WithError (err).BeginSheet (Window, ()=>{});
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

		#endregion

		void WillStartRecording (object sender, QTCaptureFileUrlEventArgs e)
		{
			Console.WriteLine ("Will start recording");
		}

		void DidFinishRecording (object sender, QTCaptureFileErrorEventArgs e)
		{
			Console.WriteLine ("Recorded {0} bytes duration {1}", movieFileOutput.RecordedFileSize, movieFileOutput.RecordedDuration);
			DidChangeValue ("Recording");

			if (e.Reason != null) {
				NSAlert.WithError (e.Reason).BeginSheet (Window, () => {
				});
				return;
			}

			var save = NSSavePanel.SavePanel;
			save.AllowedFileTypes = new string[] { "mov" };
			save.CanSelectHiddenExtension = true;
			save.Begin (code => {
				NSError err2;
				if (code == (int)NSPanelButtonType.Ok)
					NSFileManager.DefaultManager.Move (e.OutputFileURL, save.Url, out err2);
				else
					NSFileManager.DefaultManager.Remove (e.OutputFileURL.Path, out err2);
			});
		}

		void DidStartRecording (object sender, QTCaptureFileUrlEventArgs e)
		{
			Console.WriteLine ("Started Recording");
		}

		void MustChangeOutputFile (object sender, QTCaptureFileErrorEventArgs e)
		{
			Console.WriteLine ("Must change file due to error");
		}

		bool ShouldChangeOutputFile (QTCaptureFileOutput captureOutput, NSUrl outputFileURL, QTCaptureConnection[] connections, NSError reason)
		{
			// Should change the file on error
			Console.WriteLine (reason.LocalizedDescription);
			return false;
		}

		void WillFinishRecording (object sender, QTCaptureFileErrorEventArgs e)
		{
			Console.WriteLine ("Will finish recording");
			InvokeOnMainThread (() => WillChangeValue ("Recording"));
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
		[Export("VideoPreviewFilterDescriptions")]
		NSDictionary [] VideoPreviewFilterDescriptions {
			get {
				return (from name in filterNames 
					select NSDictionary.FromObjectsAndKeys (
						new NSObject [] { new NSString (name), new NSString (CIFilter.FilterLocalizedName (name)) },
						new NSObject [] { filterNameKey, localizedFilterKey })).ToArray ();
			}
		}

		NSDictionary videoPreviewFilterDescription;

		[Export("VideoPreviewFilterDescription")]
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

		//
		// Binding support
		//
		[Export("AudioPreviewOutput")]
		public QTCaptureAudioPreviewOutput AudioPreviewOutput {
			get {
				return audioPreviewOutput;
			}
		}

		[Export("MediaFormatSummary")]
		public string MediaFormatSummary {
			get {
				var sb = new StringBuilder ();
				
				if (videoDeviceInput != null)
					foreach (var c in videoDeviceInput.Connections)
						sb.AppendFormat ("{0}\n", c.FormatDescription.LocalizedFormatSummary);
				if (audioDeviceInput != null)
					foreach (var c in audioDeviceInput.Connections)
						sb.AppendFormat ("{0}\n", c.FormatDescription.LocalizedFormatSummary);

				return sb.ToString ();
			}
		}

		[Export("HasRecordingDevice")]
		public bool HasRecordingDevice {
			get {
				return videoDeviceInput != null || audioDeviceInput != null;
			}
		}

		[Export("Recording")]
		public bool Recording {
			get {
				return movieFileOutput != null && movieFileOutput.OutputFileUrl != null;
			}
			set {
				if (value == Recording)
					return;
				if (value){
					var tempName = string.Format ("{0}.mov", Path.GetTempFileName ());
					movieFileOutput.RecordToOutputFile (NSUrl.FromFilename (tempName));
				} else {
					movieFileOutput.RecordToOutputFile (null);
				}
			}
		}

		// UI controls
		[Export("ControllableDevice")]
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

		[Export("DevicePlaying")]
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

		bool GetDeviceSpeed (Func<QTCaptureDeviceControlsSpeed?, bool> g)
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

		[Export("DeviceRewinding")]
		public bool DeviceRewinding {
			get {
				return GetDeviceSpeed (x => x < QTCaptureDeviceControlsSpeed.Stopped);
			}
			set {
				SetDeviceSpeed (value, QTCaptureDeviceControlsSpeed.FastReverse);
			}
		}

		[Export("DeviceFastForwarding")]
		public bool DeviceFastForwarding {
			get {
				return GetDeviceSpeed (x => x > QTCaptureDeviceControlsSpeed.Stopped);
			}
			set {
				SetDeviceSpeed (value, QTCaptureDeviceControlsSpeed.FastForward);
			}
		}

		// Notifications
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
				WillChangeValue ("MediaFormatSummary");
		}

		void FormatDidChange (NSNotification n)
		{
			var owner = ((QTCaptureConnection)n.Object).Owner;
			Console.WriteLine (owner);
			if (owner == videoDeviceInput || owner == audioDeviceInput)
				DidChangeValue ("MediaFormatSummary");
		}

		#region UI updating

		void UpdateAudioLevels(NSTimer timer)
		{
			throw new NotImplementedException ();
			// Get the mean audio level from the movie file output's audio connections

//			float totalDecibels = 0.0;
//
//			QTCaptureConnection *connection = nil;
//			NSUInteger i = 0;
//			NSUInteger numberOfPowerLevels = 0;	// Keep track of the total number of power levels in order to take the mean
//
//			for (i = 0; i < [[movieFileOutput connections] count]; i++) {
//				connection = [[movieFileOutput connections] objectAtIndex:i];
//
//				if ([[connection mediaType] isEqualToString:QTMediaTypeSound]) {
//					NSArray *powerLevels = [connection attributeForKey:QTCaptureConnectionAudioAveragePowerLevelsAttribute];
//					NSUInteger j, powerLevelCount = [powerLevels count];
//
//					for (j = 0; j < powerLevelCount; j++) {
//						NSNumber *decibels = [powerLevels objectAtIndex:j];
//						totalDecibels += [decibels floatValue];
//						numberOfPowerLevels++;
//					}
//				}
//			}
//
//			if (numberOfPowerLevels > 0) {
//				[audioLevelMeter setFloatValue:(pow(10., 0.05 * (totalDecibels / (float)numberOfPowerLevels)) * 20.0)];
//			} else {
//				[audioLevelMeter setFloatValue:0];
//			}
		}

		#endregion
	}
}

