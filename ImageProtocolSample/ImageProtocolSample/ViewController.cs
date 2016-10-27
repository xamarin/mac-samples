using System;

using AppKit;
using Foundation;
using WebKit;

namespace ImageProtocolSample
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Register our custom url protocol
			NSUrlProtocol.RegisterClass (new ObjCRuntime.Class (typeof (ImageProtocol)));

			var web = new WebView {
				AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable
			};

			web.Frame = View.Bounds;
			View.AutoresizesSubviews = true;
			View.AddSubview (web);

			web.MainFrame.LoadRequest (NSUrlRequest.FromUrl (NSUrl.FromFilename (NSBundle.MainBundle.PathForResource ("test", "html"))));
		}
	}
}
