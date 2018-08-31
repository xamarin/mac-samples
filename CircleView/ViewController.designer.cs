// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace CircleView
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		CircleView CircleView { get; set; }

		[Action ("ApplyColor:")]
		partial void ApplyColor (Foundation.NSObject sender);

		[Action ("ApplyRadius:")]
		partial void ApplyRadius (Foundation.NSObject sender);

		[Action ("ApplySpin:")]
		partial void ApplySpin (Foundation.NSObject sender);

		[Action ("ApplyStartingAngle:")]
		partial void ApplyStartingAngle (Foundation.NSObject sender);

		[Action ("ApplyText:")]
		partial void ApplyText (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CircleView != null) {
				CircleView.Dispose ();
				CircleView = null;
			}
		}
	}
}
