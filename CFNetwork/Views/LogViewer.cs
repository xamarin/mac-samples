
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MonoMac.CFNetwork.Test.Views {

	public partial class LogViewer : MonoMac.AppKit.NSWindow {
		#region Constructors
		
		// Called when created from unmanaged code
		public LogViewer (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LogViewer (NSCoder coder) : base (coder)
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

