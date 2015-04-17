using System;

using Foundation;
using AppKit;

namespace MacToolbar
{
	public partial class MainWindow : NSWindow
	{
		public MainWindow (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public MainWindow (NSCoder coder) : base (coder)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Disable trash
			trashItem.Active = false;
		}

		#region Toolbar Handlers
		[Export ("trashDocument:")]
		void TrashDocument (NSObject sender) {

			documentEditor.Value = "";
		}
		#endregion
	}
}
