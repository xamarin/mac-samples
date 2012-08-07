//
// MonoMac.CFNetwork.Test.Views.BenchmarkViewController
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
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.CoreText;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	using Models;

	public partial class BenchmarkViewController : NSViewController, IAsyncViewController {
		#region Constructors
		
		// Called when created from unmanaged code
		public BenchmarkViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public BenchmarkViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public BenchmarkViewController () : base ("BenchmarkView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			repeatCount = 1;

			var font = NSFont.UserFixedPitchFontOfSize (16);
			textAttrs = new NSMutableDictionary ();
			textAttrs.SetValueForKey (font, NSAttributedString.FontAttributeName);
		}
		
		#endregion
		
		//strongly typed view accessor
		public new BenchmarkView View {
			get {
				return (BenchmarkView)base.View;
			}
		}

		public AsyncTaskRunner TaskRunner {
			get { return AsyncTaskRunnerController.Benchmark; }
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			AsyncTaskRunnerController.Benchmark.DetailedMessageEvent += (sender, e) => {
				Storage.Append (new NSAttributedString (e.Message, textAttrs));
			}; 
		}

		int mode;
		int repeatCount;
		NSMutableDictionary textAttrs;
		const string kMode = "Mode";
		const string kRepeatCount = "RepeatCount";
		const string kResults = "Results";

		public enum ModeTag {
			GetByteArray = 0,
			CheckHeaders = 1
		}

		protected NSTextStorage Storage {
			get { return Results.TextStorage; }
		}

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

		public ModeTag GetMode ()
		{
			return (ModeTag)mode;
		}

		[Export (kRepeatCount)]
		public int RepeatCount {
			get {
				return repeatCount;
			}
			set {
				WillChangeValue (kRepeatCount);
				repeatCount = value;
				DidChangeValue (kRepeatCount);
			}
		}

		partial void Clear (NSObject sender)
		{
			Storage.DeleteRange (new NSRange (0, Storage.Length));
		}

		[Register ("LogValueTransformer")]
		public class LogValueTransformer : NSValueTransformer {
			public override NSObject TransformedValue (NSObject value)
			{
				var number = (NSNumber)value;
				var log = Math.Log (number.IntValue, 2);
				return (NSNumber)log;
			}

			public override NSObject ReverseTransformedValue (NSObject value)
			{
				var number = (NSNumber)value;
				var exp = Math.Pow (2, number.IntValue);
				return (NSNumber)exp;
			}
		}
	}
}

