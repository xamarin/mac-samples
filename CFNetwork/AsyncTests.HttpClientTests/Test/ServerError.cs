//
// AsyncTests.HttpClientTests.Test.ServerError
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

	[HttpClientTestFixture]
	public class ServerError {
		public const string Text = "Error Text";

		[RequestHandler]
		public static void Error500Handler (ServerContext ctx)
		{
			ctx.Response.StatusCode = 500;
			using (var writer = new StreamWriter (ctx.Response.OutputStream))
				writer.Write (Text);
		}

		[RequestHandler]
		public static void NoResponseHandler (ServerContext ctx)
		{
			ctx.Response.StatusCode = 500;
		}

		[HttpClientTest]
		[ExpectedException (typeof (HttpRequestException))]
		public async Task GetStringAsync (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (Error500Handler);
			await ctx.Client.GetStringAsync (uri).ConfigureAwait (false);
		}

		[HttpClientTest]
		public async Task GetAsync (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (Error500Handler);
			var res = await ctx.Client.GetAsync (uri).ConfigureAwait (false);
			ctx.AutoDispose (res);

			ctx.Assert (res.StatusCode, Is.EqualTo (HttpStatusCode.InternalServerError));
		}

		[HttpClientTest]
		public async Task GetErrorMessage (HttpClientTestContext ctx,
		                                   CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (Error500Handler);
			var res = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseContentRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (res);

			ctx.Assert (res.StatusCode, Is.EqualTo (HttpStatusCode.InternalServerError));
			var text = await res.Content.ReadAsStringAsync ();
			ctx.Assert (text, Is.EqualTo (Text));
		}

		[HttpClientTest]
		[ExpectedException (typeof (HttpRequestException))]
		public async Task GetStringNoResponse (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (NoResponseHandler);
			await ctx.Client.GetStringAsync (uri).ConfigureAwait (false);
		}
	}
}
