//
// AsyncTests.HttpClientTests.Test.Authentication
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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace AsyncTests.HttpClientTests.Test {

	using Framework;
	using Addin;

	[HttpClientTestFixture (ReuseContext = false)]
	public class Authentication {
		[RequestHandler (Authentication = AuthenticationSchemes.Basic)]
		public static void BasicAuthentication_Handler (ServerContext ctx)
		{
			var user = ctx.ListenerContext.User;
			ctx.Expect (user, Is.Not.Null, "#1");
			ctx.Assert (user.Identity, Is.TypeOf (typeof (HttpListenerBasicIdentity)), "#2");

			var identity = (HttpListenerBasicIdentity)user.Identity;
			ctx.Expect (identity.Name, Is.EqualTo ("monkey"), "#3");
			ctx.Expect (identity.Password, Is.EqualTo ("banana"), "#4");
			ctx.Expect (identity.IsAuthenticated, Is.True, "#5");
		}

		[HttpClientTest]
		[TestWarning ("The default implementation keeps the connection open")]
		public async Task BasicAuthentication (HttpClientTestContext ctx,
		                                       CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (BasicAuthentication_Handler);
			ctx.Handler.Credentials = new NetworkCredential ("monkey", "banana");

			var response = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Expect (response.IsSuccessStatusCode, Is.True, "#100");
		}

		[HttpClientTest]
		[TestWarning ("The default implementation keeps the connection open")]
		public async Task PreAuthenticate (HttpClientTestContext ctx,
		                                   CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (BasicAuthentication_Handler);
			ctx.Handler.Credentials = new NetworkCredential ("monkey", "banana");
			ctx.Handler.PreAuthenticate = true;

			var response = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Expect (response.IsSuccessStatusCode, Is.True, "#100");
		}

		[WebDav]
		[HttpClientTest (HttpClientTestTarget.Custom)]
		public async Task DigestAuthentication (HttpClientTestContext ctx,
		                                        CancellationToken cancellationToken)
		{
			var config = ctx.GetConfiguration<WebDavConfiguration> ();

			var uri = new Uri (config.Server);
			var cache = new CredentialCache ();
			cache.Add (uri, "Digest", new NetworkCredential (config.UserName, config.Password));
			ctx.Handler.Credentials = cache;

			var response = await ctx.Client.GetAsync (
				uri, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response);

			ctx.Expect (response.IsSuccessStatusCode, Is.True, "#100");
		}

		[WebDav]
		[NotWorking]
		[HttpClientTest (HttpClientTestTarget.Default)]
		public Task DigestAuthentication_Default (HttpClientTestContext ctx,
		                                          CancellationToken cancellationToken)
		{
			return DigestAuthentication (ctx, cancellationToken);
		}

		[WebDav]
		[HttpClientTest (HttpClientTestTarget.Custom)]
		public async Task DigestAuthentication2 (HttpClientTestContext ctx,
		                                         CancellationToken cancellationToken)
		{
			var config = ctx.GetConfiguration<WebDavConfiguration> ();

			var random = new Random ();
			var text = random.Next ().ToString ("x");

			var uri = new Uri (config.Server);
			var cache = new CredentialCache ();
			cache.Add (uri, "Basic", new NetworkCredential ("invalid", text));
			cache.Add (uri, "Digest", new NetworkCredential (config.UserName, config.Password));
			ctx.Handler.Credentials = cache;

			var response = await ctx.Client.GetAsync (
				uri, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response);

			ctx.Expect (response.IsSuccessStatusCode, Is.True, "#100");
		}

		[WebDav]
		[HttpClientTest (HttpClientTestTarget.Custom)]
		public async Task DigestPreAuthentication (HttpClientTestContext ctx,
		                                           CancellationToken cancellationToken)
		{
			var config = ctx.GetConfiguration<WebDavConfiguration> ();

			var random = new Random ();
			var text = random.Next ().ToString ("x");

			var uri = new Uri (config.Server);
			var cache = new CredentialCache ();
			cache.Add (uri, "Basic", new NetworkCredential ("invalid", text));
			cache.Add (uri, "Digest", new NetworkCredential (config.UserName, config.Password));
			ctx.Handler.Credentials = cache;
			ctx.Handler.PreAuthenticate = true;

			var response = await ctx.Client.GetAsync (
				uri, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response);

			ctx.Expect (response.IsSuccessStatusCode, Is.True, "#100");

			var response2 = await ctx.Client.GetAsync (
				uri, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response2);

			ctx.Expect (response2.IsSuccessStatusCode, Is.True, "#101");
		}
	}
}
