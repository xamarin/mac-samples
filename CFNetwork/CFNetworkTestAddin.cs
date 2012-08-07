//
// MonoMac.CFNetwork.Test.CFNetworkTestAddin
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AsyncTests.Framework;
using AsyncTests.HttpClientTests.Addin;
using MonoMac.CFNetwork;
using Mono.Addins;

[assembly:Addin]
[assembly:AddinDependency ("AsyncTests", "0.1")]
[assembly:AddinDependency ("AsyncTests.HttpClientTests", "0.1")]

namespace MonoMac.CFNetwork.Test
{
	static class CFNetworkTestAddin
	{
		static CFNetworkTestAddin ()
		{
			HttpClientTestHost.Persistent = true;
		}

		[Extension]
		class CFNetwork : IHttpClientHandler
		{
			public HttpClientHandler CreateHandler ()
			{
				return new MessageHandler ();
			}

			public string Name {
				get { return "CFNetwork"; }
			}

			public Task SetUp (TestSuite suite)
			{
				return Task.FromResult<object> (null);
			}

			public Task TearDown (TestSuite suite)
			{
				return Task.FromResult<object> (null);
			}
		}

		[Extension]
		class CFNetworkOnWorkerThread : IHttpClientHandler
		{
			WorkerThread worker;

			public HttpClientHandler CreateHandler ()
			{
				return new MessageHandler (worker);
			}

			public string Name {
				get { return "CFNetwork/WorkerThread"; }
			}

			public Task SetUp (TestSuite suite)
			{
				worker = new WorkerThread ();
				return Task.FromResult<object> (null);
			}

			public Task TearDown (TestSuite suite)
			{
				worker.Stop ();
				worker = null;
				return Task.FromResult<object> (null);
			}
		}

		[Extension]
		class CFNetworkTestCategory : ITestCategory
		{
			public string Name {
				get { return "CFNetwork"; }
			}

			public bool IsEnabled (ITestRunner runner)
			{
				return runner is ExtensionTestRunner;
			}

			public bool IsEnabled (TestCase test)
			{
				return test.Attribute is HttpClientTestAttribute;
			}
		}
	}
}
