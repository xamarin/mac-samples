//
// MonoMac.CFNetwork.Test.Models.AsyncTaskRunner
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
using MonoMac.CFNetwork;

namespace MonoMac.CFNetwork.Test.Models {

	public abstract class AsyncTaskRunner {
		protected HttpClient CreateClient ()
		{
			return CreateClient (false);
		}

		protected virtual HttpClient CreateClient (bool defaultHandler)
		{
			var handler = defaultHandler ? new HttpClientHandler () : new MessageHandler ();
			var client = new HttpClient (handler, true);
			ApplySettings (client, handler);
			return client;
		}

		protected virtual void ApplySettings (HttpClient client, HttpClientHandler handler)
		{
			var settings = AppDelegate.StaticSettings;

			handler.AllowAutoRedirect = settings.AutoRedirect;

			if (!settings.UseRelativeURL)
				client.BaseAddress = null;
			else
				client.BaseAddress = new Uri (settings.RelativeURL);

			if (settings.UseAuthentication) {
				handler.Credentials = new NetworkCredential (
					settings.UserName, settings.Password);
			} else {
				handler.Credentials = null;
			}
		}

		internal abstract Task<string> Run (Uri uri, CancellationToken cancellationToken);

		public event EventHandler OperationStartedEvent;

		protected void OnOperationStartedEvent (object sender)
		{
			if (OperationStartedEvent != null)
				OperationStartedEvent (sender, null);
		}

		public class MessageEventArgs : EventArgs {
			public string Message {
				get;
				private set;
			}

			public MessageEventArgs (string message)
			{
				Message = message;
			}
		}

		public event EventHandler<MessageEventArgs> MessageEvent;

		protected virtual void OnMessageEvent (string message)
		{
			OnMessageEvent (this, new MessageEventArgs (message));
		}

		protected virtual void OnMessageEvent (string message, params object[] args)
		{
			OnMessageEvent (this, new MessageEventArgs (string.Format (message, args)));
		}

		protected virtual void OnMessageEvent (object sender, MessageEventArgs args)
		{
			if (MessageEvent != null)
				MessageEvent (sender, args);
		}

		public abstract bool CanReportProgress {
			get;
		}

		public class ProgressChangedEventArgs : EventArgs {
			public long Current {
				get;
				private set;
			}

			public long? Total {
				get;
				private set;
			}

			public ProgressChangedEventArgs (long current, long? total)
			{
				Current = current;
				Total = total;
			}
		}

		public event EventHandler<ProgressChangedEventArgs> ProgressChangedEvent;

		protected virtual void OnProgressChangedEvent (long current, long? total)
		{
			OnProgressChangedEvent (this, new ProgressChangedEventArgs (current, total));
		}

		protected virtual void OnProgressChangedEvent (object sender, ProgressChangedEventArgs args)
		{
			if (ProgressChangedEvent != null)
				ProgressChangedEvent (this, args);
		}

		public abstract bool CanSendResponse {
			get;
		}

		public class ResponseEventArgs : EventArgs {
			public HttpResponseMessage Response {
				get;
				private set;
			}

			public ResponseEventArgs (HttpResponseMessage response)
			{
				Response = response;
			}
		}

		public event EventHandler<ResponseEventArgs> ResponseEvent;

		protected virtual void OnResponseEvent (HttpResponseMessage response)
		{
			OnResponseEvent (this, new ResponseEventArgs (response));
		}

		protected virtual void OnResponseEvent (object sender, ResponseEventArgs args)
		{
			if (ResponseEvent != null)
				ResponseEvent (this, args);
		}
	}
}
