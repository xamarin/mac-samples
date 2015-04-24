// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacImages
{
	[Register ("SubviewImage")]
	partial class SubviewImage
	{
		[Outlet]
		AppKit.NSImageView Photograph { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Photograph != null) {
				Photograph.Dispose ();
				Photograph = null;
			}
		}
	}
}
