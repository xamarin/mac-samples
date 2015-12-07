using System;
using Foundation;
using AppKit;

namespace MacDialog
{
	public partial class SheetViewController : NSViewController
	{
		#region Private Variables
		private string _userName = "";
		private string _password = "";
		private NSViewController _presentor;
		#endregion

		#region Computed Properties
		public string UserName {
			get { return _userName; }
			set { _userName = value; }
		}

		public string Password {
			get { return _password;}
			set { _password = value;}
		}

		public NSViewController Presentor {
			get { return _presentor; }
			set { _presentor = value; }
		}
		#endregion

		#region Constructors
		public SheetViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			// Set initial values
			NameField.StringValue = UserName;
			PasswordField.StringValue = Password;

			// Wireup events
			NameField.Changed += (sender, e) => {
				UserName = NameField.StringValue;
			};
			PasswordField.Changed += (sender, e) => {
				Password = PasswordField.StringValue;
			};
		}
		#endregion

		#region Private Methods
		private void CloseSheet() {
			Presentor.DismissViewController (this);
		}
		#endregion

		#region Custom Actions
		partial void AcceptSheet (Foundation.NSObject sender) {
			RaiseSheetAccepted();
			CloseSheet();
		}

		partial void CancelSheet (Foundation.NSObject sender) {
			RaiseSheetCanceled();
			CloseSheet();
		}
		#endregion

		#region Events
		public EventHandler SheetAccepted;

		internal void RaiseSheetAccepted() {
			if (this.SheetAccepted != null)
				this.SheetAccepted (this, EventArgs.Empty);
		}

		public EventHandler SheetCanceled;

		internal void RaiseSheetCanceled() {
			if (this.SheetCanceled != null)
				this.SheetCanceled (this, EventArgs.Empty);
		}
		#endregion
	}
}
