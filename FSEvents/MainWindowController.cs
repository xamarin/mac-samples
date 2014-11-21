using System;
using System.IO;

using Foundation;
using AppKit;
using CoreServices;

namespace FSEventWatcher
{
	public partial class MainWindowController : AppKit.NSWindowController
	{
		public MainWindowController (IntPtr handle) : base (handle)
		{
		}
		
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
		}
		
		public MainWindowController () : base ("MainWindow")
		{
		}

		public new MainWindow Window {
			get { return (MainWindow)base.Window; }
		}

		public override void AwakeFromNib ()
		{
			WatchPathTextField.Changed += OnWatchPathChanged;
			StartStopButton.Activated += (sender, e) => ToggleFSEventStream ();

			LatencyStepper.Activated += (sender, e) => UpdateLatency ((int)LatencyStepper.IntegerValue);
			LatencyTextField.Changed += (sender, e) => UpdateLatency ((int)LatencyTextField.IntegerValue);
			
			UpdateLatency (5, reinitializeEventStream: false);

			var dirs = NSSearchPath.GetDirectories (NSSearchPathDirectory.DesktopDirectory, NSSearchPathDomain.User, true);
			if (dirs != null && dirs.Length > 0) {
				WatchPathTextField.StringValue = dirs [0];
				OnWatchPathChanged (null, null);
			}

			Window.MakeFirstResponder (EventStreamView);
		}

		void UpdateLatency (int value, bool reinitializeEventStream = true)
		{
			eventLatency = TimeSpan.FromSeconds (value);
			LatencyStepper.IntegerValue = value;
			LatencyTextField.IntegerValue = value;

			if (reinitializeEventStream) {
				InitializeFSEventStream ();
			}
		}

		string currentWatchPath;
		FSEventStream eventStream;
		FSEventDataSource eventsDataSource;
		bool eventStreamIsRunning;
		TimeSpan eventLatency;

		void OnWatchPathChanged (object sender, EventArgs e)
		{
			var newWatchPath = WatchPathTextField.StringValue.Trim ();

			if (newWatchPath != Path.DirectorySeparatorChar.ToString ()) {
				newWatchPath.TrimEnd (Path.DirectorySeparatorChar);
			}

			if (currentWatchPath != newWatchPath) {
				currentWatchPath = newWatchPath;
				InitializeFSEventStream ();
			}
		}

		void InitializeFSEventStream ()
		{
			if (eventStream != null) {
				eventStream.Events -= OnFSEventStreamEvents;
				eventStream.Dispose ();
				eventStream = null;
			}

			if (Directory.Exists (currentWatchPath)) {
				Console.WriteLine ("Creating new FSEventStream: latency={0}, path={1}", eventLatency, currentWatchPath);

				eventStreamIsRunning = false;

				eventStream = new FSEventStream (new [] { currentWatchPath },
					eventLatency, FSEventStreamCreateFlags.FileEvents);
				eventStream.Events += OnFSEventStreamEvents;
				eventStream.ScheduleWithRunLoop (NSRunLoop.Current);

				EventStreamView.DataSource = eventsDataSource = new FSEventDataSource ();

				ToggleFSEventStream ();
			}
		}

		void ToggleFSEventStream ()
		{
			if (eventStreamIsRunning) {
				eventStream.Stop ();
				eventStreamIsRunning = false;
				StartStopButton.Title = "Start";
			} else {
				eventStream.Start ();
				eventStreamIsRunning = true;
				StartStopButton.Title = "Stop";
			}
		}

		void OnFSEventStreamEvents (object sender, FSEventStreamEventsArgs e)
		{
			eventsDataSource.Add (e.Events);
			EventStreamView.ReloadData ();
			EventStreamView.ScrollRowToVisible (EventStreamView.RowCount - 1);
		}

		partial void ShowStream (NSObject sender)
		{
			if (eventStream != null) {
				eventStream.Show ();
			}
		}

		partial void FlushStreamAsync (NSObject sender)
		{
			if (eventStream != null) {
				Console.WriteLine ("Flush Async: {0}", eventStream.FlushAsync ());
			}
		}

		partial void FlushStreamSync (NSObject sender)
		{
			eventStream.FlushSync ();
		}

		partial void ChangeWatchPath (NSObject sender)
		{
			var openPanel = new NSOpenPanel {
				CanChooseDirectories = true,
				CanChooseFiles = false
			};

			openPanel.BeginSheet (Window, result => {
				if (result == 1) {
					WatchPathTextField.StringValue = openPanel.DirectoryUrl.Path;
					OnWatchPathChanged (null, null);
				}
			});
		}

		partial void RevealWatchPathInFinder (NSObject sender)
		{
			Reveal ("Finder");
		}
		
		partial void RevealWatchPathInTerminal (NSObject sender)
		{
			Reveal ("Terminal");
		}

		void Reveal (string app)
		{
			var path = WatchPathTextField.StringValue;

			if (!Directory.Exists (path)) {
				new NSAlert {
					AlertStyle = NSAlertStyle.Warning,
					MessageText = "Path does not exist or is not a directory:",
					InformativeText = String.IsNullOrWhiteSpace (path) ? "<no path given>" : path
				}.BeginSheet (Window);
				return;
			}

			NSWorkspace.SharedWorkspace.OpenFile (path, app);
		}
	}
}