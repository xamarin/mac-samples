//
// MonoMac.CFNetwork.Test.Models.GetStringRunner
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

namespace MonoMac.CFNetwork.Test.Models {

	public class GetStringRunner : SimpleTaskRunner {
		public class CompletedEventArgs : EventArgs {
			public Uri Uri {
				get;
				private set;
			}

			public string Text {
				get;
				private set;
			}

			public CompletedEventArgs (Uri uri, string text)
			{
				Uri = uri;
				Text = text;
			}
		}

		public event EventHandler<CompletedEventArgs> CompletedEvent;

		protected virtual void OnCompletedEvent (Uri uri, string text)
		{
			if (CompletedEvent != null)
				CompletedEvent (this, new CompletedEventArgs (uri, text));
		}

		public override bool CanSendResponse {
			get { return false; }
		}

		public override bool CanReportProgress {
			get { return false; }
		}

		protected override async Task<string> DoRun (HttpClient client, Uri uri,
		                                             CancellationToken cancellationToken)
		{
			var text = await client.GetStringAsync (uri);
			OnCompletedEvent (uri, text);
			return null;
		}
	}
}
