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
	[Register ("TemplateViewController")]
	partial class TemplateViewController
	{
		[Outlet]
		AppKit.NSButton ChooseButton { get; set; }

		[Outlet]
		AppKit.NSButton CSharpFile { get; set; }

		[Outlet]
		AppKit.NSButton HTMLFile { get; set; }

		[Outlet]
		AppKit.NSButton MarkDownFile { get; set; }

		[Outlet]
		AppKit.NSButton XMLFile { get; set; }

		[Action ("CancelNewDocument:")]
		partial void CancelNewDocument (Foundation.NSObject sender);

		[Action ("ChooseDocument:")]
		partial void ChooseDocument (Foundation.NSObject sender);

		[Action ("SelectCSharp:")]
		partial void SelectCSharp (Foundation.NSObject sender);

		[Action ("SelectHTML:")]
		partial void SelectHTML (Foundation.NSObject sender);

		[Action ("SelectMarkDown:")]
		partial void SelectMarkDown (Foundation.NSObject sender);

		[Action ("SelectXML:")]
		partial void SelectXML (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CSharpFile != null) {
				CSharpFile.Dispose ();
				CSharpFile = null;
			}

			if (HTMLFile != null) {
				HTMLFile.Dispose ();
				HTMLFile = null;
			}

			if (MarkDownFile != null) {
				MarkDownFile.Dispose ();
				MarkDownFile = null;
			}

			if (XMLFile != null) {
				XMLFile.Dispose ();
				XMLFile = null;
			}

			if (ChooseButton != null) {
				ChooseButton.Dispose ();
				ChooseButton = null;
			}
		}
	}
}
