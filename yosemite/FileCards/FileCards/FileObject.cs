using System;
using System.Collections.Generic;

using Foundation;
using AppKit;

namespace FileCards
{
	[Register ("FileObject")]
	public class FileObject : NSObject
	{
		public NSUrl Url {
			[Export("url")]
			get;
			private set;
		}

		public NSString Name {
			[Export("name")]
			get {
				return (NSString)GetResourceValue (NSUrl.NameKey);
			}
		}

		public NSString LocalizedName {
			[Export("localizedName")]
			get {
				return (NSString)GetResourceValue (NSUrl.LocalizedNameKey);
			}
		}

		public NSDate DateOfCreation {
			[Export("dateOfCreation")]
			get {
				return (NSDate)GetResourceValue (NSUrl.CreationDateKey);
			}
		}

		public NSDate DateOfLastModification {
			[Export("dateOfLastModification")]
			get {
				return (NSDate)GetResourceValue (NSUrl.ContentModificationDateKey);
			}
		}

		public NSNumber SizeInBytes {
			[Export("sizeInBytes")]
			get {
				return (NSNumber)GetResourceValue (NSUrl.FileSizeKey);
			}
		}

		public NSString UtiType {
			[Export("utiType")]
			get {
				return (NSString)GetResourceValue (NSUrl.TypeIdentifierKey);
			}
		}

		public NSImage Icon {
			[Export("icon")]
			get {
				return (NSImage)GetResourceValue (NSUrl.EffectiveIconKey);
			}
		}

		public FileObject (NSUrl url)
		{
			Url = url;
		}

		NSObject GetResourceValue (NSString key)
		{
			NSObject value;
			NSError error;

			if (Url.TryGetResource (key, out value, out error))
				return value;

			throw new NSErrorException (error);
		}

		public override string ToString ()
		{
			return Url.ToString ();
		}
	}
}