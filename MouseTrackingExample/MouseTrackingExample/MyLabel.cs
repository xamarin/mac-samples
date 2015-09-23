using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace MouseTrackingExample
{
	public partial class MyLabel : AppKit.NSTextField
	{
		#region Constructors

		// Called when created from unmanaged code
		public MyLabel (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public MyLabel (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void AwakeFromNib ()
		{
			this.StringValue = string.Empty;
		}
	}
}
