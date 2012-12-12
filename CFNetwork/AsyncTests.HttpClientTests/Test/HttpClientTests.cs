//
// AsyncTests.HttpClientTests.Test.HttpClientTests
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
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

/*
 * Copied from System.Net.Http.Tests/HttpClientTests.cs
 * 
 */
namespace AsyncTests.HttpClientTests.Test {
	using Framework;
	using Addin;

	[HttpClientTestFixture (ReuseContext = false)]
	public class HttpClientTests {
		#region Default Settings

		[RequestHandler]
		public static void DefaultHandler (ServerContext ctx)
		{
			var request = ctx.Request;

			ctx.Expect (request.AcceptTypes, Is.Null, "#1");
			ctx.Expect (request.ContentLength64, Is.EqualTo (0), "#2");
			ctx.Expect (request.ContentType, Is.Null, "#3");
			ctx.Expect (request.Cookies.Count, Is.EqualTo (0), "#4");
			ctx.Expect (request.HasEntityBody, Is.False, "#5");
			ctx.Expect (request.Headers ["Host"], Is.EqualTo (Server.Host), "#6b");
			ctx.Expect (request.HttpMethod, Is.EqualTo ("GET"), "#7");
			ctx.Expect (request.IsAuthenticated, Is.False, "#8");
			ctx.Expect (request.IsLocal, Is.EqualTo (Server.IsLocal), "#9");
			ctx.Expect (request.IsSecureConnection, Is.False, "#10");
			ctx.Expect (request.IsWebSocketRequest, Is.False, "#11");
			ctx.Expect (request.KeepAlive, Is.True, "#12");
			ctx.Expect (request.ProtocolVersion, Is.EqualTo (HttpVersion.Version11), "#13");
			ctx.Expect (request.ServiceName, Is.Null, "#14");
			ctx.Expect (request.UrlReferrer, Is.Null, "#15");
			// FIXME: CFNetwork seems to set the UserAgent automatically.
			// context.Expect (request.UserAgent, Is.Null, "#16");
			ctx.Expect (request.UserLanguages, Is.Null, "#17");

			ctx.Response.StatusCode = 200;
		}

