//
// MonoMac.CFNetwork.Test.Models.AsyncTaskRunnerController
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

namespace MonoMac.CFNetwork.Test.Models {

	public class AsyncTaskRunnerController : AsyncTaskRunner {
		public class StateChangedEventArgs : EventArgs {
			public bool IsRunning {
				get;
				private set;
			}

			public StateChangedEventArgs (bool isRunning)
			{
				IsRunning = isRunning;
			}
		}

		public event EventHandler<StateChangedEventArgs> StateChangedEvent;

		void OnStateChanged (bool running)
		{
			if (StateChangedEvent != null)
				StateChangedEvent (this, new StateChangedEventArgs (running));
		}

		public static readonly AsyncTaskRunnerController Instance;
		public static readonly CheckHeadersRunner CheckHeaders;
		public static readonly DownloadDataRunner DownloadData;
		public static readonly GetStringRunner GetString;
		public static readonly BenchmarkRunner Benchmark;

		static AsyncTaskRunnerController ()
		{
			CheckHeaders = new CheckHeadersRunner ();
			DownloadData = new DownloadDataRunner ();
			GetString = new GetStringRunner ();
			Benchmark = new BenchmarkRunner ();
			Instance = new AsyncTaskRunnerController ();
		}

		AsyncTaskRunner current;

		AsyncTaskRunnerController ()
		{
			SetupEvents (CheckHeaders);
			SetupEvents (DownloadData);
			SetupEvents (GetString);
			SetupEvents (Benchmark);
		}

		void SetupEvents (AsyncTaskRunner runner)
		{
			runner.OperationStartedEvent += (sender, e) => {
				OnOperationStartedEvent (sender);
			};
			runner.MessageEvent += (sender, e) => {
				OnMessageEvent (sender, e); 
			};
			runner.ResponseEvent += (sender, e) => {
				OnResponseEvent (sender, e);
			};
			runner.ProgressChangedEvent += (sender, e) => {
				OnProgressChangedEvent (sender, e);
			};
		}

		public static void Switch (AsyncTaskRunner runner)
		{
			Instance.current = runner;
		}

		public override bool CanReportProgress {
			get { return current.CanReportProgress; }
		}

		public override bool CanSendResponse {
			get { return current.CanSendResponse; }
		}

		bool running;
		CancellationTokenSource cts;

		public async Task Run (Uri uri)
		{
			if (running)
				return;

			cts = new CancellationTokenSource ();
			running = true;

			try {
				OnStateChanged (true);
				OnOperationStartedEvent (this);

				OnMessageEvent ("Loading {0}", uri);
				var result = await Run (uri, cts.Token);
				if (result != null)
					OnMessageEvent (result);
				else
					OnMessageEvent ("Loaded {0}.", uri);
			} catch (TaskCanceledException) {
				OnMessageEvent ("Cancelled!");
			} catch (Exception ex) {
				OnMessageEvent ("ERROR: {0}", ex.Message);
				Console.WriteLine ("ERROR: {0}", ex);
			} finally {
				running = false;
				OnStateChanged (false);
				cts.Dispose ();
				cts = null;
			}
		}

		public void Cancel ()
		{
			if (cts == null)
				return;
			try {
				cts.Cancel ();
			} catch {
				;
			}
		}

		internal override Task<string> Run (Uri uri, CancellationToken cancellationToken)
		{
			return current.Run (uri, cancellationToken);
		}
	}
}
