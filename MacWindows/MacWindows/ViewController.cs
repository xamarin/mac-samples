using System;

using AppKit;
using Foundation;

namespace MacWindows
{
	public partial class ViewController : NSViewController
	{
		#region Computed Properties
		public override NSObject RepresentedObject {
			get {
				return base.RepresentedObject;
			}
			set {
				base.RepresentedObject = value;
				// Update the view, if already loaded.
			}
		}

		public bool DocumentEdited {
			get { return View.Window.DocumentEdited; }
			set { View.Window.DocumentEdited = value; }
		}

		public string Text {
			get { return DocumentEditor.Value; }
			set { DocumentEditor.Value = value; }
		}

		#endregion

		#region Constructors
		public ViewController (IntPtr handle) : base (handle)
		{
		}
		#endregion

		#region Override Methods
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Do any additional setup after loading the view.

		}

		public override void ViewWillAppear ()
		{
			base.ViewWillAppear ();

			// Set Window Title
			this.View.Window.Title = "untitled";

			View.Window.WillClose += (sender, e) => {
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

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			// Show when the document is edited
			DocumentEditor.TextDidChange += (sender, e) => {
				// Mark the document as dirty
				DocumentEdited = true;
			};

			// Overriding this delegate is required to monitor the TextDidChange event
			DocumentEditor.ShouldChangeTextInRanges += (NSTextView view, NSValue[] values, string[] replacements) => {
				return true;
			};


		}
		#endregion

	}
}
