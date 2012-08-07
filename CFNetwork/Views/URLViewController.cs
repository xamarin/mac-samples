//
// MonoMac.CFNetwork.Test.Views.URLViewController
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
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	public partial class URLViewController : MonoMac.AppKit.NSViewController {
		public URLViewController ()
			: base ("URLView", NSBundle.MainBundle)
		{
		}
		
		public new URLView View {
			get {
				return (URLView)base.View;
			}
		}

		public override void LoadView ()
		{
			base.LoadView ();

			URLBox.DataSource = AppDelegate.URLList;
		}

		partial void Load (NSObject sender)
		{
			if (URLBox.StringValue == string.Empty)
				return;

			if (URLBox.SelectedIndex < 0) {
				AppDelegate.URLList.Add (URLBox.StringValue);
				URLBox.ReloadData ();
			}

			AppDelegate.Instance.MainWindowController.Load (URLBox.StringValue);
		}

		partial void Stop (NSObject sender)
		{
			AppDelegate.Instance.MainWindowController.Stop ();
		}
	}
}

