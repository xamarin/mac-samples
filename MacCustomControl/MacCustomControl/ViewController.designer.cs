// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacCustomControl
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		MacCustomControl.NSFlipSwitch OptionOne { get; set; }

		[Outlet]
		MacCustomControl.NSFlipSwitch OptionTwo { get; set; }

		[Action ("OptionOneFlipped:")]
		partial void OptionOneFlipped (Foundation.NSObject sender);

		[Action ("OptionTwoFlipped:")]
		partial void OptionTwoFlipped (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (OptionOne != null) {
				OptionOne.Dispose ();
				OptionOne = null;
			}

			if (OptionTwo != null) {
				OptionTwo.Dispose ();
				OptionTwo = null;
			}
		}
	}
}
