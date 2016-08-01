using System;

using AppKit;

namespace ExtensionSamples
{
	public partial class ViewController : NSViewController
	{
		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Extension projects needs a container application.
			TextView.Value = "This is just a container app for the interesting extensions in the sample. " +
				"Follow the instructions from README.md file.";
			// Do any additional setup after loading the view.
		}
	}
}
