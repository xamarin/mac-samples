using System;
using AppKit;
using Foundation;
using ObjCRuntime;

// -----------------------------------------------------------------------------------------------------
// NOTE: This version of the ImageDocument class provides a work around to this defect:
// https://bugzilla.xamarin.com/show_bug.cgi?id=31760
// Once the version of Xamarin.Mac that contains the fix is released to the Alpha Channel, this
// sample will be updated to the correct version.
// -----------------------------------------------------------------------------------------------------
namespace MacCopyPaste
{
	[Register("ImageDocument")]
	public class ImageDocument : NSDocument
	{
		#region DLL Access
		// NOTE: These routines are being defined as a work around to this defect:
		// https://bugzilla.xamarin.com/show_bug.cgi?id=31760
		[System.Runtime.InteropServices.DllImport ("/usr/lib/libobjc.dylib", EntryPoint="objc_msgSend")]
		public extern static global::System.IntPtr IntPtr_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

		[System.Runtime.InteropServices.DllImport ("/usr/lib/libobjc.dylib", EntryPoint="objc_msgSend")]
		public extern static bool bool_objc_msgSend_IntPtr_IntPtr (IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);
		#endregion

		#region Computed Properties
		public NSImageView ImageView {get; set;}

		public ImageInfo Info { get; set; } = new ImageInfo();

		public bool ImageAvailableOnPasteboard {
			get {
				// Initialize the pasteboard
				NSPasteboard pasteboard = NSPasteboard.GeneralPasteboard;
				NSArray classArray = NSArray.FromObjects (new Class ("NSImage"));

				// NOTE: Sending messages directly to the base Objective-C API because of this defect:
				// https://bugzilla.xamarin.com/show_bug.cgi?id=31760
				// Check to see if an image is on the pasteboard
				return bool_objc_msgSend_IntPtr_IntPtr (pasteboard.Handle, Selector.GetHandle ("canReadObjectForClasses:options:"), classArray.Handle, IntPtr.Zero);
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

			}

		}

		[Export("PasteImage:")]
		public void PasteImage(NSObject sender) {

			// Initialize the pasteboard
			NSPasteboard pasteboard = NSPasteboard.GeneralPasteboard;
			NSArray classArray = NSArray.FromObjects (new Class ("NSImage"));

			// NOTE: Sending messages directly to the base Objective-C API because of this defect:
			// https://bugzilla.xamarin.com/show_bug.cgi?id=31760
			// Check to see if an image is on the pasteboard
			bool ok = bool_objc_msgSend_IntPtr_IntPtr (pasteboard.Handle, Selector.GetHandle ("canReadObjectForClasses:options:"), classArray.Handle, IntPtr.Zero);
			if (ok) {
				// Read the image off of the pasteboard
				NSObject [] objectsToPaste = NSArray.ArrayFromHandle<Foundation.NSObject>(IntPtr_objc_msgSend_IntPtr_IntPtr (pasteboard.Handle, Selector.GetHandle ("readObjectsForClasses:options:"), classArray.Handle, IntPtr.Zero));
				NSImage image = (NSImage)objectsToPaste[0];

				// Display the new image
				ImageView.Image = image;
			}


			var classArrayPtrs = new [] { Class.GetHandle (typeof(ImageInfo)) };
			classArray = NSArray.FromIntPtrs (classArrayPtrs);

			ok = bool_objc_msgSend_IntPtr_IntPtr (pasteboard.Handle, Selector.GetHandle ("canReadObjectForClasses:options:"), classArray.Handle, IntPtr.Zero);
			if (ok) {
				// Read the image off of the pasteboard
				NSObject [] objectsToPaste = NSArray.ArrayFromHandle<Foundation.NSObject>(IntPtr_objc_msgSend_IntPtr_IntPtr (pasteboard.Handle, Selector.GetHandle ("readObjectsForClasses:options:"), classArray.Handle, IntPtr.Zero));
				Info = (ImageInfo)objectsToPaste[0];
			}

		}
		#endregion
	}
}

