using System;

using Foundation;
using AppKit;
using CoreGraphics;
using ObjCRuntime;

namespace MacWindows
{
	public partial class MainWindow : NSWindow
	{
		#region Computed Properties
		public string Text {
			get { return documentEditor.Value; }
			set { documentEditor.Value = value; }
		}

		public bool ShowPrintAsSheet { get; set;} = true;
		public bool ShowAlertAsSheet { get; set;} = true;
		#endregion

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

			// Show when the document is edited
			documentEditor.TextDidChange += (sender, e) => {
				// Mark the document as dirty
				DocumentEdited = true;
			};

			// Overriding this delegate is required to monitor the TextDidChange event
			documentEditor.ShouldChangeTextInRanges += (NSTextView view, NSValue[] values, string[] replacements) => {
				return true;
			};

			WillClose += (sender, e) => {
				// is the window dirty?
				if (DocumentEdited) {
					var alert = new NSAlert () {
						AlertStyle = NSAlertStyle.Critical,
						InformativeText = "We need to give the user the ability to save the document here...",
						MessageText = "Save Document",
					};
					alert.RunModal ();
				}
			};
		}

		#region Toolbar Handlers
		[Export ("resizeWindow:")]
		void ResizeWindow (NSObject sender) {

			nfloat y = 0;

			// Calculate new origin
			y = Frame.Y - (768 - Frame.Height);

			// Resize and position window
			CGRect frame = new CGRect (Frame.X, y, 1024, 768);
			SetFrame (frame, true);

		}

		[Export ("showDialog:")]
		void ShowDialog (NSObject sender) {

			// Load the new window
			var dialog = new CustomDialogController ();

			// Display the window modally
			NSApplication.SharedApplication.RunModalForWindow (dialog.Window);

		}

		[Export ("showLogin:")]
		void ShowLogin (NSObject sender) {
			var sheet = new LoginSheetController ();
			sheet.LoginRequested += (userID, password) => {
				Console.WriteLine("User ID: {0}, Password: {1}", userID, password);
			};
			sheet.ShowSheet (this);
		}

		[Export ("showPrinter:")]
		void ShowDocument (NSObject sender) {
			var dlg = new NSPrintPanel();

			// Display the print dialog as dialog box
			if (ShowPrintAsSheet) {
				dlg.BeginSheet(new NSPrintInfo(),this,this,null,new IntPtr());
			} else {
				if (dlg.RunModalWithPrintInfo(new NSPrintInfo()) == 1) {
					var alert = new NSAlert () {
						AlertStyle = NSAlertStyle.Warning,
						InformativeText = "We need to print the document here...",
						MessageText = "Print Document",
					};
					alert.RunModal ();
				}
			}
		}

		[Export ("showLayout:")]
		void ShowLayout (NSObject sender) {
			var dlg = new NSPageLayout();

			// Display the print dialog as dialog box
			if (ShowPrintAsSheet) {
				dlg.BeginSheet (new NSPrintInfo (), this);
			} else {
				if (dlg.RunModal () == 1) {
					var alert = new NSAlert () {
						AlertStyle = NSAlertStyle.Critical,
						InformativeText = "We need to print the document here...",
						MessageText = "Print Document",
					};
					alert.RunModal ();
				}
			}
		}

		[Export ("showAlert:")]
		void ShowAlert (NSObject sender) {
			if (ShowAlertAsSheet) {
				var input = new NSTextField (new CGRect (0, 0, 300, 20));

				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Informational,
					InformativeText = "This is the body of the alert where you describe the situation and any actions to correct it.",
					MessageText = "Alert Title",
				};
				alert.AddButton ("Ok");
				alert.AddButton ("Cancel");
				alert.AddButton ("Maybe");
				alert.ShowsSuppressionButton = true;
				alert.AccessoryView = input;
				alert.Layout ();
				alert.BeginSheetForResponse (this, (result) => {
					Console.WriteLine ("Alert Result: {0}, Suppress: {1}", result, alert.SuppressionButton.State == NSCellStateValue.On);
				});
			} else {
				var input = new NSTextField (new CGRect (0, 0, 300, 20));

				var alert = new NSAlert () {
					AlertStyle = NSAlertStyle.Informational,
					InformativeText = "This is the body of the alert where you describe the situation and any actions to correct it.",
					MessageText = "Alert Title",
				};
				alert.AddButton ("Ok");
				alert.AddButton ("Cancel");
				alert.AddButton ("Maybe");
				alert.ShowsSuppressionButton = true;
				alert.AccessoryView = input;
				alert.Layout ();
				var result = alert.RunModal ();
				Console.WriteLine ("Alert Result: {0}, Suppress: {1}", result, alert.SuppressionButton.State == NSCellStateValue.On);
			}
		}
		#endregion
	}
}
