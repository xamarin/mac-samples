//
// MonoMac.CFNetwork.Test.AppDelegate
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
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using AsyncTests.HttpClientTests.Addin;
using AsyncTests.HttpClientTests.Test;

namespace MonoMac.CFNetwork.Test {

	using UnitTests;
	using Views;

	public partial class AppDelegate : NSApplicationDelegate {
		URLListDataSource urlList;
		MainWindowController mainWindowController;
		PreferencesController preferencesController;
		LogViewerController logViewerController;
		UnitTestDelegate unitTestDelegate;
		UnitTestRunnerController unitTestController;

		public AppDelegate ()
		{
			urlList = new URLListDataSource ();
			mainWindowController = new MainWindowController ();
			preferencesController = new PreferencesController ();
			logViewerController = new LogViewerController ();

			unitTestDelegate = new UnitTestDelegate ();
			unitTestController = new UnitTestRunnerController ();

			preferencesController.RegisterDefaults ();
			unitTestController.RegisterDefaults ();
		}

		public override void FinishedLaunching (NSObject notification)
		{
			preferencesController.LoadDefaults ();
			unitTestController.LoadDefaults ();

			logViewerController.Window.LayoutIfNeeded ();

			Debug.AutoFlush = true;
			Debug.Listeners.Add (new ConsoleTraceListener ());

			StartServer ();

			RunUnitTests (this);
		}

		public override void WillTerminate (NSNotification notification)
		{
			preferencesController.SaveDefaults ();
			unitTestController.SaveDefaults ();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

		partial void GetString (NSObject sender)
		{
			mainWindowController.ShowWindow (this);
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.GetString ();
		}

		partial void DownloadData (NSObject sender)
		{
			mainWindowController.ShowWindow (this);
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.DownloadData ();
		}

		partial void CheckHeaders (NSObject sender)
		{
			mainWindowController.ShowWindow (this);
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.CheckHeaders ();
		}

		partial void Benchmark (NSObject sender)
		{
			mainWindowController.ShowWindow (this);
			mainWindowController.Window.MakeKeyAndOrderFront (this);
			mainWindowController.Benchmark ();
		}

		partial void Preferences (NSObject sender)
		{
			preferencesController.ShowWindow (this);
			preferencesController.Window.MakeKeyAndOrderFront (this);
		}

		partial void OpenLogViewer (NSObject sender)
		{
			logViewerController.ShowWindow (this);
			logViewerController.Window.MakeKeyAndOrderFront (this);
		}

		partial void RunUnitTests (NSObject sender)
		{
			unitTestController.ShowWindow (this);
			unitTestController.Window.MakeKeyAndOrderFront (this);
		}

		partial void RestartServer (NSObject sender)
		{
			RestartServer ();
		}

		internal async Task RestartServer ()
		{
			await Server.Stop ();
			await StartServer ();
		}

		internal async Task StartServer ()
		{
			var prefix = preferencesController.LocalServerAddress;
			await Server.Start (typeof (Simple).Assembly, prefix);
		}

		public static AppDelegate Instance {
			get { return (AppDelegate)NSApplication.SharedApplication.Delegate; }
		}

		public static UnitTestDelegate UnitTestDelegate {
			get { return Instance.unitTestDelegate; }
		}

		[Export ("settings")]
		public PreferencesController Settings {
			get { return preferencesController; }
		}

		public static PreferencesController StaticSettings {
			get { return Instance.Settings; }
		}

		[Export ("controller")]
		public MainWindowController MainWindowController {
			get { return mainWindowController; }
		}

		public static URLListDataSource URLList {
			get { return Instance.urlList; }
		}
	}
}

