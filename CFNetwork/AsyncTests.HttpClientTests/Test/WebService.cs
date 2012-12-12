//
// AsyncTests.HttpClientTests.Test.WebService
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
using Mono.Addins;

namespace AsyncTests.HttpClientTests.Test {

	using Framework;
	using Addin;

	[Extension]
	public class WebServiceAttribute : TestCategoryAttribute {
		public override string Name {
			get { return "Web Service"; }
		}
	}

	[WebService]
	[HttpClientTestFixture]
	public class WebService {
		// Start XSP in the TestWebServer directory
		const string ServiceURI = "http://localhost:8080/TestService.asmx";

		[HttpClientTest]
		public async Task GetRandomData (HttpClientTestContext ctx,
		                                 CancellationToken cancellationToken)
		{
			var uri = ServiceURI + "/GetRandomData";

			var body = "count=100" + Environment.NewLine;

			var request = new HttpRequestMessage (HttpMethod.Post, uri);
			request.Content = new StringContent (body);
			var ctype = new MediaTypeHeaderValue ("application/x-www-form-urlencoded");
			request.Content.Headers.ContentType = ctype;

			var response = await ctx.Client.SendAsync (
				request, cancellationToken).ConfigureAwait (false);
			ctx.AutoDispose (response);

			var text = await response.Content.ReadAsStringAsync ();

			ctx.Log ("POST: {0} {1} {2}", ctx.IsDefaultHandler, response.ReasonPhrase, text);

			ctx.Assert (response.IsSuccessStatusCode, Is.True, "#100");

			var settings = new XmlReaderSettings ();
			settings.IgnoreWhitespace = true;
			settings.IgnoreComments = true;

			var reader = XmlReader.Create (new StringReader (text), settings);
			ctx.Assert (reader.Read (), Is.True, "#101");
			ctx.Assert (reader.NodeType, Is.EqualTo (XmlNodeType.XmlDeclaration), "#102");
			ctx.Assert (reader.Read (), Is.True, "#103");
			ctx.Assert (reader.NodeType, Is.EqualTo (XmlNodeType.Element), "#104");

			var buffer = new byte [256];
			var ret = reader.ReadElementContentAsBase64 (buffer, 0, 256);
			ctx.Assert (ret, Is.EqualTo (100), "#105");
		}
	}
}
