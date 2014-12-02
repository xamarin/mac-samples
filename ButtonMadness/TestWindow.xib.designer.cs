// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace SamplesButtonMadness
{
	[Register ("TestWindowController")]
	partial class TestWindowController
	{
		[Outlet]
		AppKit.NSBox buttonBox { get; set; }

		[Outlet]
		AppKit.NSMenu buttonMenu { get; set; }

		[Outlet]
		AppKit.NSBox colorBox { get; set; }

		[Outlet]
		SamplesButtonMadness.DropDownButton dropDownButton { get; set; }

		[Outlet]
		AppKit.NSBox indicatorBox { get; set; }

		[Outlet]
		AppKit.NSStepper levelAdjuster { get; set; }

		[Outlet]
		AppKit.NSBox matrixBox { get; set; }

		[Outlet]
		AppKit.NSButton nibBasedButtonRound { get; set; }

		[Outlet]
		AppKit.NSButton nibBasedButtonSquare { get; set; }

		[Outlet]
		AppKit.NSColorWell nibBasedColorWell { get; set; }

		[Outlet]
		AppKit.NSLevelIndicator nibBasedIndicator { get; set; }

		[Outlet]
		AppKit.NSMatrix nibBasedMatrix { get; set; }

		[Outlet]
		AppKit.NSPopUpButton nibBasedPopUpDown { get; set; }

		[Outlet]
		AppKit.NSPopUpButton nibBasedPopUpRight { get; set; }

		[Outlet]
		AppKit.NSSegmentedControl nibBasedSegControl { get; set; }

		[Outlet]
		AppKit.NSView placeHolder1 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder2 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder3 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder4 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder5 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder6 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder7 { get; set; }

		[Outlet]
		AppKit.NSView placeHolder8 { get; set; }

		[Outlet]
		AppKit.NSBox popupBox { get; set; }

		[Outlet]
		AppKit.NSBox segmentBox { get; set; }

		[Action ("buttonAction:")]
		partial void buttonAction (Foundation.NSObject sender);

		[Action ("colorAction:")]
		partial void colorAction (Foundation.NSObject sender);

		[Action ("dropDownAction:")]
		partial void dropDownAction (Foundation.NSObject sender);

		[Action ("levelAction:")]
		partial void levelAction (Foundation.NSObject sender);

		[Action ("levelAdjustAction:")]
		partial void levelAdjustAction (Foundation.NSObject sender);

		[Action ("matrixAction:")]
		partial void matrixAction (Foundation.NSObject sender);

		[Action ("popupAction:")]
		partial void popupAction (Foundation.NSObject sender);

		[Action ("pullsDownAction:")]
		partial void pullsDownAction (Foundation.NSObject sender);

		[Action ("segmentAction:")]
		partial void segmentAction (Foundation.NSObject sender);

		[Action ("setStyleAction:")]
		partial void setStyleAction (Foundation.NSObject sender);

		[Action ("unselectAction:")]
		partial void unselectAction (Foundation.NSObject sender);

		[Action ("useIconAction:")]
		partial void useIconAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonBox != null) {
				buttonBox.Dispose ();
				buttonBox = null;
			}

			if (buttonMenu != null) {
				buttonMenu.Dispose ();
				buttonMenu = null;
			}

			if (colorBox != null) {
				colorBox.Dispose ();
				colorBox = null;
			}

			if (dropDownButton != null) {
				dropDownButton.Dispose ();
				dropDownButton = null;
			}

			if (indicatorBox != null) {
				indicatorBox.Dispose ();
				indicatorBox = null;
			}

			if (levelAdjuster != null) {
				levelAdjuster.Dispose ();
				levelAdjuster = null;
			}

			if (matrixBox != null) {
				matrixBox.Dispose ();
				matrixBox = null;
			}

			if (nibBasedButtonRound != null) {
				nibBasedButtonRound.Dispose ();
				nibBasedButtonRound = null;
			}

			if (nibBasedButtonSquare != null) {
				nibBasedButtonSquare.Dispose ();
				nibBasedButtonSquare = null;
			}

			if (nibBasedColorWell != null) {
				nibBasedColorWell.Dispose ();
				nibBasedColorWell = null;
			}

			if (nibBasedIndicator != null) {
				nibBasedIndicator.Dispose ();
				nibBasedIndicator = null;
			}

			if (nibBasedMatrix != null) {
				nibBasedMatrix.Dispose ();
				nibBasedMatrix = null;
			}

			if (nibBasedPopUpDown != null) {
				nibBasedPopUpDown.Dispose ();
				nibBasedPopUpDown = null;
			}

			if (nibBasedPopUpRight != null) {
				nibBasedPopUpRight.Dispose ();
				nibBasedPopUpRight = null;
			}

			if (nibBasedSegControl != null) {
				nibBasedSegControl.Dispose ();
				nibBasedSegControl = null;
			}

			if (placeHolder1 != null) {
				placeHolder1.Dispose ();
				placeHolder1 = null;
			}

			if (placeHolder2 != null) {
				placeHolder2.Dispose ();
				placeHolder2 = null;
			}

			if (placeHolder3 != null) {
				placeHolder3.Dispose ();
				placeHolder3 = null;
			}

			if (placeHolder4 != null) {
				placeHolder4.Dispose ();
				placeHolder4 = null;
			}

			if (placeHolder5 != null) {
				placeHolder5.Dispose ();
				placeHolder5 = null;
			}

			if (placeHolder6 != null) {
				placeHolder6.Dispose ();
				placeHolder6 = null;
			}

			if (placeHolder7 != null) {
				placeHolder7.Dispose ();
				placeHolder7 = null;
			}

			if (placeHolder8 != null) {
				placeHolder8.Dispose ();
				placeHolder8 = null;
			}

			if (popupBox != null) {
				popupBox.Dispose ();
				popupBox = null;
			}

			if (segmentBox != null) {
				segmentBox.Dispose ();
				segmentBox = null;
			}
		}
	}

	[Register ("DropDownButton")]
	partial class DropDownButton
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("TestWindow")]
	partial class TestWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
