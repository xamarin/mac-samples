using AppKit;
using FinderSync;
using Foundation;

namespace FinderSyncExtension
{
	[Register ("FinderSync")]
	public class FinderSync : FIFinderSync
	{
		// Building and run will attempt to register this extension with PluginKit, however often it will not be
		// enabled by default. 
		// Open System Preferences -> Extensions and enable the plugin under Finder if the button does not appear
		// The plugin will appear as an application icon menu to the right of the search bar in Finder by default.

		// Open "Console" application to view the system log to view NSLog / Errors / Crashes of extension
		// Cleaning this project will unregister this plugin from PluginKit.
		// PluginKit register/unregister can be done manually through the Apple pluginkit command line tool.
		// man pluginkit for details

		public override string ToolbarItemName { get; } = "Get URL Extension";

		public override string ToolbarItemToolTip { get; } = "FinderSyncExtension: Click the toolbar item for a menu.";

		public override NSImage ToolbarItemImage { get; } = NSImage.ImageNamed (NSImageName.ApplicationIcon);

		public override NSMenu GetMenu (FIMenuKind menuKind)
		{
			var menu = new NSMenu (string.Empty);
			menu.AddItem ("Get URL", new ObjCRuntime.Selector ("getURL:"), string.Empty);
			return menu;
		}

		[Export ("getURL:")]
		public void GetURL (NSObject sender)
		{
			// FIFinderSyncController.DefaultController is only valid on the thread that invokes this method
			var url = FIFinderSyncController.DefaultController.TargetedURL;

			// See NSLogHelper for details on this vs Console.WriteLine
			ExtensionSamples.NSLogHelper.NSLog ($"FinderSync - GetURL - {url}");

			// But we must get to the UI thread to use NSAlert
			BeginInvokeOnMainThread (() =>
			{
				NSAlert.WithMessage (url.ToString (), "OK", null, null, url.ToString ()).RunModal ();
			});
		}
	}
}
