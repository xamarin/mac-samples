using System;
using AppKit;
using Foundation;

namespace VisualEffectPlayground
{
	partial class TestLauncherViewController : NSViewController
	{
		public TestLauncherViewController () : base ("TestLauncherViewController", Foundation.NSBundle.MainBundle)
		{
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
		}

		partial void BtnClickBasicSidebarPlusText (NSButton sender)
		{
			var viewControler = new BasicSidebarViewController ();
			PresentViewControllerAsModalWindow (viewControler);
		}

		partial void BtnClickBasicSidebarPlusImages (NSButton sender)
		{
			var viewControler = new BasicSidebarViewController ("BasicSidebarViewControllerImageDemo");
			PresentViewControllerAsModalWindow (viewControler);
		}

		partial void BtnClickBasicSidebarPlusCustomViews (NSButton sender)
		{
			var viewControler = new BasicSidebarViewController ("BasicSidebarViewControllerCustomVibrancyDemo");
			PresentViewControllerAsModalWindow (viewControler);
		}

		partial void BtnClickBasicSidebarPlusCustomViews2 (NSButton sender)
		{
			var viewControler = new BasicSidebarViewController ("BasicSidebarViewControllerCustomVibrancyDemo2");
			PresentViewControllerAsModalWindow (viewControler);
		}

		partial void BtnPerformanceExampleClicked (NSButton sender)
		{
			var viewController = new VibrantControlsViewController ("PerformanceExampleViewController") {
				Title = "Performance Example"
			};
			PresentViewControllerAsModalWindow (viewController);
		}

		partial void BtnMaskImageWindowClicked (NSButton sender)
		{
			var viewController = new VibrantControlsViewController ("MaskExampleViewController") {
				Title = "Masks"
			};
			PresentViewControllerAsModalWindow (viewController);
		}

		partial void BtnVibrantColorsClicked (NSButton sender)
		{
			var viewController = new VibrantControlsViewController ("VibrantColorsViewController") {
				Title = "System Colors"
			};
			PresentViewControllerAsModalWindow (viewController);
		}

		partial void BtnSampleMapsClicked (NSButton sender)
		{
			var wc = new SampleMapsWindowController ();
			wc.ShowWindow (null);
		}

		partial void BtnDemoFacetimeClicked (NSButton sender)
		{
			var wc = new DemoFaceTimeWindowController ();
			wc.ShowWindow (null);
		}

		partial void BtnVibrantControlsCaveatsBehindWindowClicked (NSButton sender)
		{
			var viewController = new VibrantControlsViewController ("VibrantControlsCaveatsBehindWindow") {
				Title = "Caveats Behind Window"
			};
			PresentViewControllerAsModalWindow (viewController);
		}

		partial void BtnVibrantControlsCaveatsClicked (NSButton sender)
		{
			var viewController = new VibrantControlsViewController ("VibrantControlsCaveatsInWindow") {
				Title = "Caveats In Window"
			};
			PresentViewControllerAsModalWindow (viewController);
		}

		partial void BtnVibrantControlsClicked (NSButton sender)
		{
			PresentViewControllerAsModalWindow (new VibrantControlsViewController ());
		}
	}
}

