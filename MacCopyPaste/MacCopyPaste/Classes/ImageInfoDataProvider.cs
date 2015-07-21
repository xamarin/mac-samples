using System;
using AppKit;
using Foundation;

namespace MacCopyPaste
{
	[Register("ImageInfoDataProvider")]
	public class ImageInfoDataProvider : NSPasteboardItemDataProvider
	{
		#region Computed Properties
		public string Name { get; set;}
		public string ImageType { get; set;}
		#endregion


		#region Constructors
		[Export ("init")]
		public ImageInfoDataProvider ()
		{
		}

		public ImageInfoDataProvider (string name, string imageType)
		{
			// Initialize
			this.Name = name;
			this.ImageType = imageType;
		}

		public ImageInfoDataProvider (NSObjectFlag t){
		}

		public ImageInfoDataProvider (IntPtr handle){

		}
		#endregion

		#region Override Methods
		[Export ("pasteboardFinishedWithDataProvider:")]
		public override void FinishedWithDataProvider (NSPasteboard pasteboard)
		{
			
		}

		[Export ("pasteboard:item:provideDataForType:")]
		public override void ProvideDataForType (NSPasteboard pasteboard, NSPasteboardItem item, string type)
		{

			// Take action based on the type
			switch (type) {
			case "public.text":
				// Encode the data to string 
				item.SetStringForType(string.Format("{0}.{1}", Name, ImageType),type);
				break;
			}

		}
		#endregion
	}
}

