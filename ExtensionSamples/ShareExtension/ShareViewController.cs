using System;
using System.Linq;
using AppKit;
using Foundation;

namespace ShareExtension
{
	public partial class ShareViewController : NSViewController
	{
		// Building and run will attempt to register this extension with PluginKit, however sometimes it will not be
		// enabled by default. 
		// Open System Preferences -> Extensions and enable the plugin under "Share Menu" if it does not appear
		// in the share menu
		// The plugin show up under the share menu, and running will launch Safari

		// Open "Console" application to view the system log to view NSLog / Errors / Crashes of extension
		// Cleaning this project will unregister this plugin from PluginKit.
		// PluginKit register/unregister can be done manually through the Apple pluginkit command line tool.
		// man pluginkit for details

		public ShareViewController (IntPtr handle) : base (handle)
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();

			// Show the URL of the item that was requested.
			NSExtensionItem item = ExtensionContext.InputItems.First ();
			NSItemProvider provider = item.Attachments[0];
			provider.LoadItem ("public.url", null, (arg1, arg2) =>
			{
				var url = (NSUrl)arg1;

				// See NSLogHelper for details on this vs Console.WriteLine
				ExtensionSamples.NSLogHelper.NSLog ($"ShareViewController - LoadView - {url}");

				BeginInvokeOnMainThread (() =>
				{
					TitleText.StringValue = url.ToString ();
				});
			});
		}

		partial void Close (NSObject sender)
		{
			var outputItem = new NSExtensionItem ();
			var outputItems = new[] { outputItem };
			ExtensionContext.CompleteRequest (outputItems, null);
		}
	}
}

