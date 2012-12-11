//
// AsyncTests.HttpClientTests.Addin.Server
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
using System.IO;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncTests.HttpClientTests.Addin
{
	using Framework;

	public class Server
	{
		static Server instance;
		HttpListener listener;
		Dictionary<string, RequestHandler> handlerByName;
		Dictionary<MethodInfo, RequestHandler> handlerByMethod;
		TaskCompletionSource<object> startTcs;
		TaskCompletionSource<object> stopTcs;
		Dictionary<int, Exception> exceptions;
		Assembly assembly;
		string prefix;
		bool running;
		Thread thread;
		Random random;

		Server (Assembly assembly, string prefix)
		{
			this.assembly = assembly;
			this.prefix = prefix;

			random = new Random ();
			exceptions = new Dictionary<int, Exception> ();

			handlerByName = new Dictionary<string, RequestHandler> ();
			handlerByMethod = new Dictionary<MethodInfo, RequestHandler> ();

			startTcs = new TaskCompletionSource<object> ();
			stopTcs = new TaskCompletionSource<object> ();

			listener = new HttpListener ();
			listener.Prefixes.Add (prefix);

			listener.AuthenticationSchemeSelectorDelegate = SelectAuthentication;

			running = true;
			thread = new Thread (() => ServerMain ());
			thread.Start ();
		}

		AuthenticationSchemes SelectAuthentication (HttpListenerRequest request)
		{
			var path = request.Url.AbsolutePath.Substring (1);
			if (!handlerByName.ContainsKey (path))
				return AuthenticationSchemes.Anonymous;

			var handler = handlerByName [path];
			return handler.Attribute.Authentication;
		}

		internal static void Log (string message, params object[] args)
		{
			Debug.WriteLine (string.Format (message, args), "Server");
		}

		void ServerMain ()
		{
			try {
				Register ();
				listener.Start ();
			} catch (Exception ex) {
				startTcs.SetException (ex);
				return;
			}

			startTcs.SetResult (null);

			while (running) {
				try {
					Run ().Wait ();
				} catch (Exception ex) {
					if (!running)
						break;
					Log ("SERVER EXCEPTION: {0} {1}", running, ex);
					throw;
				}
			}
			Log ("Server exiting.");
			stopTcs.SetResult (null);
		}

		Task Shutdown ()
		{
			running = false;
			listener.Stop ();
			listener.Close ();

			return stopTcs.Task;
		}

		public static Uri URI {
			get { return new Uri (instance.prefix); }
		}

		public static string Host {
			get {
				var uri = URI;
				if (uri.Port != 0)
					return string.Format ("{0}:{1}", uri.Host, uri.Port);
				else
					return uri.Host;
			}
		}

		public static bool IsLocal {
			get { return URI.IsLoopback; }
		}

		public static bool IsRunning {
			get { return instance != null; }
		}

		public static async Task Start (Assembly assembly, string prefix)
		{
			if (instance != null)
				return;
			instance = new Server (assembly, prefix);
			await instance.startTcs.Task;
			Log ("Started server: {0}", prefix);
		}

		public static async Task Stop ()
		{
			if (instance != null) {
				await instance.Shutdown ();
				instance = null;
			}
		}

		void Register ()
		{
			foreach (var type in assembly.GetTypes ()) {
				CheckType (type);
			}
		}

		void CheckType (Type type)
		{
			var attr = type.GetCustomAttribute<AsyncTestFixtureAttribute> ();
			if (attr == null)
				return;

			var bf = BindingFlags.Public | BindingFlags.Static;
			foreach (var method in type.GetMethods (bf)) {
				var mattr = method.GetCustomAttribute<RequestHandlerAttribute> ();
				if (mattr == null)
					continue;

				if (CheckMethod (method, mattr))
					continue;
				Log ("ERROR: Invalid method: {0}.{1}", type.FullName, method.Name);
			}

			foreach (var nested in type.GetNestedTypes ()) {
				CheckType (nested);
			}
		}

		bool CheckMethod (MethodInfo method, RequestHandlerAttribute attr)
		{
			if ((method.ReturnType != typeof (void)) &&
			    !method.ReturnType.Equals (typeof (Task)))
				return false;

			var pinfo = method.GetParameters ();
			if (pinfo.Length != 1)
				return false;

			if (!pinfo [0].ParameterType.Equals (typeof(ServerContext)))
				return false;

			var handler = new RequestHandler (method, attr);
			handlerByName.Add (handler.Name, handler);
			handlerByMethod.Add (method, handler);
			return true;
		}

		class RequestHandler
		{
			public RequestHandlerAttribute Attribute {
				get;
				private set;
			}

			public bool IsAsync {
				get;
				private set;
			}

			public RequestHandlerDelegate Handler {
				get;
				private set;
			}

			public AsyncRequestHandlerDelegate AsyncHandler {
				get;
				private set;
			}

			public MethodInfo Method {
				get {
					return IsAsync ? AsyncHandler.Method : Handler.Method;
				}
			}

			public string Name {
				get;
				private set;
			}

			public RequestHandler (MethodInfo method, RequestHandlerAttribute attr)
			{
				Attribute = attr;
				if (method.ReturnType == typeof (void)) {
					Handler = (RequestHandlerDelegate)Delegate.CreateDelegate (
						typeof (RequestHandlerDelegate), method);
				} else {
					IsAsync = true;
					AsyncHandler = (AsyncRequestHandlerDelegate)Delegate.CreateDelegate (
						typeof (AsyncRequestHandlerDelegate), method);
				}

				Name = method.DeclaringType.FullName.Replace ('.', '/') + '/' + method.Name;
			}

			public override string ToString ()
			{
				return string.Format ("[RequestHandler: {0}]", Name);
			}
		}

		async Task Run ()
		{
			var context = await listener.GetContextAsync ();
			var url = context.Request.Url;

			var path = url.AbsolutePath.Substring (1);
			if (path == string.Empty) {
				PrintIndex (context);
				context.Response.Close ();
				return;
			} else if (path.Equals ("favicon.ico")) {
				context.Response.StatusCode = 404;
				context.Response.Close ();
				return;
			}

			if (!handlerByName.ContainsKey (path)) {
				Log ("Unknown URL: {0}", path);
				Error (context.Response, "Invalid URL: '{0}'", path);
				return;
			}

			var handler = handlerByName [path];

			var sctx = new ServerContext (context);
			if (handler.IsAsync)
				ServerContext.Register (sctx);
			try {
				await sctx.Invoke (handler.Name, handler.Method);
			} catch (Exception ex) {
				if (!sctx.SetException (ex))
					Exception (context.Response, ex);
			} finally {
				try {
					context.Response.Close ();
				} catch {
					;
				}
			}
		}

		int CreateExceptionId ()
		{
			int id;
			do {
				id = random.Next ();
			} while (exceptions.ContainsKey (id));
			return id;
		}

		static readonly string ExceptionHeaderName = typeof (Server).FullName + ".Exception";

		void Exception (HttpListenerResponse response, Exception exception)
		{
			var id = CreateExceptionId ();
			exceptions [id] = exception;
			response.AddHeader (ExceptionHeaderName, id.ToString ());
			if (exception == null)
				return;
			response.StatusCode = 500;
			using (var writer = new StreamWriter (response.OutputStream)) {
				writer.WriteLine (string.Format ("EXCEPTION: {0}", exception));
			}
			response.Close ();
		}

		public static Exception GetException (HttpResponseMessage response)
		{
			var exception = ServerContext.GetException (response);
			if (exception != null)
				return exception;

			if (!response.Headers.Contains (ExceptionHeaderName))
				return null;
			try {
				var value = response.Headers.GetValues (ExceptionHeaderName).First ();
				var id = int.Parse (value);
				return instance.exceptions [id];
			} catch {
				return null;
			}
		}

		public static void CheckException (HttpResponseMessage response)
		{
			var exc = GetException (response);
			if (exc != null)
				throw exc;
		}

		void Error (HttpListenerResponse response, string format, params object[] args)
		{
			response.StatusCode = 500;
			using (var writer = new StreamWriter (response.OutputStream)) {
				writer.WriteLine (format, args);
			}
			response.Close ();
		}

		public static Uri GetUri (RequestHandlerDelegate dlg)
		{
			var handler = instance.handlerByMethod [dlg.Method];
			if (handler == null)
				throw new InvalidOperationException ();

			return GetUri (handler);
		}

		public static Uri GetAsyncUri (AsyncRequestHandlerDelegate dlg)
		{
			var handler = instance.handlerByMethod [dlg.Method];
			if (handler == null)
				throw new InvalidOperationException ();

			return GetUri (handler);
		}

		static Uri GetUri (RequestHandler handler)
		{
			return new Uri (new Uri (instance.prefix), handler.Name);
		}

		void PrintIndex (HttpListenerContext context)
		{
			context.Response.StatusCode = 200;
			using (var writer = new StreamWriter (context.Response.OutputStream)) {
				writer.WriteLine ("<html><head><title>Test Server</title><head><body>");
				writer.WriteLine ("<p>Registered tests:");
				writer.WriteLine ("<p><ul>");
				foreach (var handler in handlerByName.Keys) {
					var uri = new Uri (new Uri (prefix), handler);
					writer.WriteLine ("<li><a href=\"{0}\">{1}</a>",
					                  uri.AbsoluteUri, handler);
				}
				writer.WriteLine ("</ul>");
				writer.WriteLine ("</body></html>");
			}
		}
	}
}
