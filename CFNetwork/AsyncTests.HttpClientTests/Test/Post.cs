//
// AsyncTests.HttpClientTests.Test.Post
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
using System.IO.Pipes;
using System.Net;
using System.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;

namespace AsyncTests.HttpClientTests.Test {

	using Framework;
	using Addin;

	[Experimental]
	[HttpClientTestFixture]
	public class Post {
		const string Text = "Hello Uploaded Text";
		const long UploadSize = 2000000;

		[RequestHandler]
		public static void Post_Handler (ServerContext ctx)
		{
			ctx.Expect (ctx.Request.HttpMethod, Is.EqualTo ("POST"), "#1");

			string text;
			using (var reader = new StreamReader (ctx.Request.InputStream))
				text = reader.ReadToEnd ();

			ctx.Expect (text, Is.EqualTo (Text), "#3");
			ctx.Response.StatusCode = 200;
		}

		[HttpClientTest (ThreadingMode = ThreadingMode.MainThread | ThreadingMode.ExitContext |
		                 ThreadingMode.ThreadPool)]
		public async Task Simple (HttpClientTestContext ctx,
		                          CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (Post_Handler);

			var request = new HttpRequestMessage (HttpMethod.Post, uri);
			request.Content = new StringContent (Text);

			HttpResponseMessage response;
			var task = ctx.Client.SendAsync (request, cancellationToken);
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
		}

		[RequestHandler]
		public static void Upload_Handler (ServerContext ctx)
		{
			ctx.Expect (ctx.Request.HttpMethod, Is.EqualTo ("POST"), "#1");

			long totalSize = 0;

			int ret;
			do {
				var buffer = new byte [20000];
				ret = ctx.Request.InputStream.Read (buffer, 0, buffer.Length);
				if (ret > 0)
					totalSize += ret;
			} while (ret > 0);

			ctx.Expect (totalSize, Is.EqualTo (UploadSize), "#2");
			ctx.Response.StatusCode = 200;
		}

		[HttpClientTest]
		public async Task Upload (HttpClientTestContext ctx,
		                          CancellationToken cancellationToken)
		{
			var uri = Server.GetUri (Upload_Handler);

			var upload = new RandomDataStream (UploadSize);

			var request = new HttpRequestMessage (HttpMethod.Post, uri);
			request.Content = new StreamContent (upload);

			HttpResponseMessage response;
			var task = ctx.Client.SendAsync (request, cancellationToken);
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
		}
	}
}
