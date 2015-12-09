// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace MacControls
{
	[Register ("ButtonsViewController")]
	partial class ButtonsViewController
	{
		[Outlet]
		AppKit.NSButton ButtonOutlet { get; set; }

		[Outlet]
		AppKit.NSButton DisclosureButton { get; set; }

		[Outlet]
		AppKit.NSTextField FeedbackLabel { get; set; }

		[Outlet]
		AppKit.NSButton HelpButton { get; set; }

		[Outlet]
		AppKit.NSTextField LorumIpsum { get; set; }

		[Outlet]
		AppKit.NSButton RoundedGradient { get; set; }

		[Action ("ButtonAction:")]
		partial void ButtonAction (Foundation.NSObject sender);

		[Action ("RecessedAction:")]
		partial void RecessedAction (Foundation.NSObject sender);

		[Action ("RoundAction:")]
		partial void RoundAction (Foundation.NSObject sender);

		[Action ("RoundRectAction:")]
		partial void RoundRectAction (Foundation.NSObject sender);

		[Action ("RoundTexturedAction:")]
		partial void RoundTexturedAction (Foundation.NSObject sender);

		[Action ("SquareAction:")]
		partial void SquareAction (Foundation.NSObject sender);

		[Action ("TexturedAction:")]
		partial void TexturedAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ButtonOutlet != null) {
				ButtonOutlet.Dispose ();
				ButtonOutlet = null;
			}

			if (DisclosureButton != null) {
				DisclosureButton.Dispose ();
				DisclosureButton = null;
			}

			if (FeedbackLabel != null) {
				FeedbackLabel.Dispose ();
				FeedbackLabel = null;
			}

			if (LorumIpsum != null) {
				LorumIpsum.Dispose ();
				LorumIpsum = null;
			}

			if (RoundedGradient != null) {
				RoundedGradient.Dispose ();
				RoundedGradient = null;
			}

			if (HelpButton != null) {
				HelpButton.Dispose ();
				HelpButton = null;
			}
		}
	}
}
