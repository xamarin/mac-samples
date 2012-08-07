//
// MonoMac.CFNetwork.Test.Views.PreferencesController
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
using AsyncTests.Framework;
using AsyncTests.HttpClientTests.Test;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	public partial class PreferencesController : MonoMac.AppKit.NSWindowController {
		#region Constructors
		
		// Called when created from unmanaged code
		public PreferencesController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PreferencesController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PreferencesController () : base ("Preferences")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}
		
		#endregion

		public static PreferencesController Global {
			get { return ((AppDelegate)NSApplication.SharedApplication.Delegate).Settings; }
		}

		//strongly typed window accessor
		public new Preferences Window {
			get {
				return (Preferences)base.Window;
			}
		}

		bool autoRedirect;
		bool downloadWithoutLength;
		bool useRelativeURL;
		bool useAuthentication;
		bool usePersistentAuthentication;
		NSString localServerAddress;
		NSString relativeURL;
		NSString userName;
		NSString password;
		WebDavConfiguration webDav;
		const string kAutoRedirect = "AutoRedirect";
		const string kDownloadWithoutLength = "DownloadWithoutLength";
		const string kUseRelativeURL = "UseRelativeURL";
		const string kRelativeURL = "RelativeURL";
		const string kUseAuthentication = "UseAuthentication";
		const string kUsePersistentAuthentication = "UsePersistentAuthentication";
		const string kUserName = "UserName";
		const string kPassword = "Password";
		const string kLocalServerAddress = "LocalServerAddress";
		const string kEnableWebDavTests = "EnableWebDavTests";
		const string kWebDavServer = "WebDavServer";
		const string kWebDavUserName = "WebDavUserName";
		const string kWebDavPassword = "WebDavPassword";

		public void RegisterDefaults ()
		{
			var dict = new NSMutableDictionary ();
			dict [kAutoRedirect] = NSNumber.FromBoolean (true);
			dict [kDownloadWithoutLength] = NSNumber.FromBoolean (false);
			dict [kUseRelativeURL] = NSNumber.FromBoolean (false);
			dict [kUseAuthentication] = NSNumber.FromBoolean (false);
			dict [kUsePersistentAuthentication] = NSNumber.FromBoolean (false);
			dict [kUserName] = (NSString)"mono";
			dict [kPassword] = (NSString)"monkey";
			dict [kLocalServerAddress] = (NSString)"http://localhost:8088/";

			dict [kEnableWebDavTests] = NSNumber.FromBoolean (false);
			dict [kWebDavServer] = (NSString)"http://localhost/uploads/";
			dict [kWebDavUserName] = (NSString)"admin";
			dict [kWebDavPassword] = (NSString)"monkey";
			NSUserDefaults.StandardUserDefaults.RegisterDefaults (dict);
		}

		public void LoadDefaults ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			webDav = TestSuite.GetConfiguration<WebDavConfiguration> ();

			autoRedirect = ((NSNumber)defaults [kAutoRedirect]).BoolValue;
			downloadWithoutLength = ((NSNumber)defaults [kDownloadWithoutLength]).BoolValue;
			useRelativeURL = ((NSNumber)defaults [kUseRelativeURL]).BoolValue;
			useAuthentication = ((NSNumber)defaults [kUseAuthentication]).BoolValue;
			usePersistentAuthentication = ((NSNumber)defaults [kUsePersistentAuthentication]).BoolValue;
			userName = (NSString)defaults [kUserName];
			password = (NSString)defaults [kPassword];
			localServerAddress = (NSString)defaults [kLocalServerAddress];

			webDav.IsEnabled = ((NSNumber)defaults [kEnableWebDavTests]).BoolValue;
			webDav.Server = (NSString)defaults [kWebDavServer];
			webDav.UserName = (NSString)defaults [kWebDavUserName];
			webDav.Password = (NSString)defaults [kWebDavPassword];
		}

		public void SaveDefaults ()
		{
			var defaults = NSUserDefaults.StandardUserDefaults;
			defaults [kAutoRedirect] = NSNumber.FromBoolean (autoRedirect);
			defaults [kDownloadWithoutLength] = NSNumber.FromBoolean (downloadWithoutLength);
			defaults [kUseRelativeURL] = NSNumber.FromBoolean (useRelativeURL);
			defaults [kUseAuthentication] = NSNumber.FromBoolean (useAuthentication);
			defaults [kUsePersistentAuthentication] = NSNumber.FromBoolean (usePersistentAuthentication);
			defaults [kUserName] = userName;
			defaults [kPassword] = password;
			defaults [kLocalServerAddress] = localServerAddress;

			defaults [kEnableWebDavTests] = NSNumber.FromBoolean (webDav.IsEnabled);
			defaults [kWebDavServer] = (NSString)webDav.Server;
			defaults [kWebDavUserName] = (NSString)webDav.UserName;
			defaults [kWebDavPassword] = (NSString)webDav.Password;
		}

		[Export (kAutoRedirect)]
		public bool AutoRedirect {
			get {
				return autoRedirect;
			}
			set {
				WillChangeValue (kAutoRedirect);
				autoRedirect = value;
				DidChangeValue (kAutoRedirect);
			}
		}

		[Export (kDownloadWithoutLength)]
		public bool DownloadWithoutLength {
			get {
				return downloadWithoutLength;
			}
			set {
				WillChangeValue (kDownloadWithoutLength);
				downloadWithoutLength = value;
				DidChangeValue (kDownloadWithoutLength);
			}
		}

		[Export (kUseRelativeURL)]
		public bool UseRelativeURL {
			get {
				return useRelativeURL;
			}
			set {
				WillChangeValue (kUseRelativeURL);
				useRelativeURL = value;
				DidChangeValue (kUseRelativeURL);
			}
		}

		[Export (kRelativeURL)]
		public NSString RelativeURL {
			get {
				return relativeURL;
			}
			set {
				WillChangeValue (kUseRelativeURL);
				relativeURL = value;
				DidChangeValue (kUseRelativeURL);
			}
		}

		[Export (kUseAuthentication)]
		public bool UseAuthentication {
			get {
				return useAuthentication;
			}
			set {
				WillChangeValue (kUseAuthentication);
				useAuthentication = value;
				DidChangeValue (kUseAuthentication);
			}
		}

		[Export (kUsePersistentAuthentication)]
		public bool UsePersistentAuthentication {
			get {
				return usePersistentAuthentication;
			}
			set {
				WillChangeValue (kUsePersistentAuthentication);
				usePersistentAuthentication = value;
				DidChangeValue (kUsePersistentAuthentication);
			}
		}

		[Export (kUserName)]
		public NSString UserName {
			get {
				return userName;
			}
			set {
				WillChangeValue (kUserName);
				userName = value;
				DidChangeValue (kUserName);
			}
		}

		[Export (kPassword)]
		public NSString Password {
			get {
				return password;
			}
			set {
				WillChangeValue (kPassword);
				password = value;
				DidChangeValue (kPassword);
			}
		}

		[Export (kLocalServerAddress)]
		public NSString LocalServerAddress {
			get {
				return localServerAddress;
			}
			set {
				WillChangeValue (kLocalServerAddress);
				localServerAddress = value;
				DidChangeValue (kLocalServerAddress);
			}
		}

		[Export (kEnableWebDavTests)]
		public bool EnableWebDavTests {
			get {
				return webDav.IsEnabled;
			}
			set {
				WillChangeValue (kEnableWebDavTests);
				webDav.IsEnabled = value;
				DidChangeValue (kEnableWebDavTests);
			}
		}

		[Export (kWebDavServer)]
		public string WebDavServer {
			get {
				return webDav.Server;
			}
			set {
				WillChangeValue (kWebDavServer);
				webDav.Server = value;
				DidChangeValue (kWebDavServer);
			}
		}

		[Export (kWebDavUserName)]
		public string WebDavUserName {
			get {
				return webDav.UserName;
			}
			set {
				WillChangeValue (kWebDavUserName);
				webDav.UserName = value;
				DidChangeValue (kWebDavUserName);
			}
		}

		[Export (kWebDavPassword)]
		public string WebDavPassword {
			get {
				return webDav.Password;
			}
			set {
				WillChangeValue (kWebDavPassword);
				webDav.Password = value;
				DidChangeValue (kWebDavPassword);
			}
		}
	}
}

