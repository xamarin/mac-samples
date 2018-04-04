
using System;
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	public partial class UnitTestRunner : AppKit.NSWindow 
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public UnitTestRunner (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public UnitTestRunner (NSCoder coder) : base (coder)
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