using System;
using AppKit;

namespace MacCopyPaste
{
	public class EditMenuDelegate : NSMenuDelegate
	{
		#region Override Methods
		public override void MenuWillHighlightItem (NSMenu menu, NSMenuItem item)
		{
		}

		public override void NeedsUpdate (NSMenu menu)
		{
			// Get list of menu items
			NSMenuItem[] Items = menu.ItemArray ();

			// Get the key window and determine if the required images are available
			var window = NSApplication.SharedApplication.KeyWindow as MainWindow;
			var hasImage = (window != null) && (window.Image != null);
			var hasImageOnPasteboard = (window != null) && window.Document.ImageAvailableOnPasteboard;

			// Process every item in the menu
			foreach(NSMenuItem item in Items) {
				// Take action based on the menu title
				switch (item.Title) {
				case "Cut":
				case "Copy":
				case "Delete":
					// Only enable if there is an image in the view
					item.Enabled = hasImage;
					break;
				case "Paste":
					// Only enable if there is an image on the pasteboard
					item.Enabled = hasImageOnPasteboard;
					break;
				default:
					// Only enable the item if it has a sub menu
					item.Enabled = item.HasSubmenu;
					break;
				}
			}
		}
		#endregion
	}
}

