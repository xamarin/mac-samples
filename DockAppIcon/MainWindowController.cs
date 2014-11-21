using System;

using Foundation;
using AppKit;

namespace DockAppIcon
{
	public partial class MainWindowController : NSWindowController
	{
		string lastValidCustomFormat = "{0}%";
	
		public MainWindowController () : base ("MainWindow")
		{
		}

		public override void AwakeFromNib ()
		{
			customFormat.StringValue = lastValidCustomFormat;
		
			badgeCheck.Activated += (sender, e) => UpdateBadge ();
			formatCheck.Activated += (sender, e) => UpdateBadge ();
			customFormat.Changed += (sender, e) => UpdateBadge ();

			showAppBadgeCheck.Activated += (sender, e) =>
				Window.DockTile.ShowsApplicationBadge = showAppBadgeCheck.State == NSCellStateValue.On;

			stepper.Activated += (sender, e) => {
				badgeNumber.StringValue = stepper.StringValue;
				UpdateBadge ();
			};

			requestButton.Activated += (sender, e) => {
				new NSAlert { MessageText = "Focus another application window, wait 3 seconds, then look at your dock!" }.BeginSheet (Window, () => {
					var type = popupRequestType.Cell.SelectedItemIndex == 0
						? NSRequestUserAttentionType.InformationalRequest
						: NSRequestUserAttentionType.CriticalRequest;

					NSTimer.CreateScheduledTimer (3.0, (d) => NSApplication.SharedApplication.RequestUserAttention (type));
				});
			};

			UpdateBadge ();
		}

		private void UpdateBadge ()
		{
			var dockTile = NSApplication.SharedApplication.DockTile;
		
			if (badgeCheck.State == NSCellStateValue.Off) {
				dockTile.BadgeLabel = null;
			} else if (formatCheck.State == NSCellStateValue.On) {
				try {
					dockTile.BadgeLabel = String.Format (customFormat.StringValue, badgeNumber.StringValue);
					lastValidCustomFormat = customFormat.StringValue;
				} catch {
					dockTile.BadgeLabel = String.Format (lastValidCustomFormat, badgeNumber.StringValue);
				}
			} else {
				dockTile.BadgeLabel = badgeNumber.StringValue;
			}
		}
	}
}