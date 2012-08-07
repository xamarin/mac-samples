//
// MonoMac.CFNetwork.Test.Models.DownloadDataRunner
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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace MonoMac.CFNetwork.Test.Models {
	public class DownloadDataRunner : SimpleTaskRunner {
		public override bool CanReportProgress {
			get { return true; }
		}

		public override bool CanSendResponse {
			get { return true; }
		}

		protected override async Task<string> DoRun (HttpClient client, Uri uri,
		                                             CancellationToken cancellationToken)
		{
			var response = await client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
			OnResponseEvent (response);

			var cts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);
			cts.Token.Register (() => response.Content.Dispose ());

			try {
				return await DoRun (response, cts.Token);
			} finally {
				cts.Dispose ();
				response.Dispose ();
			}
		}

		async Task<string> DoRun (HttpResponseMessage response, CancellationToken cancellationToken)
		{
			var length = response.Content.Headers.ContentLength;
			if (length == null) {
				var msg = "Response did not contain Content-Length!";
				if (AppDelegate.Instance.Settings.DownloadWithoutLength)
					OnMessageEvent (msg);
				else
					return msg;
			} else {
				OnProgressChangedEvent (0, length.Value);
			}

			cancellationToken.ThrowIfCancellationRequested ();

			if (!response.IsSuccessStatusCode)
				return string.Format ("ERROR: {0}", response.ReasonPhrase);

			var mystream = new DownloadStream (this, length);

			await response.Content.CopyToAsync (mystream);

			return string.Format ("Download complete ({0} bytes).", mystream.Position);
		}

		class DownloadStream : Stream {
			DownloadDataRunner runner;
			long? total;

			public DownloadStream (DownloadDataRunner runner, long? total)
			{
				this.runner = runner;
				this.total = total;
			}

			long position;

			#region implemented abstract members of System.IO.Stream
			public override void Flush ()
			{
				;
			}

			public override int Read (byte[] buffer, int offset, int count)
			{
				throw new InvalidOperationException ();
			}

			public override long Seek (long offset, SeekOrigin origin)
			{
				throw new InvalidOperationException ();
			}

			public override void SetLength (long value)
			{
				throw new InvalidOperationException ();
			}

			public override void Write (byte[] buffer, int offset, int count)
			{
				position += count;
				if (count > 0)
					runner.OnProgressChangedEvent (position, total);
				else
					runner.OnMessageEvent (string.Format ("Read {0} bytes.", position));
			}

			public override bool CanRead {
				get { return false; }
			}

			public override bool CanSeek {
				get { return false; }
			}

			public override bool CanWrite {
				get { return true; }
			}

			public override long Length {
				get { return total ?? 0; }
			}

			public override long Position {
				get { return position; }
				set {
					throw new InvalidOperationException ();
				}
			}
			#endregion
		}

	}
}
