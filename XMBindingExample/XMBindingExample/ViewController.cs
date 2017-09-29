using System;

using AppKit;
using Foundation;
using Simple;

namespace XMBindingExample
{
	public partial class ViewController : NSViewController
	{
		public ViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			SimpleClass c = new SimpleClass ();

			var alert = NSAlert.WithMessage("Native Library Value = " + c.DoIt().ToString(), "OK", null, null, String.Empty);
			alert.BeginSheetForResponse (View.Window, x => NSApplication.SharedApplication.Terminate (this) );
		}
	}
}
