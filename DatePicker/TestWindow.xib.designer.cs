// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace DatePicker
{
	[Register ("TestWindowController")]
	partial class TestWindowController
	{
		[Outlet]
		AppKit.NSColorWell backColorWell { get; set; }

		[Outlet]
		AppKit.NSButton bezeledCheck { get; set; }

		[Outlet]
		AppKit.NSButton borderedCheck { get; set; }

		[Outlet]
		AppKit.NSMatrix dateElementChecks { get; set; }

		[Outlet]
		AppKit.NSMatrix datePickerModeRadios { get; set; }

		[Outlet]
		AppKit.NSTextField dateResult1 { get; set; }

		[Outlet]
		AppKit.NSTextField dateResult2 { get; set; }

		[Outlet]
		AppKit.NSTextField dateResult3 { get; set; }

		[Outlet]
		AppKit.NSTextField dateResult4 { get; set; }

		[Outlet]
		AppKit.NSTextField dateResult5 { get; set; }

		[Outlet]
		AppKit.NSButton drawsBackgroundCheck { get; set; }

		[Outlet]
		AppKit.NSPopUpButton fontSizePopup { get; set; }

		[Outlet]
		AppKit.NSDatePicker maxDatePicker { get; set; }

		[Outlet]
		AppKit.NSDatePicker minDatePicker { get; set; }

		[Outlet]
		AppKit.NSBox outerBox { get; set; }

		[Outlet]
		AppKit.NSDatePicker overrideDate { get; set; }

		[Outlet]
		AppKit.NSButton overrideDateCheck { get; set; }

		[Outlet]
		AppKit.NSPopUpButton pickerStylePopup { get; set; }

		[Outlet]
		AppKit.NSTextField secondsRangeEdit { get; set; }

		[Outlet]
		AppKit.NSTextField secondsRangeEditLabel { get; set; }

		[Outlet]
		AppKit.NSColorWell textColorWell { get; set; }

		[Outlet]
		AppKit.NSMatrix timeElementChecks { get; set; }

		[Action ("dateOverrideAction:")]
		partial void dateOverrideAction (AppKit.NSButton sender);

		[Action ("dateOverrideChange:")]
		partial void dateOverrideChange (AppKit.NSDatePicker sender);

		[Action ("setBackgroundColor:")]
		partial void setBackgroundColor (AppKit.NSColorWell sender);

		[Action ("setBorderStyle:")]
		partial void setBorderStyle (AppKit.NSMatrix sender);

		[Action ("setDateElementFlags:")]
		partial void setDateElementFlags (AppKit.NSMatrix sender);

		[Action ("setDatePickerMode:")]
		partial void setDatePickerMode (AppKit.NSMatrix sender);

		[Action ("setDrawsBackground:")]
		partial void setDrawsBackground (AppKit.NSButton sender);

		[Action ("setFontSize:")]
		partial void setFontSize (AppKit.NSPopUpButton sender);

		[Action ("setMaxDate:")]
		partial void setMaxDate (AppKit.NSDatePicker sender);

		[Action ("setMinDate:")]
		partial void setMinDate (AppKit.NSDatePicker sender);

		[Action ("setPickerStyle:")]
		partial void setPickerStyle (AppKit.NSPopUpButton sender);

		[Action ("setTextColor:")]
		partial void setTextColor (AppKit.NSColorWell sender);

		[Action ("setTimeElementFlags:")]
		partial void setTimeElementFlags (AppKit.NSMatrix sender);

		[Action ("setToToday:")]
		partial void setToToday (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (backColorWell != null) {
				backColorWell.Dispose ();
				backColorWell = null;
			}

			if (bezeledCheck != null) {
				bezeledCheck.Dispose ();
				bezeledCheck = null;
			}

			if (borderedCheck != null) {
				borderedCheck.Dispose ();
				borderedCheck = null;
			}

			if (dateElementChecks != null) {
				dateElementChecks.Dispose ();
				dateElementChecks = null;
			}

			if (datePickerModeRadios != null) {
				datePickerModeRadios.Dispose ();
				datePickerModeRadios = null;
			}

			if (dateResult1 != null) {
				dateResult1.Dispose ();
				dateResult1 = null;
			}

			if (dateResult2 != null) {
				dateResult2.Dispose ();
				dateResult2 = null;
			}

			if (dateResult3 != null) {
				dateResult3.Dispose ();
				dateResult3 = null;
			}

			if (dateResult4 != null) {
				dateResult4.Dispose ();
				dateResult4 = null;
			}

			if (dateResult5 != null) {
				dateResult5.Dispose ();
				dateResult5 = null;
			}

			if (drawsBackgroundCheck != null) {
				drawsBackgroundCheck.Dispose ();
				drawsBackgroundCheck = null;
			}

			if (fontSizePopup != null) {
				fontSizePopup.Dispose ();
				fontSizePopup = null;
			}

			if (maxDatePicker != null) {
				maxDatePicker.Dispose ();
				maxDatePicker = null;
			}

			if (minDatePicker != null) {
				minDatePicker.Dispose ();
				minDatePicker = null;
			}

			if (outerBox != null) {
				outerBox.Dispose ();
				outerBox = null;
			}

			if (overrideDate != null) {
				overrideDate.Dispose ();
				overrideDate = null;
			}

			if (overrideDateCheck != null) {
				overrideDateCheck.Dispose ();
				overrideDateCheck = null;
			}

			if (pickerStylePopup != null) {
				pickerStylePopup.Dispose ();
				pickerStylePopup = null;
			}

			if (secondsRangeEdit != null) {
				secondsRangeEdit.Dispose ();
				secondsRangeEdit = null;
			}

			if (secondsRangeEditLabel != null) {
				secondsRangeEditLabel.Dispose ();
				secondsRangeEditLabel = null;
			}

			if (textColorWell != null) {
				textColorWell.Dispose ();
				textColorWell = null;
			}

			if (timeElementChecks != null) {
				timeElementChecks.Dispose ();
				timeElementChecks = null;
			}
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
