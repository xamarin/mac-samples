//
// AsyncTests.HttpClientTests.ServerContext
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AsyncTests.Framework;

namespace AsyncTests.HttpClientTests.Addin {

	public class ServerContext : TestContext {
		public HttpListenerContext ListenerContext {
			get;
			private set;
		}

		public HttpListenerRequest Request {
			get { return ListenerContext.Request; }
		}

		public HttpListenerResponse Response {
			get { return ListenerContext.Response; }
		}

		#region private methods

		int id;
		Exception error;
		static Random random;
		static ConcurrentDictionary<int,ServerContext> ctxById;

		static ServerContext ()
		{
			random = new Random ();
			ctxById = new ConcurrentDictionary<int, ServerContext> ();
		}

		static readonly string HeaderName = typeof (Server).FullName + ".Context";

		internal static void Register (ServerContext context)
		{
			for (int i = 0; i < 100; i++) {
				var id = random.Next ();
				if ((id == 0) || ctxById.ContainsKey (id))
					continue;

				context.id = id;
				ctxById [id] = context;

				var response = context.ListenerContext.Response;
				response.AddHeader (HeaderName, id.ToString ());
				return;
			}
			throw new InvalidOperationException ();
		}

		internal bool SetException (Exception error)
		{
			if (id == 0)
				return false;
			this.error = error;
			return true;
		}

		internal static Exception GetException (HttpResponseMessage response)
		{
			if (!response.Headers.Contains (HeaderName))
				return null;

			try {
				var value = response.Headers.GetValues (HeaderName).First ();
				var id = int.Parse (value);
				var context = ctxById [id];
				if (context == null)
					return null;
				return context.error;
			} catch {
				return null;
			}
		}

		internal ServerContext (HttpListenerContext context)
			: base (null, null)
		{
			this.ListenerContext = context;
		}

		public override void Log (string message, params object[] args)
		{
			Server.Log (message, args);
		}

		internal Task Invoke (string name, MethodInfo method)
		{
			return Invoke_internal (name, method, null, new object[] { this });
		}

		#endregion
	}
}
