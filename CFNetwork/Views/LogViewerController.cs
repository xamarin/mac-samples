//
// MonoMac.CFNetwork.Test.Views.LogViewerController
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
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	public partial class LogViewerController : MonoMac.AppKit.NSWindowController {
		#region Constructors
		
		// Called when created from unmanaged code
		public LogViewerController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LogViewerController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public LogViewerController () : base ("LogViewer")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			writer = new OutputTextWriter (this);
			listener = new Listener (this);
		}
		
		#endregion

		NSFont font;
		TraceListener listener;
		OutputTextWriter writer;

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			font = NSFont.SystemFontOfSize (16.0f);
		}

		//strongly typed window accessor
		public new LogViewer Window {
			get {
				return (LogViewer)base.Window;
			}
		}

		public NSTextStorage Storage {
			get {
				return Text.TextStorage;
			}
		}

		public TextWriter TextWriter {
			get {
				return writer;
			}
		}

		public override void ShowWindow (NSObject sender)
		{
			base.ShowWindow (sender);
			Debug.Listeners.Add (listener);
		}

		partial void Clear (NSObject sender)
		{
			Storage.DeleteRange (new NSRange (0, Storage.Length));
		}

		partial void Quit (NSObject sender)
		{
			Debug.Listeners.Remove (listener);
			Window.Close ();
		}

		#region Appending Text

		public void Append (string text)
		{
			InvokeOnMainThread (() => DoAppend (text));
		}

		public void AppendLine (string message)
		{
			InvokeOnMainThread (() => DoAppend (message + Environment.NewLine));
		}

		public void AppendLine (NSColor color, string text)
		{
			InvokeOnMainThread (() => DoAppend (color, text + Environment.NewLine));
		}

		void DoAppend (string text)
		{
			var pos = Storage.Length;
			Storage.Append (new NSAttributedString (text));
			Text.SetFont (font, new NSRange (pos, text.Length));
		}

		void DoAppend (NSColor color, string text)
		{
			var pos = Storage.Length;
			Storage.Append (new NSAttributedString (text));
			var range = new NSRange (pos, text.Length);
			Text.SetFont (font, range);
			Text.SetTextColor (color, range);
		}

		#endregion

		#region Text Writer

		class OutputTextWriter : TextWriter {
			LogViewerController controller;

			public OutputTextWriter (LogViewerController controller)
			{
				this.controller = controller;
			}

			public override Encoding Encoding {
				get { return Encoding.Default; }
			}

			public override void Write (char value)
			{
				controller.Append (value.ToString ());
			}

			public override void Write (string value)
			{
				controller.Append (value);
			}
		}

		class Listener : TraceListener {
			LogViewerController controller;

			public Listener (LogViewerController controller)
			{
				this.controller = controller;
			}

			#region implemented abstract members of TraceListener
			public override void Write (string message)
			{
				controller.Append (message);
			}

			public override void WriteLine (string message)
			{
				controller.AppendLine (message);
			}

			public override void Fail (string message)
			{
				Fail (message, "");
			}

			public override void Fail (string message, string detailMessage)
			{
				controller.AppendLine ("---- DEBUG ASSERTION FAILED ----");
				controller.AppendLine ("---- Assert Short Message ----");
				controller.AppendLine (NSColor.Red, message);
				controller.AppendLine ("---- Assert Long Message ----");
				controller.AppendLine (NSColor.Orange, detailMessage);
				controller.AppendLine ("");
			}
			#endregion
		}

		#endregion
	}
}

