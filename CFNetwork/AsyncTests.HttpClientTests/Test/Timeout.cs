//
// AsyncTests.HttpClientTests.Test.Timeout
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
	public class Timeout {
		const int DataDelay = 1000;
		const int DataTimeout = 1500;
		const int HeaderTimeout = 500;
		static readonly byte[] Header;

		static Timeout ()
		{
			var random = new Random ();
			Header = new byte [8];
			random.NextBytes (Header);
		}

		[RequestHandler]
		public static async Task RandomData_Handler (ServerContext ctx)
		{
			ctx.Response.StatusCode = 200;
			ctx.Response.OutputStream.Write (Header, 0, Header.Length);
			ctx.Response.OutputStream.Flush ();

			await Task.Delay (DataDelay).ConfigureAwait (false);
			ctx.Response.OutputStream.Write (Simple.Data, 0, Simple.Data.Length);
		}

		[HttpClientTest]
		public async Task DownloadWithDelay (HttpClientTestContext ctx,
		                                     CancellationToken cancellationToken)
		{
			var uri = Server.GetAsyncUri (RandomData_Handler);

			var cts = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);
			cts.CancelAfter (HeaderTimeout);

			HttpResponseMessage response;
			try {
				response = await ctx.Client.GetAsync (
					uri, HttpCompletionOption.ResponseHeadersRead, cts.Token);
				ctx.AutoDispose (response);
			} finally {
				cts.Dispose ();
			}

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			var cts2 = CancellationTokenSource.CreateLinkedTokenSource (cancellationToken);
			cts2.Token.Register (() => response.Content.Dispose ());
			cts2.CancelAfter (DataTimeout);

			byte[] data;
			try {
				data = await response.Content.ReadAsByteArrayAsync ();
			} finally {
				cts2.Dispose ();
			}

			ctx.Assert (data.Length, Is.EqualTo (Header.Length + Simple.Data.Length), "#101");

			var header = new byte [Header.Length];
			Buffer.BlockCopy (data, 0, header, 0, header.Length);
			var data2 = new byte [Simple.Data.Length];
			Buffer.BlockCopy (data, header.Length, data2, 0, data2.Length);

			ctx.Expect (header, Is.EqualTo (Header), "#102");
			ctx.Expect (data2, Is.EqualTo (Simple.Data), "#103");
		}
	}
}

