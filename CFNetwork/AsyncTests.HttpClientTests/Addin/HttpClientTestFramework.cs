//
// AsyncTests.HttpClientTests.Addin.HttpClientTestFramework.cs
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
using System.Reflection;
using System.Collections.Generic;
using Mono.Addins;

[assembly:Addin]
[assembly:AddinDependency ("AsyncTests", "0.1")]
[assembly:AddinRoot ("AsyncTests.HttpClientTests", "0.1")]

namespace AsyncTests.HttpClientTests.Addin {

	using Framework;

	[Extension]
	public class HttpClientTestFramework : ITestFramework {
		static IHttpClientHandler[] extensions;
		static ITestRunner defaultRunner;
		static ITestRunner[] extensionRunners;

		static HttpClientTestFramework ()
		{
			extensions = AddinManager.GetExtensionObjects<IHttpClientHandler> ();
			defaultRunner = new DefaultTestRunner ();

			var list = new List<ITestRunner> ();
			for (int i = 0; i < extensions.Length; i++)
				list.Add (new ExtensionTestRunner (extensions [i]));
			extensionRunners = list.ToArray ();
		}

		internal static IHttpClientHandler[] Extensions {
			get { return extensions; }
		}

		public Type FixtureAttributeType {
			get { return typeof (HttpClientTestFixtureAttribute); }
		}

		public Type TestAttributeType {
			get { return typeof (HttpClientTestAttribute); }
		}

		public Type ContextType {
			get { return typeof (HttpClientTestContext); }
		}

		public ITestRunner[] GetTestRunners (TestFixture fixture)
		{
			var attr = fixture.Attribute as HttpClientTestFixtureAttribute;
			if (attr == null)
				return null;
			var list = new List<ITestRunner> ();
			if ((attr.Target & HttpClientTestTarget.Default) != 0)
				list.Add (defaultRunner);
			if ((attr.Target & HttpClientTestTarget.Custom) != 0)
				list.AddRange (extensionRunners);
			return list.ToArray ();
		}
	}
}
