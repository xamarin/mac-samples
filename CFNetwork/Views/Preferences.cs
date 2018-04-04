using System;
using Foundation;

namespace MonoMac.CFNetwork.Test.Views
{
	public partial class Preferences : AppKit.NSWindow 
	{
		#region Constructors
		
		// Called when created from unmanaged code
		public Preferences (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public Preferences (NSCoder coder) : base (coder)
		{
			Console.WriteLine ("PREFERENCES CTOR: {0}", Environment.StackTrace);
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion
	}
}

