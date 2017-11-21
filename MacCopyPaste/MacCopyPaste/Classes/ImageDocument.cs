using System;
using AppKit;
using Foundation;
using ObjCRuntime;

namespace MacCopyPaste
{
	[Register("ImageDocument")]
	public class ImageDocument : NSDocument
	{
		#region Computed Properties
		public NSImageView ImageView {get; set;}

		public ImageInfo Info { get; set; } = new ImageInfo();

		public bool ImageAvailableOnPasteboard {
			get {
				// Initialize the pasteboard
				NSPasteboard pasteboard = NSPasteboard.GeneralPasteboard;
				Class [] classArray  = { new Class ("NSImage") };

				// Check to see if an image is on the pasteboard
				return pasteboard.CanReadObjectForClasses (classArray, null);
			}
		}
		#endregion

		#region Constructor
		public ImageDocument ()
		{
		}
		#endregion

		#region Public Methods
		[Export("CopyImage:")]
		public void CopyImage(NSObject sender) {

			// Grab the current image
			var image = ImageView.Image;

			// Anything to process?
			if (image != null) {
				// Get the standard pasteboard
				var pasteboard = NSPasteboard.GeneralPasteboard;

				// Empty the current contents
				pasteboard.ClearContents();

				// Add the current image to the pasteboard
				pasteboard.WriteObjects (new NSImage[] {image});

				// Save the custom data class to the pastebaord
				pasteboard.WriteObjects (new ImageInfo[] { Info });

				// Using a Pasteboard Item
				NSPasteboardItem item = new NSPasteboardItem();
				string[] writableTypes = {"public.text"};

				// Add a data provier to the item
				ImageInfoDataProvider dataProvider = new ImageInfoDataProvider (Info.Name, Info.ImageType);
				var ok = item.SetDataProviderForTypes (dataProvider, writableTypes);

				// Save to pasteboard
				if (ok) {
					pasteboard.WriteObjects (new NSPasteboardItem[] { item });
				}
			}

		}

		[Export("PasteImage:")]
		public void PasteImage(NSObject sender) {

			// Initialize the pasteboard
			NSPasteboard pasteboard = NSPasteboard.GeneralPasteboard;
			Class [] classArray  = { new Class ("NSImage") };

			bool ok = pasteboard.CanReadObjectForClasses (classArray, null);
			if (ok) {
				// Read the image off of the pasteboard
				NSObject [] objectsToPaste = pasteboard.ReadObjectsForClasses (classArray, null);
				NSImage image = (NSImage)objectsToPaste[0];

				// Display the new image
				ImageView.Image = image;
			}

			Class [] classArray2 = { new Class ("ImageInfo") };
			ok = pasteboard.CanReadObjectForClasses (classArray2, null);
			if (ok) {
				// Read the image off of the pasteboard
				NSObject[] objectsToPaste = pasteboard.ReadObjectsForClasses(classArray2, null);
				if (objectsToPaste.Length > 0)
				{
					ImageInfo info = (ImageInfo)objectsToPaste[0];
				}
			}
		}
		#endregion
	}
}
