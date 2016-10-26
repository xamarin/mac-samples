using System;

using AppKit;

namespace XMLocalizationSample
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// See CustomBuildActions.targets for a work around for https://bugzilla.xamarin.com/show_bug.cgi?id=45696
			// to make this works on non-english locales
			ButtonResx.Title = Resources.ButtonText;
			LabelResx.StringValue = Resources.LabelText;
		}
	}
}
