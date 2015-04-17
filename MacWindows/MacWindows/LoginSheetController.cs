using System;

using Foundation;
using AppKit;

namespace MacWindows
{
	public class LoginSheetController : NSObject
	{
		#region Computed Properties
		[Export("window")]
		public LoginSheet Window { get; set;}

		[Outlet]
		public NSTextField loginPassword { get; set; }

		[Outlet]
		public NSTextField loginUser { get; set; }

		public bool Canceled { get; set;}

		public string UserID {
			get { return loginUser.StringValue; }
			set { loginUser.StringValue = value; }
		}

		public string Password {
			get { return loginPassword.StringValue; }
			set { loginPassword.StringValue = value; }
		}
		#endregion

		#region Constructors
		public LoginSheetController ()
		{
			// Load the .xib file for the sheet
			NSBundle.LoadNib ("LoginSheet", this);
		}
		#endregion

		#region Public Methods
		public void ShowSheet(NSWindow inWindow) {
			NSApplication.SharedApplication.BeginSheet (Window, inWindow);
		}

		public void CloseSheet() {
			NSApplication.SharedApplication.EndSheet (Window);
			Window.Close();
		}
		#endregion

		#region Button Handlers
		[Export ("loginCancel:")]
		void LoginCancel (NSObject sender) {
			Canceled = true;
			CloseSheet();
			RaiseLoginCanceled ();
		}

		[Export ("loginOK:")]
		void LoginOK (NSObject sender) {
			Canceled = false;
			CloseSheet();
			RaiseLoginRequested ();
		}
		#endregion

		#region Events
		public delegate void LoginCanceledDelegate();
		public event LoginCanceledDelegate LoginCanceled;

		internal void RaiseLoginCanceled() {
			if (this.LoginCanceled != null) {
				this.LoginCanceled();
			}
		}

		public delegate void LoginRequestedDelegate(string userID, string password);
		public event LoginRequestedDelegate LoginRequested;

		internal void RaiseLoginRequested() {
			if (this.LoginRequested != null) {
				this.LoginRequested(UserID, Password);
			}
		}
		#endregion

	}
}

