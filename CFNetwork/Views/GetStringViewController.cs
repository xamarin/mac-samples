//
// MonoMac.CFNetwork.Test.Views.GetStringViewController
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
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.WebKit;
using MonoMac.CFNetwork;

namespace MonoMac.CFNetwork.Test.Views {

	using Models;

	public partial class GetStringViewController : NSViewController, IAsyncViewController {
		#region Constructors
		
		// Called when created from unmanaged code
		public GetStringViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public GetStringViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public GetStringViewController () : base ("GetStringView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			url = string.Empty;
			text = string.Empty;
		}

		#endregion
		
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			textView = new NSTextView ();
			webView = new WebView ();

			AsyncTaskRunnerController.Instance.OperationStartedEvent += (sender, e) => {
				url = string.Empty;
				text = string.Empty;
				Update ();
			};
			AsyncTaskRunnerController.GetString.CompletedEvent += (sender, e) => {
				url = e.Uri.OriginalString;
				text = e.Text;
				Update ();
			};
		}
		
		//strongly typed view accessor
		public new GetStringView View {
			get {
				return (GetStringView)base.View;
			}
		}

		public AsyncTaskRunner TaskRunner {
			get { return AsyncTaskRunnerController.GetString; }
		}

		public enum ModeIndex {
			Text = 0,
			Html
		}
		;

		NSTextView textView;
		WebView webView;
		string text;
		string url;
		int mode;
		const string kMode = "Mode";

		[Export (kMode)]
		public int Mode {
			get {
				return mode;
			}
			set {
				WillChangeValue (kMode);
				mode = value;
				DidChangeValue (kMode);
			}
		}

		public override void LoadView ()
		{
			base.LoadView ();

			DisplayModeChanged (null);
		}

		partial void DisplayModeChanged (NSObject sender)
		{
			NSView view;
			switch ((ModeIndex)Mode) {
			case ModeIndex.Text:
				view = textView;
				break;
			case ModeIndex.Html:
				view = webView;
				break;
			default:
				throw new InvalidOperationException ();
			}

			Content.ContentView = view;
			view.Frame = Content.Bounds;
			view.AutoresizingMask = NSViewResizingMask.HeightSizable |
				NSViewResizingMask.WidthSizable;

			Update ();
		}

		void Update ()
		{
			switch ((ModeIndex)Mode) {
			case ModeIndex.Text:
				textView.TextStorage.SetString (new NSAttributedString (text));
				break;
			case ModeIndex.Html:
				webView.MainFrame.LoadHtmlString (text, new NSUrl (url));
				break;
			}
		}
	}
}

