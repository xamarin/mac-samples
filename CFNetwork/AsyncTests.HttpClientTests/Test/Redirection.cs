//
// AsyncTests.HttpClientTests.Test.Redirection
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
using System.Text;
using System.Diagnostics;
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
	public class Redirection {
		public const string Text = "Hello World";

		[RequestHandler]
		public static void HandleRequest (ServerContext ctx)
		{
			var simple = Server.GetUri (Simple.SimpleHtml_Handler);
			ctx.Response.StatusCode = (int)HttpStatusCode.Moved;
			ctx.Response.RedirectLocation = simple.AbsolutePath;
		}

		[HttpClientTest]
		public async Task TestRedirection (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (HandleRequest);
			ctx.Handler.AllowAutoRedirect = true;
			var result = await ctx.Client.GetStringAsync (uri).ConfigureAwait (false);
			ctx.Assert (result, Is.EqualTo (Text));
		}

		[HttpClientTest]
		[ExpectedException (typeof (HttpRequestException))]
		public async Task TestNoRedirection (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (HandleRequest);
			ctx.Handler.AllowAutoRedirect = false;
			await ctx.Client.GetStringAsync (uri).ConfigureAwait (false);
		}

		[HttpClientTest]
		public async Task TestStatusCode (HttpClientTestContext ctx,
		                                  CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (HandleRequest);
			ctx.Handler.AllowAutoRedirect = false;
			var response = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			ctx.Assert (response.StatusCode, Is.EqualTo (HttpStatusCode.Moved));
		}
	}
}
