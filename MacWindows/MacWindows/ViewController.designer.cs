// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacWindows
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextView DocumentEditor { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DocumentEditor != null) {
				DocumentEditor.Dispose ();
				DocumentEditor = null;
			}
		}
	}
}
