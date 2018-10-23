// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace RoundedTransparentWindow
{
    [Register ("CustomWindow")]
    partial class CustomWindow
    {
        [Outlet]
        AppKit.NSSlider slider { get; set; }
        
        void ReleaseDesignerOutlets ()
        {
            if (slider != null) {
                slider.Dispose ();
                slider = null;
            }
        }
    }
}
