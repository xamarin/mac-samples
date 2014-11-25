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
		public NSPageController PageController { get; set; }

		[Outlet]
		public NSTableView TableView { get; set; }
	}
}
