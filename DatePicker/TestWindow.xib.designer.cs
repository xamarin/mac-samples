// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace DatePicker
{
	[Register ("TestWindowController")]
	partial class TestWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSColorWell backColorWell { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton bezeledCheck { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton borderedCheck { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMatrix dateElementChecks { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMatrix datePickerModeRadios { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField dateResult1 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField dateResult2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField dateResult3 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField dateResult4 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField dateResult5 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton drawsBackgroundCheck { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton fontSizePopup { get; set; }

		[Outlet]
		MonoMac.AppKit.NSDatePicker maxDatePicker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSDatePicker minDatePicker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSBox outerBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSDatePicker overrideDate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton overrideDateCheck { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton pickerStylePopup { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField secondsRangeEdit { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField secondsRangeEditLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSColorWell textColorWell { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMatrix timeElementChecks { get; set; }

		[Action ("dateOverrideAction:")]
		partial void dateOverrideAction (MonoMac.AppKit.NSButton sender);

		[Action ("dateOverrideChange:")]
		partial void dateOverrideChange (MonoMac.AppKit.NSDatePicker sender);

		[Action ("setBackgroundColor:")]
		partial void setBackgroundColor (MonoMac.AppKit.NSColorWell sender);

		[Action ("setBorderStyle:")]
		partial void setBorderStyle (MonoMac.AppKit.NSMatrix sender);

		[Action ("setDateElementFlags:")]
		partial void setDateElementFlags (MonoMac.AppKit.NSMatrix sender);

		[Action ("setDatePickerMode:")]
		partial void setDatePickerMode (MonoMac.AppKit.NSMatrix sender);

		[Action ("setDrawsBackground:")]
		partial void setDrawsBackground (MonoMac.AppKit.NSButton sender);

		[Action ("setFontSize:")]
		partial void setFontSize (MonoMac.AppKit.NSPopUpButton sender);

		[Action ("setMaxDate:")]
		partial void setMaxDate (MonoMac.AppKit.NSDatePicker sender);

		[Action ("setMinDate:")]
		partial void setMinDate (MonoMac.AppKit.NSDatePicker sender);

		[Action ("setPickerStyle:")]
		partial void setPickerStyle (MonoMac.AppKit.NSPopUpButton sender);

		[Action ("setTextColor:")]
		partial void setTextColor (MonoMac.AppKit.NSColorWell sender);

		[Action ("setTimeElementFlags:")]
		partial void setTimeElementFlags (MonoMac.AppKit.NSMatrix sender);

		[Action ("setToToday:")]
		partial void setToToday (MonoMac.AppKit.NSButton sender);
		
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
