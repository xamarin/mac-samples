//
// AsyncTests.HttpClientTests.Addin.RequestHandlerAttribute
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
using System.Threading.Tasks;

namespace AsyncTests.HttpClientTests.Addin {
	/*
	 * Use the [RequestHandler] attribute on a public static method
	 * matching the RequestHandlerDelegate signature to automatically register
	 * that method with the HttpListener.
	 * 
	 * Only public types with the [AsyncTestFixture] attribute (or an
	 * attribute derived from it) are searched, as well as their nested
	 * children.
	 * 
	 * The @context may be used to log debugging messages or report
	 * errors using NUnit's Constraint syntax.
	 * 
	 * If you throw any exceptions inside the handler, the listener will
	 * return a 500 containing the error message and stores the exception
	 * object locally.
	 * 
	 * It also adds a header field to the response which can be used to
	 * retreived the stored exception object by calling
	 * Server.GetException (HttpResponseMessage) in your test method.
	 * 
	 * Call Server.GetUrl (Method) to get the URL.
	 * 
	 * The listener also automatically generates an index page, so you
	 * can simply open the root URL (http://localhost:8088 by default)
	 * in a web browser.
	 * 
	 */

	public delegate void RequestHandlerDelegate (ServerContext context);

	public delegate Task AsyncRequestHandlerDelegate (ServerContext context);

	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false)]
	public class RequestHandlerAttribute : Attribute {
		AuthenticationSchemes authentication = AuthenticationSchemes.Anonymous;

		public AuthenticationSchemes Authentication {
			get { return authentication; }
			set { authentication = value; }
		}
	}
}

