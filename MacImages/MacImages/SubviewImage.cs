using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MacImages
{
	public partial class SubviewImage : AppKit.NSView
	{
		#region Computed Properties
		public NSImage Image {
			get { return Photograph.Image; }
			set { Photograph.Image = value; }
		}
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public SubviewImage (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public SubviewImage (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}
