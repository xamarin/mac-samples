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
	[Register ("CheckboxViewController")]
	partial class CheckboxViewController
	{
		[Outlet]
		AppKit.NSButton AdjustTime { get; set; }

		[Outlet]
		AppKit.NSTextField FeedbackLabel { get; set; }

		[Outlet]
		AppKit.NSButtonCell TransportationCar { get; set; }

		[Outlet]
		AppKit.NSButtonCell TransportationPublic { get; set; }

		[Outlet]
		AppKit.NSButtonCell TransportationWalking { get; set; }

		[Action ("SelectCar:")]
		partial void SelectCar (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (AdjustTime != null) {
				AdjustTime.Dispose ();
				AdjustTime = null;
			}

			if (FeedbackLabel != null) {
				FeedbackLabel.Dispose ();
				FeedbackLabel = null;
			}

			if (TransportationCar != null) {
				TransportationCar.Dispose ();
				TransportationCar = null;
			}

			if (TransportationPublic != null) {
				TransportationPublic.Dispose ();
				TransportationPublic = null;
			}

			if (TransportationWalking != null) {
				TransportationWalking.Dispose ();
				TransportationWalking = null;
			}
		}
	}
}
