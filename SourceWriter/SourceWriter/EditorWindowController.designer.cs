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
	[Register ("EditorWindowController")]
	partial class EditorWindowController
	{
		[Outlet]
		AppKit.ManualToolbarItem ToolbarAutoComplete { get; set; }

		[Outlet]
		AppKit.ManualToolbarItem ToolbarDefinition { get; set; }

		[Outlet]
		AppKit.ManualToolbarItem ToolbarIndent { get; set; }

		[Outlet]
		AppKit.ManualToolbarItem ToolbarOutdent { get; set; }

		[Outlet]
		AppKit.ManualToolbarItem ToolbarPrint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ToolbarAutoComplete != null) {
				ToolbarAutoComplete.Dispose ();
				ToolbarAutoComplete = null;
			}

			if (ToolbarDefinition != null) {
				ToolbarDefinition.Dispose ();
				ToolbarDefinition = null;
			}

			if (ToolbarIndent != null) {
				ToolbarIndent.Dispose ();
				ToolbarIndent = null;
			}

			if (ToolbarOutdent != null) {
				ToolbarOutdent.Dispose ();
				ToolbarOutdent = null;
			}

			if (ToolbarPrint != null) {
				ToolbarPrint.Dispose ();
				ToolbarPrint = null;
			}
		}
	}
}
