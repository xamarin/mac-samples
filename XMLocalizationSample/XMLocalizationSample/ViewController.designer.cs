// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace XMLocalizationSample
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton ButtonResx { get; set; }

		[Outlet]
		AppKit.NSButton ButtonStrings { get; set; }

		[Outlet]
		AppKit.NSTextField LabelResx { get; set; }

		[Outlet]
		AppKit.NSTextField LabelStrings { get; set; }

		[Outlet]
		AppKit.NSTabView TabView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TabView != null) {
				TabView.Dispose ();
				TabView = null;
			}

			if (LabelResx != null) {
				LabelResx.Dispose ();
				LabelResx = null;
			}

			if (ButtonResx != null) {
				ButtonResx.Dispose ();
				ButtonResx = null;
			}

			if (ButtonStrings != null) {
				ButtonStrings.Dispose ();
				ButtonStrings = null;
			}

			if (LabelStrings != null) {
				LabelStrings.Dispose ();
				LabelStrings = null;
			}
		}
	}
}
