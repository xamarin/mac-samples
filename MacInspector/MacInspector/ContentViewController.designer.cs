// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacInspector
{
	[Register ("ContentViewController")]
	partial class ContentViewController
	{
		[Outlet]
		AppKit.NSBox BackgroundBox { get; set; }

		[Outlet]
		MacInspector.CustomBox BoxFour { get; set; }

		[Outlet]
		MacInspector.CustomBox BoxOne { get; set; }

		[Outlet]
		MacInspector.CustomBox BoxThree { get; set; }

		[Outlet]
		MacInspector.CustomBox BoxTwo { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BoxFour != null) {
				BoxFour.Dispose ();
				BoxFour = null;
			}

			if (BoxOne != null) {
				BoxOne.Dispose ();
				BoxOne = null;
			}

			if (BoxThree != null) {
				BoxThree.Dispose ();
				BoxThree = null;
			}

			if (BoxTwo != null) {
				BoxTwo.Dispose ();
				BoxTwo = null;
			}

			if (BackgroundBox != null) {
				BackgroundBox.Dispose ();
				BackgroundBox = null;
			}
		}
	}
}
