//
// MainWindowController.cs
//
// Author:
//   Aaron Bockover <abock@xamarin.com>
//
// Copyright 2012 Xamarin Inc. (http://xamarin.com)
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
using System.Reflection;

using MonoMac.Foundation;
using MonoMac.AppKit;

namespace NSAlertSample
{
	public partial class MainWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public MainWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MainWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController () : base ("MainWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			var timerCount = 0;
			NSRunLoop.Current.AddTimer (NSTimer.CreateRepeatingTimer (TimeSpan.FromSeconds (0.1), () => {
				ModalCounter.StringValue = (timerCount++).ToString ();
			}), NSRunLoopMode.Default);
		}
		
		#endregion
		
		//strongly typed window accessor
		public new MainWindow Window {
			get {
				return (MainWindow)base.Window;
			}
		}

		void ShowResponse (NSAlert alert, int response)
		{
			string message;

			if (response <= 1) {
				switch (response) {
				case -1:
					message = String.Format ("Non-custom response: -1 (other)");
					break;
				case 0:
					message = String.Format ("Non-custom response: 0 (alternate)");
					break;
				case 1:
					message = String.Format ("Non-custom response: 1 (default)");
					break;
				default:
					message = String.Format ("Unknown Response: {0}", response);
					break;
				}
			} else {
				var buttonIndex = response - (int)NSAlertButtonReturn.First;
				if (buttonIndex >= alert.Buttons.Length)
					message = String.Format ("Unknown Response: {0}", response);
				else
					message = String.Format (
						"\"{0}\"\n\nButton Index: {1}\nResult (NSAlertButtonReturn): {2}\nResult (int): {3}",
						alert.Buttons [buttonIndex].Title,
						buttonIndex,
						(NSAlertButtonReturn)response,
						response);
			}

			if (alert.ShowsSuppressionButton)
				message += String.Format ("\nSuppression: {0}", alert.SuppressionButton.State);

			ResultLabel.StringValue = message;
		}

		void Run (NSAlert alert)
		{
			switch (AlertOptions.SelectedTag) {
			case 0:
				alert.BeginSheetForResponse (Window, response => ShowResponse (alert, response));
				break;
			case 1:
				ShowResponse (alert, alert.RunSheetModal (Window));
				break;
			case 2:
				ShowResponse (alert, alert.RunModal ());
				break;
			default:
				ResultLabel.StringValue = "Unknown Alert Option";
				break;
			}
		}

		#region NSAlert Sample Implementations

		partial void NSAlertWithMessage (NSObject sender)
		{
			Run (NSAlert.WithMessage ("Hello NSAlert", "Default", "Alternate", "Other", String.Empty));
		}

		partial void NSAlertWithError (NSObject sender)
		{
			Run (NSAlert.WithError (new NSError (new NSString ("org.mono-project.NSAlertSample"), 3000, null)));
		}

		partial void CustomButtons (NSObject sender)
		{
			var alert = new NSAlert {
				MessageText = "Pick a Number!",
				InformativeText = "Long description about why picking a number is important."
			};
			
			alert.AddButton ("One");
			alert.AddButton ("Two");
			alert.AddButton ("Three");
			alert.AddButton ("Four");
			alert.AddButton ("Five");
			alert.AddButton ("Six");

			Run (alert);
		}

		partial void CustomImage (NSObject sender)
		{
			var alert = new NSAlert {
				MessageText = "The cat that started it all!"
			};
			
			var asm = Assembly.GetExecutingAssembly ();
			using (var stream = asm.GetManifestResourceStream ("NSAlertSample.i-can-has-cheezburger.jpg")) {
				alert.Icon = NSImage.FromStream (stream);
			}

			alert.AddButton ("No Can Has");

			Run (alert);
		}

		partial void DefaultSuppression (NSObject sender)
		{
			var alert = new NSAlert {
				MessageText = "Purchase More Gold!",
				InformativeText = "Would you like to purchase 30 more pounds of gold?",
				ShowsSuppressionButton = true
			};

			alert.AddButton ("Yes Please!");
			alert.AddButton ("Absolutely Not");

			Run (alert);
		}

		partial void CustomSuppression (NSObject sender)
		{
			var alert = new NSAlert {
				MessageText = "Subscribe to CatOverflow.com",
				InformativeText = "CatOverflow.com features the best cats the Internet has to offer.\n\nUpdated regularly and curated by a professional cat analyist, CatOverflow.com cannot be missed. Make it part of your daily regimen now!\n",
				ShowsSuppressionButton = true
			};

			alert.SuppressionButton.Title = "Go away forever, meow";
			alert.SuppressionButton.Font = NSFont.ControlContentFontOfSize (NSFont.SmallSystemFontSize);
			
			alert.AddButton ("YES YES YES");
			alert.AddButton ("Remind me later");
			alert.AddButton ("I prefer DogOverflow.com");
			
			Run (alert);
		}

		#endregion
	}
}
