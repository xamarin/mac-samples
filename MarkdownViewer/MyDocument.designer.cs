// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace Markdown
{
	[Register ("MyDocument")]
	partial class MyDocument
	{
		[Outlet]
		MonoMac.WebKit.WebView WebView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (WebView != null) {
				WebView.Dispose ();
				WebView = null;
			}
		}
	}
}
