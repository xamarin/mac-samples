
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace DocumentSample
{
	[Register ("MyDocument")]
	public partial class MyDocument : NSDocument
	{
		// Called when created from unmanaged code
		public MyDocument (IntPtr handle) : base (handle)
		{
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MyDocument (NSCoder coder) : base(coder)
		{
		}
		
		// Override returning the nib file name of the document
		// If you need to use a subclass of NSWindowController or if your document supports
		// multiple NSWindowControllers remove this method and override MakeWindowControllers
		// instead		
		public override string WindowNibName {
			get {

				return "MyDocument";
			}
		}

		public override void WindowControllerDidLoadNib (NSWindowController windowController)
		{
			base.WindowControllerDidLoadNib (windowController);
			
			// Add code to here after the controller has loaded the document window
		}
		
		// 
		// Save support:
		//    Override one of GetAsData, GetAsFileWrapper, or WriteToUrl.
		//
		
		// This method should store the contents of the document using the given typeName
		// on the return NSData value.
		public override NSData GetAsData (string documentType, out NSError outError)
		{
			outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
			return null;
		}
		
		// 
		// Load support:
		//    Override one of ReadFromData, ReadFromFileWrapper or ReadFromUrl
		//
		public override bool ReadFromData (NSData data, string typeName, out NSError outError)
		{
			outError = NSError.FromDomain (NSError.OsStatusErrorDomain, -4);
			return false;
		}
	}
}

