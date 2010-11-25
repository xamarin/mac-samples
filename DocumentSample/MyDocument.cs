
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
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MyDocument (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		void Initialize ()
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
		
		override 
	}
}

