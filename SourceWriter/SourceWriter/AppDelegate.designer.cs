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
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem DefinitionMenuItem { get; set; }

		[Outlet]
		AppKit.NSMenu FormatMenu { get; set; }

		[Outlet]
		AppKit.NSMenuItem IndentMenuItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem OutdentMenuItem { get; set; }

		[Outlet]
		AppKit.NSMenuItem ReformatMenuItem { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FormatMenu != null) {
				FormatMenu.Dispose ();
				FormatMenu = null;
			}

			if (OutdentMenuItem != null) {
				OutdentMenuItem.Dispose ();
				OutdentMenuItem = null;
			}

			if (IndentMenuItem != null) {
				IndentMenuItem.Dispose ();
				IndentMenuItem = null;
			}

			if (DefinitionMenuItem != null) {
				DefinitionMenuItem.Dispose ();
				DefinitionMenuItem = null;
			}

			if (ReformatMenuItem != null) {
				ReformatMenuItem.Dispose ();
				ReformatMenuItem = null;
			}
		}
	}
}
