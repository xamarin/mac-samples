// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SourceWriter
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField StatusDesc { get; set; }

		[Outlet]
		AppKit.NSTextField StatusLanguage { get; set; }

		[Outlet]
		AppKit.NSTextField StatusText { get; set; }

		[Outlet]
		AppKit.TextKit.Formatter.SourceTextView TextEditor { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (StatusDesc != null) {
				StatusDesc.Dispose ();
				StatusDesc = null;
			}

			if (StatusLanguage != null) {
				StatusLanguage.Dispose ();
				StatusLanguage = null;
			}

			if (StatusText != null) {
				StatusText.Dispose ();
				StatusText = null;
			}

			if (TextEditor != null) {
				TextEditor.Dispose ();
				TextEditor = null;
			}
		}
	}
}
