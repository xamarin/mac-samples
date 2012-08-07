//
// MonoMac.CFNetwork.Test.Views.DownloadDataViewController
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CFNetwork;

namespace MonoMac.CFNetwork.Test.Views {

	using Models;

	public partial class DownloadDataViewController : NSViewController, IAsyncViewController {
		#region Constructors
		
		// Called when created from unmanaged code
		public DownloadDataViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public DownloadDataViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public DownloadDataViewController () : base ("DownloadDataView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion
		
		//strongly typed view accessor
		public new DownloadDataView View {
			get {
				return (DownloadDataView)base.View;
			}
		}

		public AsyncTaskRunner TaskRunner {
			get { return AsyncTaskRunnerController.DownloadData; }
		}

		CheckHeadersViewController HeaderController {
			get {
				return AppDelegate.Instance.MainWindowController.HeaderController;
			}
		}

		public override void LoadView ()
		{
			base.LoadView ();

			var header = HeaderController.View;

			header.AutoresizingMask = NSViewResizingMask.WidthSizable |
				NSViewResizingMask.HeightSizable;
			header.Frame = View.Bounds;
			View.AddSubview (header);
		}
	}
}

