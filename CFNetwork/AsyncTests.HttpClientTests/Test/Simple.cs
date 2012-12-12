//
// AsyncTests.HttpClientTests.Test.Simple
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
	public class Simple {
		public const string Text = "Hello World";
		public static readonly byte[] Data;

		static Simple ()
		{
			var random = new Random ();
			Data = new byte [16384];
			random.NextBytes (Data);
		}

		[RequestHandler]
		public static void SimpleHtml_Handler (ServerContext ctx)
		{
			ctx.Response.StatusCode = 200;
			using (var writer = new StreamWriter (ctx.Response.OutputStream))
				writer.Write (Text);
		}

		[HttpClientTest]
		public async Task GetStringAsync (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (SimpleHtml_Handler);
			var text = await ctx.Client.GetStringAsync (uri).ConfigureAwait (false);
			ctx.Assert (text, Is.EqualTo (Text));
		}

		[HttpClientTest]
		public async Task GetByteArrayAsync (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (SimpleHtml_Handler);
			var data = await ctx.Client.GetByteArrayAsync (uri).ConfigureAwait (false);
			ctx.Assert (data, Is.EqualTo (Encoding.UTF8.GetBytes (Text)));
		}

		[HttpClientTest]
		public async Task GetAsync (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (SimpleHtml_Handler);
			var result = await ctx.Client.GetAsync (uri).ConfigureAwait (false);
			ctx.AutoDispose (result);

			ctx.Assert (result.StatusCode, Is.EqualTo (HttpStatusCode.OK));

			var text = await result.Content.ReadAsStringAsync ();
			ctx.Assert (text, Is.EqualTo (Text));
		}

		[RequestHandler]
		public static void RandomData_Handler (ServerContext ctx)
		{
			ctx.Response.StatusCode = 200;
			ctx.Response.OutputStream.Write (Data, 0, Data.Length);
			ctx.Response.Close ();
		}

		[HttpClientTest]
		public async Task RandomDataAsByteArray (HttpClientTestContext ctx)
		{
			var uri = Server.GetUri (RandomData_Handler);
			var data = await ctx.Client.GetByteArrayAsync (uri).ConfigureAwait (false);
			ctx.Assert (data, Is.EqualTo (Data));
		}

		[HttpClientTest]
		public async Task ContentCopyToAsync (HttpClientTestContext ctx,
		                                      CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (RandomData_Handler);
			var response = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			using (var stream = new MemoryStream ()) {
				await response.Content.CopyToAsync (stream);
				var data = stream.GetBuffer ();
				ctx.Expect (data, Is.EqualTo (Data), "#101");
			}
		}

		[HttpClientTest]
		public async Task ContentAsByteArray (HttpClientTestContext ctx,
		                                      CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (RandomData_Handler);
			var response = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			var data = await response.Content.ReadAsByteArrayAsync ();
			ctx.Expect (data, Is.EqualTo (Data), "#101");
		}
	}
}

