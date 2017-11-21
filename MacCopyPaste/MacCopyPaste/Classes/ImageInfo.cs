using System;

using AppKit;
using Foundation;

namespace MacCopyPaste
{
	[Register("ImageInfo")]
	public class ImageInfo : NSObject, INSCoding, INSPasteboardWriting, INSPasteboardReading
	{
		#region Computed Properties
		[Export("name")]
		public string Name { get; set; }

		[Export("imageType")]
		public string ImageType { get; set; }
		#endregion

		#region Constructors
		[Export ("init")]
		public ImageInfo ()
		{
		}

		public ImageInfo (IntPtr p) : base (p)
		{
		}

		[Export ("initWithCoder:")]
		public ImageInfo(NSCoder decoder)
		{
			// Decode data
			var name = decoder.DecodeObject("name") as NSString;
			var type = decoder.DecodeObject("imageType") as NSString;

			// Save data
            		if (name != null && type != null)
            		{
                		Name = name.ToString();
                		ImageType = type.ToString();
            		}
		}
		#endregion

		#region Public Methods
		[Export ("encodeWithCoder:")]
		public void EncodeTo (NSCoder encoder)
		{
			if (Name != null && ImageType != null)
			{
				encoder.Encode(new NSString(Name), "name");
				encoder.Encode(new NSString(ImageType), "imageType");
			}
		}

		[Export ("writableTypesForPasteboard:")]
		public virtual string[] GetWritableTypesForPasteboard (NSPasteboard pasteboard)
		{
			string[] writableTypes = {"com.xamarin.image-info", "public.text"};
			return writableTypes;
		}

		[Export ("pasteboardPropertyListForType:")]
		public virtual NSObject GetPasteboardPropertyListForType (string type)
		{
			// Take action based on the requested type
			switch (type) {
			case "com.xamarin.image-info":
				return NSKeyedArchiver.ArchivedDataWithRootObject(this);
			case "public.text":
				return new NSString(string.Format("{0}.{1}", Name, ImageType));
			}

			// Failure, return null
			return null;
		}

		[Export ("readableTypesForPasteboard:")]
		public static string[] GetReadableTypesForPasteboard (NSPasteboard pasteboard)
		{
			string[] readableTypes = {"com.xamarin.image-info", "public.text"};
			return readableTypes;
		}

		[Export ("readingOptionsForType:pasteboard:")]
		public static NSPasteboardReadingOptions GetReadingOptionsForType (string type, NSPasteboard pasteboard)
		{
			// Take action based on the requested type
			switch (type) {
			case "com.xamarin.image-info":
				return NSPasteboardReadingOptions.AsKeyedArchive;
			case "public.text":
				return NSPasteboardReadingOptions.AsString;
			}

			// Default to property list
			return NSPasteboardReadingOptions.AsPropertyList;
		}

		[Export ("initWithPasteboardPropertyList:ofType:")]
		public NSObject InitWithPasteboardPropertyList (NSObject propertyList, string type)
		{
			// Take action based on the requested type
			switch (type) {
			case "public.text":
				return new ImageInfo();
			}

			// Failure, return null
			return null;
		}
		#endregion
	}
}
	
