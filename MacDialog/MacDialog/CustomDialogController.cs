using System;
using Foundation;
using AppKit;

namespace MacDialog
{
	public partial class CustomDialogController : NSViewController
	{
		#region Private Variables
		private string _dialogTitle = "Title";
		private string _dialogDescription = "Description";
		private NSViewController _presentor;
		#endregion

		#region Computed Properties
		public string DialogTitle {
			get { return _dialogTitle; }
			set { _dialogTitle = value; }
		}

		public string DialogDescription {
			get { return _dialogDescription; }
			set { _dialogDescription = value; }
		}

		public NSViewController Presentor {
			get { return _presentor; }
			set { _presentor = value; }
		}
		#endregion

		#region Constructors
		public CustomDialogController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			// Set initial title and description
			Title.StringValue = DialogTitle;
			Description.StringValue = DialogDescription;
		}
		#endregion

		#region Private Methods
		private void CloseDialog() {
			Presentor.DismissViewController (this);
		}
		#endregion

		#region Custom Actions
		partial void AcceptDialog (Foundation.NSObject sender) {
			RaiseDialogAccepted();
			CloseDialog();
		}

		partial void CancelDialog (Foundation.NSObject sender) {
			RaiseDialogCanceled();
			CloseDialog();
		}
		#endregion

		#region Events
		public EventHandler DialogAccepted;

		internal void RaiseDialogAccepted() {
			if (this.DialogAccepted != null)
				this.DialogAccepted (this, EventArgs.Empty);
		}

		public EventHandler DialogCanceled;

		internal void RaiseDialogCanceled() {
			if (this.DialogCanceled != null)
				this.DialogCanceled (this, EventArgs.Empty);
		}
		#endregion
	}
}
