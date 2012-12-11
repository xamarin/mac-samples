
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace DatePicker
{
        public partial class TestWindowController : MonoMac.AppKit.NSWindowController
        {

                // control we will add by code.
                NSDatePicker datePickerControl;

                int shrinkGrowFacter;

                enum DateElementSelections
                {
                        YearMonth,
                        YearMonthDay,
                        Era
                }

                enum TimeElementSelections
                {
                        HourMinute,
                        HourMinuteSecond,
                        TimeZone
                }

                enum DatePickerModeSelections
                {
                        SingleDateMode,
                        RangeDateMode
                }

                #region Constructors

                // Called when created from unmanaged code
                public TestWindowController (IntPtr handle) : base(handle)
                {
                        Initialize ();
                }

                // Called when created directly from a XIB file
                [Export("initWithCoder:")]
                public TestWindowController (NSCoder coder) : base(coder)
                {
                        Initialize ();
                }

                // Call to load from the XIB/NIB file
                public TestWindowController () : base("TestWindow")
                {
                        Initialize ();
                }

                // Shared initialization code
                void Initialize ()
                {
                }

                #endregion

                //strongly typed window accessor
                public new TestWindow Window {
                        get { return (TestWindow)base.Window; }
                }

                public override void AwakeFromNib ()
                {
                        base.AwakeFromNib ();
                        
                        NSDateFormatter.DefaultBehavior = NSDateFormatterBehavior.Default;
                        
                        setupDatePickerControl (NSDatePickerStyle.ClockAndCalendar);
                        
                        // setup the initial NSDatePickerElementFlags since we are using picker style: NSClockAndCalendarDatePickerStyle
                        NSDatePickerElementFlags flags = 0;
                        flags |= NSDatePickerElementFlags.YearMonthDate;
                        flags |= NSDatePickerElementFlags.YearMonthDateDay;
                        flags |= NSDatePickerElementFlags.Era;
                        flags |= NSDatePickerElementFlags.HourMinute;
                        flags |= NSDatePickerElementFlags.HourMinuteSecond;
                        flags |= NSDatePickerElementFlags.TimeZone;
                        
                        datePickerControl.DatePickerElements = flags;
                        
                        minDatePicker.DateValue = DateTime.Now;
                        maxDatePicker.DateValue = NSDate.DistantFuture;
                        
                        secondsRangeEdit.EditingEnded += SecondsRangeEditingEnded;
                        
                        updateControls ();
                        // force update of all UI elements and the picker itself
                }

                /// <summary>
                /// Delegate method to respond to changes in the Seconds Range field.
                /// </summary>
                /// <param name="sender">
                /// A <see cref="System.Object"/>
                /// </param>
                /// <param name="e">
                /// A <see cref="EventArgs"/>
                /// </param>
                void SecondsRangeEditingEnded (object sender, EventArgs e)
                {
                        updateDatePickerMode ();
                }

                /// <summary>
                /// Create and setup a new NSDatePicker
                /// </summary>
                /// <param name="pickerStyle">
                /// A <see cref="NSDatePickerStyle"/>
                /// </param>
                private void setupDatePickerControl (NSDatePickerStyle pickerStyle)
                {
                        
                        RectangleF frame = new RectangleF (10, 10, 295, 154);
                        shrinkGrowFacter = (int)frame.Size.Height - 30;
                        
                        // create the date picker control if not created already
                        if (datePickerControl == null)
                                datePickerControl = new NSDatePicker (frame);
                        
                        datePickerControl.DatePickerStyle = pickerStyle;
                        // set our desired picker style
                        outerBox.AddSubview (datePickerControl);
                        
                        datePickerControl.DrawsBackground = true;
                        datePickerControl.Bordered = false;
                        datePickerControl.Bezeled = true;
                        datePickerControl.Enabled = true;
                        
                        datePickerControl.TextColor = textColorWell.Color;
                        datePickerControl.BackgroundColor = backColorWell.Color;
                        
                        // always set the date/time to TODAY
                        datePickerControl.DateValue = DateTime.Now;
                        
                        datePickerControl.NeedsDisplay = true;
                        updateControls ();
                        // force update of all UI elements and the picker itself
                        // synch the picker style popup with the new style change
                        pickerStylePopup.SelectItemWithTag ((int)pickerStyle);
                        
                        // we want to be the cell's delegate to catch date validation
                        datePickerControl.WeakDelegate = this;
                        // or we can set us as the delegate to its cell like so:
                        // datePickerControl.Cell.WeakDelegate = this;
                        
                        // we want to respond to date/time changes
                        datePickerControl.Action = new MonoMac.ObjCRuntime.Selector ("datePickerAction:");
                        
                }

                /// <summary>
                /// Respond to date/time changes of the NSDatePicker control.  This is the method
                /// that is called as the user interacts with the control.
                /// </summary>
                [Export("datePickerAction:")]
                private void datePickerAction ()
                {
                        updateDateResult ();
                }

                /// <summary>
                /// Force update of all UI elements and the picker itself.
                /// </summary>
                private void updateControls ()
                {
                        datePickerControl.NeedsDisplay = true;
                        // force it to update
                        updateDatePickerMode ();
                        updateDateTimeElementFlags ();
                        updateDateResult ();
                }

                /// <summary>
                /// Updates the text field values representing different date styles.
                /// </summary>
                private void updateDateResult ()
                {
                        NSDate theDate = datePickerControl.DateValue;
                        
                        if (theDate != null) {
                                NSDateFormatter formatter = new NSDateFormatter ();
                                
                                string formattedDateString;
                                
                                                                /* some examples:
				formatter.DateStyle = NSDateFormatterStyle.None;		// <no date displayed>
				formatter.DateStyle = NSDateFormatterStyle.Medium;		// Jan 24, 1984
				formatter.DateStyle = NSDateFormatterStyle.Short;		// 1/24/84
				formatter.DateStyle = NSDateFormatterStyle.Long;		// January 24, 1984
				formatter.DateStyle = NSDateFormatterStyle.Full;		// Tuesday, January 24, 1984
				
				formatter.TimeStyle = NSDateFormatterStyle.None;		// <no time displayed>
				formatter.TimeStyle = NSDateFormatterStyle.Short;		// 2:44 PM
				formatter.TimeStyle = NSDateFormatterStyle.Medium;		// 2:44:55 PM
				formatter.TimeStyle = NSDateFormatterStyle.Long;		// 2:44:55 PM PDT
				formatter.TimeStyle = NSDateFormatterStyle.Full;		// 2:44:55 PM PDT
				*/


formatter.DateStyle = NSDateFormatterStyle.Short;
                                formatter.TimeStyle = NSDateFormatterStyle.None;
                                formattedDateString = formatter.ToString (theDate);
                                dateResult1.StringValue = formattedDateString;
                                
                                formatter.DateStyle = NSDateFormatterStyle.Short;
                                formatter.TimeStyle = NSDateFormatterStyle.Short;
                                formattedDateString = formatter.ToString (theDate);
                                dateResult2.StringValue = formattedDateString;
                                
                                formatter.DateStyle = NSDateFormatterStyle.Medium;
                                formatter.TimeStyle = NSDateFormatterStyle.Short;
                                formattedDateString = formatter.ToString (theDate);
                                dateResult3.StringValue = formattedDateString;
                                
                                formatter.DateStyle = NSDateFormatterStyle.Long;
                                formatter.TimeStyle = NSDateFormatterStyle.Short;
                                formattedDateString = formatter.ToString (theDate);
                                dateResult4.StringValue = formattedDateString;
                                
                                formatter.DateStyle = NSDateFormatterStyle.Full;
                                formatter.TimeStyle = NSDateFormatterStyle.Full;
                                formattedDateString = formatter.ToString (theDate);
                                dateResult5.StringValue = formattedDateString;
                        }
                }

                #region NSDATEPICKER

                /// <summary>
                /// User chose a different picker style from the Picker Style popup.
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSPopUpButton"/>
                /// </param>
                partial void setPickerStyle (NSPopUpButton sender)
                {
                        int tag = sender.SelectedCell.Tag;
                        
                        if (datePickerControl.DatePickerStyle != (NSDatePickerStyle)tag) {
                                RectangleF windowFrame = this.Window.Frame;
                                RectangleF boxFrame = outerBox.Frame;
                                
                                datePickerControl.Hidden = true;
                                
                                if ((NSDatePickerStyle)tag == NSDatePickerStyle.ClockAndCalendar) {
                                        
                                        // for this picker style, we need to grow the window to make room for it.
                                        SizeF size = windowFrame.Size;
                                        size.Height += shrinkGrowFacter;
                                        windowFrame.Size = size;
                                        PointF origin = windowFrame.Location;
                                        origin.Y -= shrinkGrowFacter;
                                        windowFrame.Location = origin;
                                        
                                        size = boxFrame.Size;
                                        size.Height += shrinkGrowFacter;
                                        boxFrame.Size = size;
                                        outerBox.Frame = boxFrame;
                                        
                                        this.Window.SetFrame (windowFrame, true, true);
                                        
                                        datePickerControl.DatePickerStyle = NSDatePickerStyle.ClockAndCalendar;
                                        
                                        // shows these last
                                        dateResult1.Hidden = false;
                                        dateResult2.Hidden = false;
                                        dateResult3.Hidden = false;
                                        dateResult4.Hidden = false;
                                } else {
                                        NSDatePickerStyle currentPickerStyle = datePickerControl.DatePickerStyle;
                                        
                                        // shrink the window only if the current style is "clock and calendar"
                                        if (currentPickerStyle == NSDatePickerStyle.ClockAndCalendar) {
                                                
                                                dateResult1.Hidden = true;
                                                dateResult2.Hidden = true;
                                                dateResult3.Hidden = true;
                                                dateResult4.Hidden = true;
                                                
                                                SizeF size = windowFrame.Size;
                                                size.Height -= shrinkGrowFacter;
                                                windowFrame.Size = size;
                                                PointF origin = windowFrame.Location;
                                                origin.Y += shrinkGrowFacter;
                                                windowFrame.Location = origin;
                                                
                                                size = boxFrame.Size;
                                                size.Height -= shrinkGrowFacter;
                                                boxFrame.Size = size;
                                                outerBox.Frame = boxFrame;
                                                
                                                this.Window.SetFrame (windowFrame, true, true);
                                        }
                                        
                                        // set our desired picker style
                                        setupDatePickerControl ((NSDatePickerStyle)tag);
                                        
                                }
                                
                                datePickerControl.Hidden = false;
                                
                                updateControls ();
                                // force update of all UI elements and the picker itself
                        }
                        
                }

                /// <summary>
                /// User chose a different control font size from the Font Size popup
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSPopUpButton"/>
                /// </param>
                partial void setFontSize (NSPopUpButton sender)
                {
                        int tag = sender.SelectedCell.Tag;
                        
                        switch ((NSControlSize)tag) {
                        case NSControlSize.Mini:
                                datePickerControl.Cell.ControlSize = NSControlSize.Mini;
                                datePickerControl.Cell.Font = NSFont.SystemFontOfSize (9.0f);
                                break;
                        case NSControlSize.Small:
                                datePickerControl.Cell.ControlSize = NSControlSize.Small;
                                datePickerControl.Cell.Font = NSFont.SystemFontOfSize (11.0f);
                                break;
                        case NSControlSize.Regular:
                                datePickerControl.Cell.ControlSize = NSControlSize.Regular;
                                datePickerControl.Cell.Font = NSFont.SystemFontOfSize (13.0f);
                                break;
                        }
                }

                partial void setToToday (NSButton sender)
                {
                        datePickerControl.DateValue = DateTime.Now;
                }

                partial void dateOverrideAction (NSButton sender)
                {
                        if (sender.SelectedCell.State == NSCellStateValue.On) {
                                datePickerControl.WeakDelegate = this;
                                datePickerControl.DateValue = overrideDate.DateValue;
                        } else {
                                datePickerControl.Delegate = null;
                                datePickerControl.DateValue = DateTime.Now;
                        }
                        
                }

                /// <summary>
                /// 
                /// When returning a new proposedDateValue, the NSDate instance should be autoreleased, and the 
                /// proposedDateValue should not be released by the delegate.
                /// 
                /// The proposedDateValue and proposedTimeInterval are guaranteed to lie between the dates returned 
                /// by minDate and maxDate. 
                /// 
                /// If you modify these values, you should ensure that the new values are within the appropriate range.
                /// 
                /// </summary>
                /// <param name="aDatePickerCell">
                /// A <see cref="NSDatePickerCell"/>
                /// 
                /// The cell that sent the message.
                ///
                /// </param>
                /// <param name="proposedDateValue">
                /// A <see cref="NSDate"/>
                /// 
                /// On input, contains the proposed new date. The delegate may change this value before returning.
                /// 
                /// </param>
                /// <param name="proposedTimeInterval">
                /// A <see cref="System.Double"/>
                /// 
                /// On input, contains the proposed new time interval. The delegate may change this value before returning.
                /// 
                /// </param>
                [Export("datePickerCell:validateProposedDateValue:timeInterval:")]
                void ValidateProposedDateValue (NSDatePickerCell aDatePickerCell, ref NSDate proposedDateValue, double proposedTimeInterval)
                {
                        TestWindowController controller = (TestWindowController)aDatePickerCell.WeakDelegate;
                        if (controller != null && controller == this && aDatePickerCell == datePickerControl.Cell) {
                                // override code goes here.  You should ensure that the new values are within the appropriate range.
                                if (overrideDateCheck.SelectedCell.State == NSCellStateValue.On)
                                        // override the date using the user specified date
                                        proposedDateValue = overrideDate.DateValue;
                                
                                // NOTE:  I think there is a problem setting the proposed Date value as it is a ref
                        }
                }

                partial void dateOverrideChange (NSDatePicker sender)
                {
                        if (overrideDateCheck.SelectedCell.State == NSCellStateValue.On)
                                // force the delegate to be called.
                                datePickerControl.DateValue = overrideDate.DateValue;
                }

                #endregion

                #region APPEARANCE

                /// <summary>
                /// The user checked/unchecked the "Draws Background" checkbox
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSButton"/>
                /// </param>
                partial void setDrawsBackground (NSButton sender)
                {
                        datePickerControl.DrawsBackground = (sender.State == NSCellStateValue.On);
                }

                /// <summary>
                /// The user checked/unchecked the "Bezeled" checkbox
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSButton"/>
                /// </param>
                partial void setBezeled (NSButton sender)
                {
                        datePickerControl.Bezeled = sender.State == NSCellStateValue.On;
                }

                /// <summary>
                /// The user checked/unchecked the "Bordered" checkbox
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSButton"/>
                /// </param>
                partial void setBordered (NSButton sender)
                {
                        datePickerControl.Bordered = sender.State == NSCellStateValue.On;
                }

                /// <summary>
                /// The user chose a different background color from the "Back Color" color well
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSColorWell"/>
                /// </param>
                partial void setBackgroundColor (NSColorWell sender)
                {
                        datePickerControl.BackgroundColor = sender.Color;
                }

                /// <summary>
                /// The user chose a different text color from the "Text Color" color well
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSColorWell"/>
                /// </param>
                partial void setTextColor (NSColorWell sender)
                {
                        datePickerControl.TextColor = sender.Color;
                }

                #endregion

                #region DATE TIME ELEMENTS

                /// <summary>
                /// The user checked/unchecked one of the "Date Element" checkboxes.
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSMatrix"/>
                /// </param>
                partial void setDateElementFlags (NSMatrix sender)
                {
                        NSDatePickerElementFlags flags = datePickerControl.DatePickerElements;
                        
                        bool checkedState = sender.SelectedCell.State == NSCellStateValue.On;
                        
                        switch ((DateElementSelections)sender.SelectedCell.Tag) {
                        
                        case DateElementSelections.YearMonth:
                                if (checkedState)
                                        flags |= NSDatePickerElementFlags.YearMonthDate;
                                else
                                        flags ^= NSDatePickerElementFlags.YearMonthDate;
                                break;
                        case DateElementSelections.YearMonthDay:
                                if (checkedState)
                                        flags |= NSDatePickerElementFlags.YearMonthDateDay;
                                else
                                        flags ^= NSDatePickerElementFlags.YearMonthDateDay;
                                break;
                        case DateElementSelections.Era:
                                if (checkedState)
                                        flags |= NSDatePickerElementFlags.Era;
                                else
                                        flags ^= NSDatePickerElementFlags.Era;
                                break;
                        }
                        
                        datePickerControl.DatePickerElements = flags;
                        
                        updateControls ();
                }

                /// <summary>
                /// The user checked/unchecked one of the "Time Element" checkboxes
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSMatrix"/>
                /// </param>
                partial void setTimeElementFlags (NSMatrix sender)
                {
                        NSDatePickerElementFlags flags = datePickerControl.DatePickerElements;
                        
                        bool checkedState = sender.SelectedCell.State == NSCellStateValue.On;
                        
                        switch ((TimeElementSelections)sender.SelectedCell.Tag) {
                        
                        case TimeElementSelections.HourMinute:
                                if (checkedState)
                                        flags |= NSDatePickerElementFlags.HourMinute;
                                else
                                        flags ^= NSDatePickerElementFlags.HourMinute;
                                break;
                        case TimeElementSelections.HourMinuteSecond:
                                if (checkedState)
                                        flags |= NSDatePickerElementFlags.HourMinuteSecond;
                                else
                                        flags ^= NSDatePickerElementFlags.HourMinuteSecond;
                                break;
                        case TimeElementSelections.TimeZone:
                                if (checkedState)
                                        flags |= NSDatePickerElementFlags.TimeZone;
                                else
                                        flags ^= NSDatePickerElementFlags.TimeZone;
                                break;
                        }
                        
                        datePickerControl.DatePickerElements = flags;
                        
                        updateControls ();
                }

                /// <summary>
                /// Updates our checkboxes to reflect the current control flags
                /// </summary>
                private void updateDateTimeElementFlags ()
                {
                        NSDatePickerElementFlags elementFlags = datePickerControl.DatePickerElements;
                        
                        // time elements
                        if ((elementFlags & NSDatePickerElementFlags.HourMinute) != 0)
                                timeElementChecks.SelectCellWithTag ((int)TimeElementSelections.HourMinute);
                        if ((elementFlags & NSDatePickerElementFlags.HourMinuteSecond) != 0)
                                timeElementChecks.SelectCellWithTag ((int)TimeElementSelections.HourMinuteSecond);
                        if ((elementFlags & NSDatePickerElementFlags.TimeZone) != 0)
                                timeElementChecks.SelectCellWithTag ((int)TimeElementSelections.TimeZone);
                        
                        // date elements
                        if ((elementFlags & NSDatePickerElementFlags.YearMonthDate) != 0)
                                dateElementChecks.SelectCellWithTag ((int)DateElementSelections.YearMonth);
                        if ((elementFlags & NSDatePickerElementFlags.YearMonthDateDay) != 0)
                                dateElementChecks.SelectCellWithTag ((int)DateElementSelections.YearMonthDay);
                        if ((elementFlags & NSDatePickerElementFlags.Era) != 0)
                                dateElementChecks.SelectCellWithTag ((int)DateElementSelections.Era);
                        
                        
                }
                #endregion

                #region PICKER MODE

                /// <summary>
                /// User wants to change the "Date Picker Mode"
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSMatrix"/>
                /// </param>
                partial void setDatePickerMode (NSMatrix sender)
                {
                        switch ((DatePickerModeSelections)sender.SelectedCell.Tag) {
                        case DatePickerModeSelections.SingleDateMode:
                                datePickerControl.DatePickerMode = NSDatePickerMode.Single;
                                break;
                        case DatePickerModeSelections.RangeDateMode:
                                datePickerControl.DatePickerMode = NSDatePickerMode.Range;
                                break;
                        }
                        
                        updateControls ();
                        // force update of all UI elements and the picker itself
                }

                /// <summary>
                /// Method to update the NSDatePicker's NSDatePickerMode attributes.
                /// </summary>
                private void updateDatePickerMode ()
                {
                        NSDatePickerMode mode = datePickerControl.DatePickerMode;
                        
                        switch (mode) {
                        case NSDatePickerMode.Single:
                                datePickerModeRadios.SelectCellWithTag (0);
                                
                                // interval value not applicable
                                secondsRangeEdit.Enabled = false;
                                secondsRangeEditLabel.TextColor = NSColor.LightGray;
                                
                                datePickerControl.TimeInterval = 0;
                                break;
                        case NSDatePickerMode.Range:
                                datePickerModeRadios.SelectCellWithTag (1);
                                
                                // interval value applies
                                secondsRangeEdit.Enabled = true;
                                secondsRangeEditLabel.TextColor = NSColor.Black;
                                
                                // set the date range by start date (here we use the current date in the date picker control), 
                                // and time interval (in seconds)
                                string secsStr = secondsRangeEdit.StringValue;
                                int numSeconds = Int32.Parse (secsStr);
                                datePickerControl.TimeInterval = numSeconds;
                                break;
                        }
                }

                #endregion

                #region PICKER MIN/MAX DATE
                /// <summary>
                /// User wants to set the minimum date for the picker
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSDatePicker"/>
                /// </param>
                partial void setMinDate (NSDatePicker sender)
                {
                        datePickerControl.MinDate = sender.DateValue;
                }

                /// <summary>
                /// User wants to set the maximum date for the picker
                /// </summary>
                /// <param name="sender">
                /// A <see cref="NSDatePicker"/>
                /// </param>
                partial void setMaxDate (NSDatePicker sender)
                {
                        datePickerControl.MaxDate = sender.DateValue;
                }
                #endregion
        }
}

