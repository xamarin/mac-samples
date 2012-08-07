//
// MonoMac.CFNetwork.Test.MainWindowController
//
// Authors:
//      Martin Baulig (martin.baulig@gmail.com)
//
// Copyright 2012 Xamarin Inc. (http://www.xamarin.com)
//
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test {

	using Models;
	using Views;

	public partial class MainWindowController : MonoMac.AppKit.NSWindowController {
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			var runner = AsyncTaskRunnerController.Instance;
			runner.StateChangedEvent += (sender, e) => {
				IsRunning = e.IsRunning;

				if (!e.IsRunning)
					ShowProgress = false;
				else
					ShowProgress = runner.CanReportProgress;
			};
			runner.ProgressChangedEvent += (sender, e) => {
				if (e.Total != null)
					CurrentProgress = 100.0 * e.Current / e.Total.Value;
			};
			runner.MessageEvent += (sender, e) => {
				StatusLabel.StringValue = e.Message;
			};

			getStringController = new GetStringViewController ();

			checkHeadersViewController = new CheckHeadersViewController ();
			downloadDataViewController = new DownloadDataViewController ();
			benchmarkViewController = new BenchmarkViewController ();
		}
		
		#endregion
		
		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		public bool AutoRedirect {
			get;
			set;
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			urlController = new URLViewController ();
			urlController.View.AutoresizingMask = NSViewResizingMask.WidthSizable;
			urlController.View.Frame = URLView.Bounds;
			URLView.AddSubview (urlController.View);
		}

		URLViewController urlController;
		GetStringViewController getStringController;
		DownloadDataViewController downloadDataViewController;
		CheckHeadersViewController checkHeadersViewController;
		BenchmarkViewController benchmarkViewController;
		NSViewController currentController;

		internal CheckHeadersViewController HeaderController {
			get { return checkHeadersViewController; }
		}

		internal BenchmarkViewController BenchmarkController {
			get { return benchmarkViewController; }
		}

		public void DownloadData ()
		{
			SwitchView (downloadDataViewController, "Download Data");
		}

		public void GetString ()
		{
			SwitchView (getStringController, "Get String");
		}

		public void CheckHeaders ()
		{
			SwitchView (checkHeadersViewController, "Check Headers");
		}

		public void Benchmark ()
		{
			SwitchView (benchmarkViewController, "Benchmark");
		}

		void SwitchView (NSViewController controller, string name)
		{
			if (currentController != null)
				currentController.View.RemoveFromSuperview ();

			currentController = controller;
			AsyncTaskRunnerController.Switch (((IAsyncViewController)controller).TaskRunner);
			View.AddSubview (controller.View);
			currentController.View.Frame = View.Bounds;
			currentController.View.AutoresizingMask = NSViewResizingMask.HeightSizable |
				NSViewResizingMask.WidthSizable;

			Window.Title = name;
		}

		bool isRunning;
		bool showProgress;
		double currentProgress;

		[Export("IsRunning")]
		public bool IsRunning {
			get { return isRunning; }
			set {
				WillChangeValue ("IsRunning");
				isRunning = value;
				View.NeedsDisplay = true;
				DidChangeValue ("IsRunning");
			}
		}

		[Export("ShowProgress")]
		public bool ShowProgress {
			get { return showProgress; }
			set {
				WillChangeValue ("ShowProgress");
				showProgress = value;
				DidChangeValue ("ShowProgress");
			}
		}

		[Export("CurrentProgress")]
		public double CurrentProgress {
			get { return currentProgress; }
			set {
				WillChangeValue ("CurrentProgress");
				currentProgress = value;
				View.NeedsDisplay = true;
				DidChangeValue ("CurrentProgress");
				if (!ShowProgress)
					ShowProgress = true;
			}
		}

		public void Load (string url)
		{
			AsyncTaskRunnerController.Instance.Run (new Uri (url));
		}

		public void Stop ()
		{
			AsyncTaskRunnerController.Instance.Cancel ();
		}
	}
}

