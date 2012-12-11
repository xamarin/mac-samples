//
// AsyncTests.HttpClientTests.Test.WebDav
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
using System.Xml;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.SyntaxHelpers;
using Mono.Addins;

namespace AsyncTests.HttpClientTests.Test {

	using Framework;
	using Addin;

	[WebDav]
	[TestWarning ("Digest authentication is not supported in Mono's default HttpClientHandler.")]
	[HttpClientTestFixture (Target = HttpClientTestTarget.Custom)]
	public class WebDav {
		public Uri Server {
			get;
			private set;
		}

		static Random random = new Random ();

		[AsyncTestSetUp]
		public void SetUp (HttpClientTestContext ctx)
		{
			var config = (WebDavConfiguration)ctx.Fixture.Configuration;

			Server = new Uri (config.Server);

			var cache = new CredentialCache ();
			cache.Add (Server, "Digest", new NetworkCredential (config.UserName, config.Password));
			ctx.Handler.Credentials = cache;
		}

		Uri GetRandomUri ()
		{
			var name = random.Next ().ToString ("x") + ".txt";
			return new Uri (Server, name);
		}

		public const string Text = "Hello World";

		[HttpClientTest]
		public async Task Put (HttpClientTestContext ctx,
		                       CancellationToken cancellationToken)
		{
			var uri = GetRandomUri ();

			var content = new StringContent (Text);

			var response = await ctx.Client.PutAsync (
				uri, content, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");
		}

		[HttpClientTest]
		public async Task PutAndDelete (HttpClientTestContext ctx,
		                                CancellationToken cancellationToken)
		{
			var uri = GetRandomUri ();

			var content = new StringContent (Text);

			var response = await ctx.Client.PutAsync (
				uri, content, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			var response2 = await ctx.Client.DeleteAsync (uri, cancellationToken);
			ctx.AutoDispose (response2);

			ctx.Assert (response2.IsSuccessStatusCode, Is.True, "#101");

			var response3 = await ctx.Client.GetAsync (uri, cancellationToken);
			ctx.AutoDispose (response3);

			ctx.Assert (response3.StatusCode, Is.EqualTo (HttpStatusCode.NotFound), "#102");
		}
	}
}
