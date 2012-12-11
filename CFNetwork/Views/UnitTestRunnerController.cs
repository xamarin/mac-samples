//
// MonoMac.CFNetwork.Test.Views.UnitTestRunnerController
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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonoMac.Foundation;
using MonoMac.AppKit;
using AsyncTests.Framework;
using AsyncTests.HttpClientTests.Addin;
using AsyncTests.HttpClientTests.Test;

namespace MonoMac.CFNetwork.Test.Views {

	using UnitTests;

	public partial class UnitTestRunnerController : NSWindowController {
		#region Constructors
		
		// Called when created from unmanaged code
		public UnitTestRunnerController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public UnitTestRunnerController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public UnitTestRunnerController () : base ("UnitTestRunner")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			repeatCount = 1;
			categoryArray = new NSMutableArray ();

			categories = TestSuite.Categories;
			foreach (var category in categories) {
				categoryArray.Add ((NSString)category.Name);
			}
		}
		
		#endregion
		
		//strongly typed window accessor
		public new UnitTestRunner Window {
			get {
				return (UnitTestRunner)base.Window;
			}
		}

		public UnitTestDelegate Delegate {
			get { return AppDelegate.UnitTestDelegate; }
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			Delegate.ChangedEvent += (sender, e) => ResultArea.ReloadData ();
		}

		bool isRunning;
		int repeatCount;
		int selectedCategory;
		ITestCategory[] categories;
		NSMutableArray categoryArray;
		const string kIsRunning = "IsRunning";
		const string kRepeatCount = "RepeatCount";
		const string kCategories = "Categories";
		const string kSelectedCategory = "SelectedCategory";

		public void RegisterDefaults ()
		{
			var dict = new NSMutableDictionary ();
			dict [kRepeatCount] = (NSNumber)1;
			dict [kSelectedCategory] = (NSString)"All";
			NSUserDefaults.StandardUserDefaults.RegisterDefaults (dict);
		}

		public void LoadDefaults ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			repeatCount = (int)(NSNumber)defaults [kRepeatCount];
			var selected = (string)(NSString)defaults [kSelectedCategory];
			for (int i = 0; i < categories.Length; i++) {
				if (!categories [i].Name.Equals (selected))
					continue;
				selectedCategory = i;
				break;
			}
		}

		public void SaveDefaults ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			defaults [kRepeatCount] = (NSNumber)repeatCount;
			defaults [kSelectedCategory] = (NSString)categories [selectedCategory].Name;
		}

		[Export (kIsRunning)]
		public bool IsRunning {
			get { return isRunning; }
			set {
				WillChangeValue (kIsRunning);
				isRunning = value;
				DidChangeValue (kIsRunning);
			}
		}

		[Export (kRepeatCount)]
		public int RepeatCount {
			get { return repeatCount; }
			set {
				WillChangeValue (kRepeatCount);
				repeatCount = value;
				DidChangeValue (kRepeatCount);
			}
		}

		[Export (kCategories)]
		public NSArray Categories {
			get { return categoryArray; }
		}

		[Export (kSelectedCategory)]
		public int SelectedCategory {
			get { return selectedCategory; }
			set {
				WillChangeValue (kSelectedCategory);
				selectedCategory = value;
				DidChangeValue (kSelectedCategory);
			}
		}

		partial void Close (NSObject sender)
		{
			Stop (sender);
			Window.Close ();
		}

		partial void Run (NSObject sender)
		{
			Run ();
		}

		partial void Stop (NSObject sender)
		{
			var c = cts;
			if (c == null)
				return;

			Thread.MemoryBarrier ();

			try {
				c.Cancel ();
			} catch {
				;
			}
		}

		CancellationTokenSource cts;

		async Task Run ()
		{
			lock (this) {
				if (cts != null)
					return;
				cts = new CancellationTokenSource ();
			}

			var assembly = typeof (Simple).Assembly;

			try {
				IsRunning = true;
				Status.StringValue = "Running ...";

				var category = categories [SelectedCategory];

				Delegate.Clear ();
				await AppDelegate.Instance.StartServer ();
				var suite = await TestSuite.Create (assembly);
				Status.StringValue = string.Format ("Got testsuite: {0} fixtures, {1} tests.",
				                                    suite.CountFixtures, suite.CountTests);

				lastStatus = DateTime.MinValue;

				suite.StatusMessageEvent += (sender, e) => UpdateStatus (e.Message);

				bool completed = false;
				var task = suite.Run (category, RepeatCount, cts.Token);
				while (!completed) {
					var ret = await Task.WhenAny (task, Task.Delay (statusInterval));
					if (ret == task)
						break;
					CheckStatus ();
				}
				Delegate.SetResult (task.Result);
				Status.StringValue = "Done";
			} catch (Exception ex) {
				Status.StringValue = string.Format ("ERROR: {0}", ex.Message);
				Debug.Fail (string.Format ("ERROR: {0}", ex));
			} finally {
				IsRunning = false;
				cts.Dispose ();
				cts = null;
			}
		}

		static readonly TimeSpan statusInterval = TimeSpan.FromMilliseconds (200);
		string currentStatus;
		DateTime lastStatus;

		void UpdateStatus (string status)
		{
			currentStatus = status;
			CheckStatus ();
		}

		void CheckStatus ()
		{
			if (DateTime.Now - lastStatus <= statusInterval)
				return;
			Status.StringValue = currentStatus;
			lastStatus = DateTime.Now;
		}
	}
}

