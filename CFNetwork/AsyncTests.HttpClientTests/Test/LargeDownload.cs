//
// AsyncTests.HttpClientTests.Test.LargeDownload
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
	public class LargeDownload {
		const int ChunkSize = 80000;
		const int Count = 50;
		const int TotalSize = Count * ChunkSize;

		[RequestHandler]
		public static async Task LargeDownload_Handler (ServerContext ctx)
		{
			var random = new Random ();

			for (int i = 0; i < Count; i++) {
				var chunk = new byte [ChunkSize];
				random.NextBytes (chunk);

				await ctx.Response.OutputStream.WriteAsync (
					chunk, 0, chunk.Length).ConfigureAwait (false);

				if (i > 0)
					continue;

				await ctx.Response.OutputStream.FlushAsync ();
			}
		}

		[HttpClientTest (ThreadingMode = ThreadingMode.MainThread | ThreadingMode.ExitContext |
		                 ThreadingMode.ThreadPool)]
		public async Task Download (HttpClientTestContext ctx,
		                            CancellationToken cancellationToken)
		{
			var uri = Server.GetAsyncUri (LargeDownload_Handler);

			var task = ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

			HttpResponseMessage response;
			if (ctx.ThreadingMode == ThreadingMode.MainThread)
				response = await task;
			else {
				response = await task.ConfigureAwait (false);
				if (ctx.ThreadingMode == ThreadingMode.ThreadPool)
					await Task.Yield ();
			}

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			var stream = await response.Content.ReadAsStreamAsync ();
			ctx.AutoDispose (stream);

			long total = 0;
			var buffer = new byte [1000];
			while (true) {
				int ret = await stream.ReadAsync (buffer, 0, buffer.Length);
				ctx.Assert (ret, Is.GreaterThanOrEqualTo (0), "#101");
				if (ret == 0)
					break;
				total += ret;
			}

			ctx.Assert (total, Is.EqualTo (TotalSize), "#102");
		}

		[HttpClientTest (ThreadingMode = ThreadingMode.ThreadPool)]
		public async Task SynchronousRead (HttpClientTestContext ctx,
		                                   CancellationToken cancellationToken)
		{
			var uri = Server.GetAsyncUri (LargeDownload_Handler);

			var response = await ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead,
				cancellationToken).ConfigureAwait (false);

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			/*
			 * Since we used ConfigureAwait(false) above, this should send
			 * us to the ThreadPool.
			 */
			await Task.Yield ();
			ctx.Assert (SynchronizationContext.Current, Is.Null, "#101");

			var stream = await response.Content.ReadAsStreamAsync ();
			ctx.AutoDispose (stream);

			ctx.Assert (SynchronizationContext.Current, Is.Null, "#102");

			byte[] buffer = new byte [16];
			int ret = stream.Read (buffer, 0, buffer.Length);
			ctx.Assert (ret, Is.EqualTo (buffer.Length), "#103");
		}

		[HttpClientTest (ThreadingMode = ThreadingMode.MainThread | ThreadingMode.ExitContext |
		                 ThreadingMode.ThreadPool)]
		public async Task CopyToAsync (HttpClientTestContext ctx,
		                               CancellationToken cancellationToken)
		{
			var uri = Server.GetAsyncUri (LargeDownload_Handler);

			var task = ctx.Client.GetAsync (
				uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

			HttpResponseMessage response;
			if (ctx.ThreadingMode == ThreadingMode.MainThread)
				response = await task;
			else {
				response = await task.ConfigureAwait (false);
				if (ctx.ThreadingMode == ThreadingMode.ThreadPool)
					await Task.Yield ();
			}

			ctx.AutoDispose (response);
			Server.CheckException (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			using (var ms = new MemoryStream (TotalSize)) {
				await response.Content.CopyToAsync (ms);
				ctx.Assert (ms.Position, Is.EqualTo (TotalSize), "#101");
			}
		}
	}
}
