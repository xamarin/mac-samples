
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace DockAppIcon
{
        public partial class MainWindowController : MonoMac.AppKit.NSWindowController
        {

                NSApplication NSApp = NSApplication.SharedApplication;
                NSDockTile dockTile;
                bool isShowBadge = false;
                const string defaultFormat = "{0}%";
                bool useCustomFormat = false;
                NSTimer myTimer;

                // Call to load from the XIB/NIB file
                public MainWindowController () : base("MainWindow")
                {
                        Initialize ();
                }

                // Shared initialization code
                void Initialize ()
                {
                }

                //strongly typed window accessor
                public new MainWindow Window {
                        get { return (MainWindow)base.Window; }
                }

                public override void AwakeFromNib ()
                {
                        dockTile = NSApp.DockTile;
                        badgeNumber.EditingEnded += HandleBadgeNumberEditingEnded;
                        customFormat.EditingEnded += HandleCustomFormatEditingEnded;
                }

                void HandleCustomFormatEditingEnded (object sender, EventArgs e)
                {
                        updateBadge ();
                }

                void HandleBadgeNumberEditingEnded (object sender, EventArgs e)
                {
                        updateBadge ();
                }

                private void updateBadge ()
                {
                        if (isShowBadge) {
                                if (useCustomFormat) {
                                        dockTile.BadgeLabel = string.Format (customFormat.StringValue, badgeNumber.StringValue);
                                } else {
                                        dockTile.BadgeLabel = string.Format (defaultFormat, badgeNumber.StringValue);
                                }
                        } else {
                                dockTile.BadgeLabel = null;
                        }
                }

                partial void stepperAction (NSStepper sender)
                {
                        badgeNumber.StringValue = sender.StringValue;
                        updateBadge ();
                }

                partial void badgeCheckAction (NSButton sender)
                {
                        isShowBadge = (badgeCheck.State == NSCellStateValue.On) ? true : false;
                        updateBadge ();
                }

                partial void customFormatAction (NSButton sender)
                {
                        useCustomFormat = (formatCheck.State == NSCellStateValue.On) ? true : false;
                        updateBadge ();
                }

                partial void showAppBadgeAction (NSButton sender)
                {
                        Window.DockTile.ShowsApplicationBadge = (showAppBadgeCheck.State == NSCellStateValue.On) ? true : false;
                }

                partial void requestAction (NSButton sender)
                {
                        myTimer = NSTimer.CreateScheduledTimer (3.0, delegate {
                                if (popupRequestType.Cell.SelectedItemIndex == 0)
                                        NSApp.RequestUserAttention (NSRequestUserAttentionType.InformationalRequest);
                                else
                                        NSApp.RequestUserAttention (NSRequestUserAttentionType.CriticalRequest);
                        });
                }
        }
}

