
using System;
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	public partial class LogViewer : AppKit.NSWindow 
	{
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