		[HttpClientTest]
		public async Task Default (HttpClientTestContext ctx,
		                           CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (DefaultHandler);

			var request = new HttpRequestMessage (HttpMethod.Get, uri);
			var response = await ctx.Client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.OK), "#100");
			ctx.Assert (await response.Content.ReadAsStringAsync (), Is.Empty, "#101");
		}

		[RequestHandler]
		public static void DefaultHandler_Version_1_0 (ServerContext ctx)
		{
			var request = ctx.Request;

			ctx.Expect (request.AcceptTypes, Is.Null, "#1");
			ctx.Expect (request.ContentLength64, Is.EqualTo (0), "#2");
			ctx.Expect (request.ContentType, Is.Null, "#3");
			ctx.Expect (request.Cookies.Count, Is.EqualTo (0), "#4");
			ctx.Expect (request.HasEntityBody, Is.False, "#5");

			/*
			 * CFNetwork seems to always send 'Connection' and 'User-Agent'.
			 */
			foreach (var obj in request.Headers) {
				if (obj.Equals ("Host"))
					continue;
				else if (obj.Equals ("Connection"))
					continue;
				else if (obj.Equals ("User-Agent"))
					continue;
				ctx.Expect (false, "#6");
			}
			ctx.Expect (request.Headers ["Host"], Is.EqualTo (Server.Host), "#6a");
			ctx.Expect (request.HttpMethod, Is.EqualTo ("GET"), "#7");
			ctx.Expect (request.IsAuthenticated, Is.False, "#8");
			ctx.Expect (request.IsLocal, Is.EqualTo (Server.IsLocal), "#9");
			ctx.Expect (request.IsSecureConnection, Is.False, "#10");
			ctx.Expect (request.IsWebSocketRequest, Is.False, "#11");
			ctx.Expect (request.KeepAlive, Is.False, "#12");
			ctx.Expect (request.ProtocolVersion, Is.EqualTo (HttpVersion.Version10), "#13");
			ctx.Expect (request.ServiceName, Is.Null, "#14");
			ctx.Expect (request.UrlReferrer, Is.Null, "#15");
			// FIXME: CFNetwork seems to set the UserAgent automatically.
			// Assert.IsNull (request.UserAgent, "#16");
			ctx.Expect (request.UserLanguages, Is.Null, "#17");

			ctx.Response.StatusCode = 200;
		}

		[HttpClientTest]
		public async Task Default_Version_1_0 (HttpClientTestContext ctx,
		                                       CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (DefaultHandler_Version_1_0);

			var request = new HttpRequestMessage (HttpMethod.Get, uri);
			request.Version = HttpVersion.Version10;

			var response = await ctx.Client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.OK), "#100");
			ctx.Assert (await response.Content.ReadAsStringAsync (), Is.Empty, "#101");
		}

		#endregion

		#region Keep-Alive

		[RequestHandler]
		public static void KeepAlive_Version1_0_Handler (ServerContext ctx)
		{
			var request = ctx.Request;

			var keepalive = request.Headers ["Keep-Alive"];
			var connection = request.Headers ["Connection"];

			ctx.Expect (keepalive, Is.Not.Null, "#1");
			ctx.Expect ((connection == null) ||
			            connection.Equals ("keep-alive", StringComparison.OrdinalIgnoreCase), "#2");
			ctx.Expect (request.KeepAlive, Is.True, "#3");
		}

		[HttpClientTest]
		public async Task KeepAlive_Version1_0 (HttpClientTestContext ctx,
		                                        CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (KeepAlive_Version1_0_Handler);

			var req = new HttpRequestMessage (HttpMethod.Get, uri);
			req.Version = HttpVersion.Version10;
			req.Headers.Add ("Keep-Alive", "false");

			var response = await ctx.Client.SendAsync (
				req, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);
		}

		#endregion


		[RequestHandler]
		public static void ClientHandlerSettings_Handler (ServerContext ctx)
		{
			var request = ctx.Request;

			ctx.Expect (request.AcceptTypes, Is.Null, "#1");
			ctx.Expect (request.ContentLength64, Is.EqualTo (0), "#2");
			ctx.Expect (request.ContentType, Is.Null, "#3");
			if (ctx.Expect (request.Cookies.Count, Is.EqualTo (1), "#4"))
				ctx.Expect (request.Cookies [0], Is.EqualTo (new Cookie ("mycookie", "vv")), "#4a");
			ctx.Expect (request.HasEntityBody, Is.False, "#5");

			/*
			 * CFNetwork seems to always send 'Connection' and 'User-Agent'.
			 */
			var allowed = new string[] {
				"Host", "Connection", "User-Agent", "Keep-Alive", "Cookie",
				"Accept-Encoding"
			};

			foreach (var obj in request.Headers) {
				var header = (string)obj;
				if (allowed.Contains (header))
					continue;
				ctx.Expect (false, "#6: unexpected header: {0}", header);
			}

			ctx.Expect (request.Headers ["Host"], Is.EqualTo (Server.Host), "#6a");
			ctx.Expect (request.Headers ["Accept-Encoding"], Is.EqualTo ("gzip"), "#6b");
			ctx.Expect (request.Headers ["Cookie"], Is.EqualTo ("mycookie=vv"), "#6c");
			ctx.Expect (request.HttpMethod, Is.EqualTo ("GET"), "#7");
			ctx.Expect (request.IsAuthenticated, Is.False, "#8");
			ctx.Expect (Server.IsLocal, request.IsLocal, Is.EqualTo (Server.IsLocal), "#9");
			ctx.Expect (request.IsSecureConnection, Is.False, "#10");
			ctx.Expect (request.IsWebSocketRequest, Is.False, "#11");
			ctx.Expect (request.KeepAlive, Is.True, "#12");
			ctx.Expect (request.ProtocolVersion, Is.EqualTo (HttpVersion.Version10), "#13");
			ctx.Expect (request.ServiceName, Is.Null, "#14");
			ctx.Expect (request.UrlReferrer, Is.Null, "#15");
			// ctx.Expect (request.UserAgent, Is.Null, "#16");
			ctx.Expect (request.UserLanguages, Is.Null, "#17");

			ctx.Response.StatusCode = 200;
		}

		[HttpClientTest]
		public async Task ClientHandlerSettings (HttpClientTestContext ctx,
		                                         CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (ClientHandlerSettings_Handler);

			ctx.Handler.AllowAutoRedirect = true;
			ctx.Handler.AutomaticDecompression = DecompressionMethods.GZip;
			ctx.Handler.MaxAutomaticRedirections = 33;
			ctx.Handler.MaxRequestContentBufferSize = 5555;
			ctx.Handler.PreAuthenticate = true;
			ctx.Handler.CookieContainer.Add (uri, new Cookie ("mycookie", "vv"));
			ctx.Handler.UseCookies = true;
			ctx.Handler.UseDefaultCredentials = true;

			if (Server.IsLocal) {
				ctx.Handler.Proxy = new WebProxy ("ee");
				ctx.Handler.UseProxy = true;
			}

			var request = new HttpRequestMessage (HttpMethod.Get, uri);
			request.Version = HttpVersion.Version10;
			request.Headers.Add ("Keep-Alive", "false");

			var response = await ctx.Client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.OK), "#100");
			ctx.Assert (await response.Content.ReadAsStringAsync (), Is.Empty, "#101");
		}

		[RequestHandler]
		public static void CustomHeaders_Handler (ServerContext ctx)
		{
			var request = ctx.Request;
			ctx.Expect (request.Headers ["aa"], Is.EqualTo ("vv"), "#1");
	
			var response = ctx.Response;
			response.Headers.Add ("rsp", "rrr");
			response.Headers.Add ("upgrade", "vvvvaa");
			response.Headers.Add ("Date", "aa");
			response.Headers.Add ("cache-control", "audio");
	
			response.StatusDescription = "test description";
			response.ProtocolVersion = HttpVersion.Version10;
			response.SendChunked = true;
			response.RedirectLocation = "w3.org";
		}

		[HttpClientTest]
		public async Task CustomHeaders (HttpClientTestContext ctx,
		                                 CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (CustomHeaders_Handler);

			var request = new HttpRequestMessage (HttpMethod.Get, uri);
			ctx.Expect (request.Headers.TryAddWithoutValidation ("aa", "vv"), Is.True, "#0");

			var response = await ctx.Client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.OK), "#101");
			ctx.Assert (await response.Content.ReadAsStringAsync (), Is.Empty, "#100");

			var headers = response.Headers;
				
			IEnumerable<string> values;
			if (ctx.Expect (headers.TryGetValues ("rsp", out values), Is.True, "#102"))
				ctx.Expect (values.First (), Is.EqualTo ("rrr"), "#102a");

			if (ctx.Expect (headers.TryGetValues ("Transfer-Encoding", out values), Is.True, "#103")) {
				// CFNetwork automatically combines all chunks.
				if (ctx.IsDefaultHandler) {
					ctx.Expect (values.First (), Is.EqualTo ("chunked"), "#103a");
					ctx.Expect (headers.TransferEncodingChunked, Is.True, "#103b");
				} else {
					ctx.Expect (values.First (), Is.EqualTo ("Identity"), "#103c");
					ctx.Expect (headers.TransferEncodingChunked, Is.Null, "#103d");
				}
			}

			if (ctx.Expect (headers.TryGetValues ("Date", out values), Is.True, "#104")) {
				ctx.Expect (values.Count (), Is.EqualTo (1), "#104b");
				// .NET overwrites Date, Mono does not
				if (!ctx.IsDefaultHandler)
					ctx.Expect (headers.Date, Is.Not.Null, "#104c");
			}

			ctx.Expect (headers.Upgrade.First (), Is.EqualTo (new ProductHeaderValue ("vvvvaa")), "#105");

			ctx.Expect (headers.CacheControl.Extensions.First ().Name, Is.EqualTo ("audio"), "#106");

			ctx.Expect (headers.Location.OriginalString, Is.EqualTo ("w3.org"), "#107");
			ctx.Expect (response.ReasonPhrase, Is.EqualTo ("test description"), "#110");
			// FIXME: .NET also returns 1.1 - bug ?
			if (ctx.IsDefaultHandler)
				ctx.Expect (response.Version, Is.EqualTo (HttpVersion.Version11), "#111");
		}

		[RequestHandler]
		public static void ContentHandler (ServerContext ctx)
		{
			ctx.Response.OutputStream.WriteByte (55);
			ctx.Response.OutputStream.WriteByte (75);
		}

		[HttpClientTest]
		public async Task Content (HttpClientTestContext ctx,
		                           CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (ContentHandler);
			var request = new HttpRequestMessage (HttpMethod.Get, uri);
			ctx.Assert (request.Headers.TryAddWithoutValidation ("aa", "vv"), Is.True, "#0");

			var response = await ctx.Client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.OK), "#101");
			ctx.Expect (await response.Content.ReadAsStringAsync (), Is.EqualTo ("7K"), "#100");

			var headers = response.Headers;

			IEnumerable<string> values;
			if (ctx.Expect (headers.TryGetValues ("Transfer-Encoding", out values), Is.True, "#102")) {
				// CFNetwork automatically combines all chunks.
				if (ctx.IsDefaultHandler) {
					ctx.Expect (values.First (), Is.EqualTo ("chunked"), "#102a");
					ctx.Expect (headers.TransferEncodingChunked, Is.True, "#102b");
				} else {
					ctx.Expect (values.First (), Is.EqualTo ("Identity"), "#102c");
					ctx.Expect (headers.TransferEncodingChunked, Is.Null, "#102d");
				}
			}
		}

		const int MaxResponseContentBufferSize_size = 8000;

		[RequestHandler]
		public static void MaxResponseContentBufferSize_Handler (ServerContext ctx)
		{
			var b = new byte[MaxResponseContentBufferSize_size];
			ctx.Response.OutputStream.Write (b, 0, b.Length);
		}

		[HttpClientTest]
		public async Task MaxResponseContentBufferSize (HttpClientTestContext ctx,
		                                                CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (MaxResponseContentBufferSize_Handler);
			ctx.Client.MaxResponseContentBufferSize = 1000;
			var request = new HttpRequestMessage (HttpMethod.Get, uri);

			var response = await ctx.Client.SendAsync (
				request, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.OK), "#101");
			var result = await response.Content.ReadAsStringAsync ();
			ctx.Expect (result.Length, Is.EqualTo (MaxResponseContentBufferSize_size), "#100");
		}

		[HttpClientTest]
		public async Task MaxResponseContentBufferSize_Error (HttpClientTestContext ctx,
		                                                      CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (MaxResponseContentBufferSize_Handler);
			ctx.Client.MaxResponseContentBufferSize = 1000;
			var request = new HttpRequestMessage (HttpMethod.Get, uri);

			try {
				var response = await ctx.Client.SendAsync (
					request, HttpCompletionOption.ResponseContentRead,
					cancellationToken).ConfigureAwait (false);
				ctx.AutoDispose (response);
				ctx.Assert (false, "#2");
			} catch (AggregateException e) {
				ctx.Assert (e.InnerException, Is.InstanceOfType (typeof(HttpRequestException)), "#3");
			} catch (HttpRequestException) {
				;
			}
		}
	}
}
