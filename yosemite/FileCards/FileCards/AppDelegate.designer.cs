using Foundation;
using AppKit;

namespace FileCards
{
	[Register ("AppDelegate")]
	public partial class AppDelegate
	{
		[Outlet]
		NSWindow Window { get; set; }

		[Outlet]
		NSPageController PageController { get; set; }

		[Outlet]
		NSTableView TableView { get; set; }
	}
}
