//
// MonoMac.CFNetwork.Test.Models.BenchmarkRunner
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
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MonoMac.Foundation;
using MonoMac.CFNetwork;

namespace MonoMac.CFNetwork.Test.Models {

	using Views;

	public class BenchmarkRunner : AsyncTaskRunner {
		public override bool CanReportProgress {
			get { return false; }
		}

		public override bool CanSendResponse {
			get { return false; }
		}

		public event EventHandler<MessageEventArgs> DetailedMessageEvent;

		protected virtual void OnDetailedMessageEvent (object sender, MessageEventArgs args)
		{
			if (DetailedMessageEvent != null)
				DetailedMessageEvent (sender, args);
		}

		void Write (string message, params object[] args)
		{
			Write (string.Format (message, args));
		}

		void Write (string message)
		{
			OnDetailedMessageEvent (this, new MessageEventArgs (message));
		}

		void WriteLine (string message, params object[] args)
		{
			Write (string.Format (message, args) + Environment.NewLine);
		}

		void WriteLine (string message)
		{
			Write (message + Environment.NewLine);
		}

		BenchmarkViewController ViewController {
			get { return AppDelegate.Instance.MainWindowController.BenchmarkController; }
		}

		Uri uri;
		int repeatCount;
		DateTime startTime;
		BenchmarkViewController.ModeTag mode;
		CancellationTokenSource cts;

		internal override async Task<string> Run (Uri uri, CancellationToken cancellationToken)
		{
			if (cts != null)
				throw new InvalidOperationException ();

			this.uri = uri;
			this.mode = (BenchmarkViewController.ModeTag)ViewController.Mode;
			this.repeatCount = ViewController.RepeatCount;
			cts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);

			var message = string.Format (
				"Running {0} iterations of {1} ({2}).", repeatCount, mode, uri);
			OnMessageEvent (message);
			WriteLine (message);

			startTime = DateTime.Now;

			try {
				await DoRun ();
				var time = DateTime.Now - startTime;
				var completed = string.Format ("Benchmark completed in {0}.", time);
				WriteLine (completed);
				WriteLine (string.Empty);
				return message;
			} finally {
				cts.Dispose ();
				cts = null;
			}
		}

		protected override HttpClient CreateClient (bool defaultHandler)
		{
			var client = base.CreateClient (defaultHandler);
			cts.Token.Register (() => client.CancelPendingRequests ());
			return client;
		}

		async Task DoRun ()
		{
			await SingleRequest ();
			await SerialRequests (true);
			await SerialRequests (false);
			await SerialRequestsOnWorker ();
			// await ParallelRequests (true);
			// await ParallelRequests (false);
		}

		async Task SingleRequest ()
		{
			var start = DateTime.Now;
			using (var client = CreateClient (false))
				await CreateTask (client);
			WriteLine ("Single request done in {0}.", DateTime.Now - start);
		}

		string GetHandlerName (bool defaultHandler)
		{
			return defaultHandler ? "default" : "CFNetwork";
		}

		async Task SerialRequests (bool defaultHandler)
		{
			WriteLine ("Starting {0} serial requests.", GetHandlerName (defaultHandler));

			var start = DateTime.Now;

			for (int i = 0; i < repeatCount; i++) {
				using (var client = CreateClient (defaultHandler))
					await CreateTask (client);
			}

			var time = DateTime.Now - start;
			var perRequest = (int)(time.TotalMilliseconds / repeatCount);

			WriteLine ("Serial {0} requests done in {1} ({2}ms/request).",
			           GetHandlerName (defaultHandler), time, perRequest);
		}

		async Task SerialRequestsOnWorker ()
		{
			WriteLine ("Starting serial requests on worker thread.");

			var worker = new WorkerThread ();
			var handler = new MessageHandler (worker);
			var client = new HttpClient (handler, true);

			var start = DateTime.Now;

			try {
				for (int i = 0; i < repeatCount; i++) {
					await CreateTask (client);
				}
			} finally {
				client.Dispose ();
				worker.Stop ();
			}

			var time = DateTime.Now - start;
			var perRequest = (int)(time.TotalMilliseconds / repeatCount);

			WriteLine ("Serial requests on worker thread done in {0} ({1}ms/request).",
			           time, perRequest);
		}


		async Task ParallelRequests (bool defaultHandler)
		{
			WriteLine ("Starting {0} parallel requests.", GetHandlerName (defaultHandler));

			var start = DateTime.Now;

			var clients = new HttpClient [repeatCount];
			var task = new Task [repeatCount];

			for (int i = 0; i < task.Length; i++) {
				clients [i] = CreateClient (defaultHandler);
				task [i] = CreateTask (clients [i]);
			}

			await Task.WhenAll (task);

			var time = DateTime.Now - start;
			var perRequest = (int)(time.TotalMilliseconds / repeatCount);

			WriteLine ("Parallel {0} requests done in {1} ({2}ms/request).",
			           GetHandlerName (defaultHandler), time, perRequest);
		}

		Task CreateTask (HttpClient client)
		{
			switch (mode) {
			case BenchmarkViewController.ModeTag.GetByteArray:
				return client.GetByteArrayAsync (uri);

			case BenchmarkViewController.ModeTag.CheckHeaders:
				return CheckHeaders (client);

			default:
				throw new InvalidOperationException ();
			}
		}

		async Task CheckHeaders (HttpClient client)
		{
			var request = new HttpRequestMessage (HttpMethod.Get, uri);
			var response = await client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
			response.Dispose ();
		}
	}
}
